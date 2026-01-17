using Application.Abstractions;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Approvals
{
    public sealed record ApproveRequestCommand(Guid RequestId, string? Comment) : IRequest<Result>;
    public sealed record RejectRequestCommand(Guid RequestId, string Reason) : IRequest<Result>;

    public sealed class ApproveRequestHandler : IRequestHandler<ApproveRequestCommand, Result>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public ApproveRequestHandler(IAppDbContext db, ICurrentUser me) { _db = db; _me = me; }

        public async Task<Result> Handle(ApproveRequestCommand cmd, CancellationToken ct)
        {
            var req = await _db.Requests.Include(r => r.Decisions).FirstOrDefaultAsync(r => r.Id == cmd.RequestId, ct);
            if (req is null) return Result.Fail("request.notFound", "Request not found.");

            var wf = await _db.Workflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == req.WorkflowId, ct);
            if (wf is null || !wf.IsActive) return Result.Fail("workflow.notFound", "Workflow not found.");

            var step = wf.Steps.FirstOrDefault(s => s.Order == req.CurrentStepOrder);
            if (step is null) return Result.Fail("workflow.step.missing", "Current workflow step missing.");

            if (!_me.IsInRole(step.ApproverRole.ToString()))
                return Result.Fail("approval.forbidden", $"Only {step.ApproverRole} can approve this step.");

            var result = req.Approve(_me.UserId, cmd.Comment);
            if (!result.IsSuccess) return result;

            // if next step doesn't exist -> approved
            var nextStep = wf.Steps.FirstOrDefault(s => s.Order == req.CurrentStepOrder);
            if (nextStep is null)
                req.MarkApproved();

            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }

    public sealed class RejectRequestHandler : IRequestHandler<RejectRequestCommand, Result>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public RejectRequestHandler(IAppDbContext db, ICurrentUser me) { _db = db; _me = me; }

        public async Task<Result> Handle(RejectRequestCommand cmd, CancellationToken ct)
        {
            var req = await _db.Requests.Include(r => r.Decisions).FirstOrDefaultAsync(r => r.Id == cmd.RequestId, ct);
            if (req is null) return Result.Fail("request.notFound", "Request not found.");

            var wf = await _db.Workflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == req.WorkflowId, ct);
            if (wf is null || !wf.IsActive) return Result.Fail("workflow.notFound", "Workflow not found.");

            var step = wf.Steps.FirstOrDefault(s => s.Order == req.CurrentStepOrder);
            if (step is null) return Result.Fail("workflow.step.missing", "Current workflow step missing.");

            if (!_me.IsInRole(step.ApproverRole.ToString()))
                return Result.Fail("approval.forbidden", $"Only {step.ApproverRole} can reject this step.");

            var result = req.Reject(_me.UserId, cmd.Reason);
            if (!result.IsSuccess) return result;

            await _db.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
