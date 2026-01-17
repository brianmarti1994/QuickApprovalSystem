using Application.Abstractions;
using Application.Requests;
using Domain.Requests;
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

namespace QuickApproval.Tests.Requests
{
    public class CreateRequestHandlerTests
    {
        [Fact]
        public async Task CreateRequest_Fails_When_Workflow_Not_Found()
        {
            // Arrange
            var db = new Mock<IAppDbContext>();
            db.Setup(x => x.Workflows).ReturnsDbSet(Array.Empty<Workflow>());
            db.Setup(x => x.Requests).ReturnsDbSet(Array.Empty<Request>());

            var me = new CurrentUserStub { UserId = Guid.NewGuid(), Roles = new[] { "Employee" } };

            var handler = new CreateRequestHandler(db.Object, me);

            // Act
            var result = await handler.Handle(
                new CreateRequestCommand("T", "D", 10m, Guid.NewGuid()),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error!.Code.Should().Be("workflow.notFound");
        }

        [Fact]
        public async Task CreateRequest_Success_Adds_Request_And_Saves()
        {
            // Arrange
            var wf = Workflow.Create("Default");

            var db = new Mock<IAppDbContext>();

            // Workflows exists
            db.Setup(x => x.Workflows)
              .ReturnsDbSet(new[] { wf });

            // Requests DbSet exists (we don't rely on list mutation)
            db.Setup(x => x.Requests)
              .ReturnsDbSet(new List<Request>());

            db.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(1);

            var me = new CurrentUserStub
            {
                UserId = Guid.NewGuid(),
                Roles = new[] { "Employee" }
            };

            var handler = new CreateRequestHandler(db.Object, me);

            // Act
            var result = await handler.Handle(
                new CreateRequestCommand("Laptop", "Need laptop", 1000m, wf.Id),
                CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            db.Verify(x => x.Requests.Add(It.Is<Request>(r =>
                r.Title == "Laptop" &&
                r.Description == "Need laptop" &&
                r.Amount == 1000m &&
                r.WorkflowId == wf.Id &&
                r.CreatedByUserId == me.UserId
            )), Times.Once);

            db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
    }
    }
}
