using Application.Abstractions;
using Application.Approvals;
using Domain.Requests;
using Domain.Users;
using Domain.Workflows;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using QuickApproval.Tests.TestDoubles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickApproval.Tests.Approvals
{
    public class ApproveRejectHandlerTests
    {
        [Fact]
        public async Task Approve_Fails_When_Request_Not_Found()
        {
            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(Array.Empty<Request>());
            db.Setup(x => x.Workflows).ReturnsDbSet(Array.Empty<Workflow>());

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Manager" } };
            var handler = new ApproveRequestHandler(db.Object, me);

            var result = await handler.Handle(new ApproveRequestCommand(Guid.NewGuid(), "ok"), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("request.notFound");
        }

        [Fact]
        public async Task Approve_Fails_When_User_Not_In_Required_Role()
        {
            var wf = Workflow.Create("WF");
            wf.AddStep(1, Role.Manager, "Manager approval");

            var req = Request.Create(Guid.NewGuid(), wf.Id, "T", "D", 10m);
            req.Submit(); // now PendingApproval

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(new[] { req });
            db.Setup(x => x.Workflows).ReturnsDbSet(new[] { wf });

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Employee" } }; // not Manager
            var handler = new ApproveRequestHandler(db.Object, me);

            var result = await handler.Handle(new ApproveRequestCommand(req.Id, null), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("approval.forbidden");
        }

        [Fact]
        public async Task Approve_Marks_Request_Approved_When_Last_Step()
        {
            var wf = Workflow.Create("WF");
            wf.AddStep(1, Role.Manager, "Manager approval"); // only 1 step

            var req = Request.Create(Guid.NewGuid(), wf.Id, "T", "D", 10m);
            req.Submit(); // PendingApproval

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(new[] { req });
            db.Setup(x => x.Workflows).ReturnsDbSet(new[] { wf });
            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Manager" } };
            var handler = new ApproveRequestHandler(db.Object, me);

            var result = await handler.Handle(new ApproveRequestCommand(req.Id, "approved"), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            req.Status.Should().Be(RequestStatus.Approved);
            req.Decisions.Should().HaveCount(1);
            req.Decisions.First().IsApproved.Should().BeTrue();

            db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Reject_Marks_Request_Rejected()
        {
            var wf = Workflow.Create("WF");
            wf.AddStep(1, Role.Manager, "Manager approval");

            var req = Request.Create(Guid.NewGuid(), wf.Id, "T", "D", 10m);
            req.Submit();

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(new[] { req });
            db.Setup(x => x.Workflows).ReturnsDbSet(new[] { wf });
            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Manager" } };
            var handler = new RejectRequestHandler(db.Object, me);

            var result = await handler.Handle(new RejectRequestCommand(req.Id, "no"), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            req.Status.Should().Be(RequestStatus.Rejected);
            req.Decisions.Should().HaveCount(1);
            req.Decisions.First().IsApproved.Should().BeFalse();
        }
    }
}
