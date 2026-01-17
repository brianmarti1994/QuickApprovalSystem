using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Approvals
{
    public sealed record PendingApprovalItem(Guid RequestId, string Title, decimal Amount, int StepOrder, DateTime SubmittedAtUtc);
    public sealed record GetPendingApprovalsQuery() : IRequest<IReadOnlyList<PendingApprovalItem>>;

    public sealed class GetPendingApprovalsHandler : IRequestHandler<GetPendingApprovalsQuery, IReadOnlyList<PendingApprovalItem>>
    {
        private readonly IAppDbContext _db;

        public GetPendingApprovalsHandler(IAppDbContext db) => _db = db;

        public async Task<IReadOnlyList<PendingApprovalItem>> Handle(GetPendingApprovalsQuery q, CancellationToken ct)
        {
            // The filtering by role is done in the next query via workflow step role check (see Approve/Reject).
            // For speed, you can also pre-join workflow steps here if needed.
            return await _db.Requests
                .Where(r => r.Status == Domain.Requests.RequestStatus.PendingApproval)
                .OrderBy(r => r.SubmittedAtUtc)
                .Select(r => new PendingApprovalItem(
                    r.Id, r.Title, r.Amount, r.CurrentStepOrder, r.SubmittedAtUtc ?? r.CreatedAtUtc))
                .ToListAsync(ct);
        }
    }
}
