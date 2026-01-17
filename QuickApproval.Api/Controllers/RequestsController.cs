using Application.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuickApproval.Api.Controllers
{
    [ApiController]
    [Route("api/requests")]
    [Authorize]
    public sealed class RequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RequestsController(IMediator mediator) => _mediator = mediator;

        // Employee: Create New Request
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRequestCommand cmd, CancellationToken ct)
        {
            var r = await _mediator.Send(cmd, ct);
            return r.IsSuccess ? Ok(new { id = r.Value }) : BadRequest(r.Error);
        }

        // Employee: Submit
        [HttpPost("{id:guid}/submit")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Submit([FromRoute] Guid id, CancellationToken ct)
        {
            var r = await _mediator.Send(new SubmitRequestCommand(id), ct);
            return r.IsSuccess ? Ok() : BadRequest(r.Error);
        }

        // Employee: View Request History
        [HttpGet("mine")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> Mine(CancellationToken ct)
            => Ok(await _mediator.Send(new GetMyRequestsQuery(), ct));

        // Request Details + Audit Trail (Employee own, Manager/Admin any)
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Details([FromRoute] Guid id, CancellationToken ct)
        {
            var r = await _mediator.Send(new GetRequestDetailsQuery(id), ct);
            return r.IsSuccess ? Ok(r.Value) : Forbid();
        }
    }
}
