using Application.Approvals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuickApproval.Api.Controllers
{
    [ApiController]
    [Route("api/approvals")]
    [Authorize(Roles = "Manager,Admin")]
    public sealed class ApprovalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ApprovalsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("pending")]
        public async Task<IActionResult> Pending(CancellationToken ct)
            => Ok(await _mediator.Send(new GetPendingApprovalsQuery(), ct));

        [HttpPost("{requestId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid requestId, [FromBody] ApproveBody body, CancellationToken ct)
        {
            var r = await _mediator.Send(new ApproveRequestCommand(requestId, body.Comment), ct);
            return r.IsSuccess ? Ok() : BadRequest(r.Error);
        }

        [HttpPost("{requestId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid requestId, [FromBody] RejectBody body, CancellationToken ct)
        {
            var r = await _mediator.Send(new RejectRequestCommand(requestId, body.Reason), ct);
            return r.IsSuccess ? Ok() : BadRequest(r.Error);
        }

        public sealed record ApproveBody(string? Comment);
        public sealed record RejectBody(string Reason);
    }
}
