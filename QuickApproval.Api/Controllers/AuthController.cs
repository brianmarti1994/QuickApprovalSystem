using Application.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuickApproval.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand cmd, CancellationToken ct)
        {
            var result = await _mediator.Send(cmd, ct);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
        }
    }
}
