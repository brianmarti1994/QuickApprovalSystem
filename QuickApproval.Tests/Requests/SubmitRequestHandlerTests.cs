using Application.Abstractions;
using Application.Requests;
using Domain.Requests;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using QuickApproval.Tests.TestDoubles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickApproval.Tests.Requests
{
    public class SubmitRequestHandlerTests
    {
        [Fact]
        public async Task Submit_Fails_When_Request_Not_Found()
        {
            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(Array.Empty<Request>());

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Employee" } };
            var handler = new SubmitRequestHandler(db.Object, me);

            var result = await handler.Handle(new SubmitRequestCommand(Guid.NewGuid()), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("request.notFound");
        }

        [Fact]
        public async Task Submit_Fails_When_Not_Owner()
        {
            var ownerId = Guid.NewGuid();
            var otherUser = Guid.NewGuid();

            var req = Request.Create(ownerId, Guid.NewGuid(), "T", "D", 10);
            // it starts as Draft

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(new[] { req });

            var me = new CurrentUserStub { UserId = otherUser, Roles = new[] { "Employee" } };
            var handler = new SubmitRequestHandler(db.Object, me);

            var result = await handler.Handle(new SubmitRequestCommand(req.Id), CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("request.forbidden");
        }

        [Fact]
        public async Task Submit_Success_Saves()
        {
            var ownerId = Guid.NewGuid();
            var req = Request.Create(ownerId, Guid.NewGuid(), "T", "D", 10);

            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Requests).ReturnsDbSet(new[] { req });
            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var me = new CurrentUserStub { UserId = ownerId, Roles = new[] { "Employee" } };
            var handler = new SubmitRequestHandler(db.Object, me);

            var result = await handler.Handle(new SubmitRequestCommand(req.Id), CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            req.Status.Should().Be(RequestStatus.PendingApproval);

            db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
