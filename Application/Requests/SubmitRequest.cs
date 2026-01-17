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
    public sealed record SubmitRequestCommand(Guid RequestId) : IRequest<Result>;

    public sealed class SubmitRequestHandler : IRequestHandler<SubmitRequestCommand, Result>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public SubmitRequestHandler(IAppDbContext db, ICurrentUser me)
        {
            _db = db; _me = me;
        }

        public async Task<Result> Handle(SubmitRequestCommand cmd, CancellationToken ct)
        {
            var req = await _db.Requests.FirstOrDefaultAsync(r => r.Id == cmd.RequestId, ct);
            if (req is null) return Result.Fail("request.notFound", "Request not found.");
            if (req.CreatedByUserId != _me.UserId) return Result.Fail("request.forbidden", "Not your request.");

            var result = req.Submit();
            if (!result.IsSuccess) return result;

            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
