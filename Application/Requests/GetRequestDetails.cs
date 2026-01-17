using Application.Abstractions;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests
{
    public sealed record DecisionDto(int StepOrder, bool IsApproved, string Comment, Guid ActorUserId, DateTime AtUtc);
    public sealed record RequestDetailsDto(
        Guid Id, string Title, string Description, decimal Amount, string Status, int CurrentStepOrder,
        Guid CreatedByUserId, Guid WorkflowId, DateTime CreatedAtUtc, IReadOnlyList<DecisionDto> Decisions);

    public sealed record GetRequestDetailsQuery(Guid RequestId) : IRequest<Result<RequestDetailsDto>>;

    public sealed class GetRequestDetailsHandler : IRequestHandler<GetRequestDetailsQuery, Result<RequestDetailsDto>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public GetRequestDetailsHandler(IAppDbContext db, ICurrentUser me)
        {
            _db = db; _me = me;
        }

        public async Task<Result<RequestDetailsDto>> Handle(GetRequestDetailsQuery q, CancellationToken ct)
        {
            var req = await _db.Requests
                .Include(r => r.Decisions)
                .FirstOrDefaultAsync(r => r.Id == q.RequestId, ct);

            if (req is null) return Result<RequestDetailsDto>.Fail("request.notFound", "Request not found.");

            // allow creator, managers/admins (history/audit per your diagram)
            var canView = req.CreatedByUserId == _me.UserId || _me.IsInRole("Manager") || _me.IsInRole("Admin");
            if (!canView) return Result<RequestDetailsDto>.Fail("request.forbidden", "Forbidden.");

            var dto = new RequestDetailsDto(
                req.Id, req.Title, req.Description, req.Amount, req.Status.ToString(), req.CurrentStepOrder,
                req.CreatedByUserId, req.WorkflowId, req.CreatedAtUtc,
                req.Decisions
                    .OrderBy(d => d.StepOrder).ThenBy(d => d.AtUtc)
                    .Select(d => new DecisionDto(d.StepOrder, d.IsApproved, d.Comment, d.ActorUserId, d.AtUtc))
                    .ToList());

            return Result<RequestDetailsDto>.Ok(dto);
        }
    }
}
