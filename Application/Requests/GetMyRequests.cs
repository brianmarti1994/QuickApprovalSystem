using Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests
{
    public sealed record RequestListItem(Guid Id, string Title, decimal Amount, string Status, DateTime CreatedAtUtc);
    public sealed record GetMyRequestsQuery() : IRequest<IReadOnlyList<RequestListItem>>;

    public sealed class GetMyRequestsHandler : IRequestHandler<GetMyRequestsQuery, IReadOnlyList<RequestListItem>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public GetMyRequestsHandler(IAppDbContext db, ICurrentUser me)
        {
            _db = db; _me = me;
        }

        public async Task<IReadOnlyList<RequestListItem>> Handle(GetMyRequestsQuery q, CancellationToken ct)
        {
            return await _db.Requests
                .Where(r => r.CreatedByUserId == _me.UserId)
                .OrderByDescending(r => r.CreatedAtUtc)
                .Select(r => new RequestListItem(r.Id, r.Title, r.Amount, r.Status.ToString(), r.CreatedAtUtc))
                .ToListAsync(ct);
        }
    }
}
