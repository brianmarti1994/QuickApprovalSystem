using Application.Abstractions;
using Domain.Common;
using Domain.Users;
using Domain.Workflows;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Workflows
{
    public sealed record CreateWorkflowCommand(string Name, IReadOnlyList<StepInput> Steps) : IRequest<Result<Guid>>;
    public sealed record StepInput(int Order, Role ApproverRole, string Label);

    public sealed class CreateWorkflowHandler : IRequestHandler<CreateWorkflowCommand, Result<Guid>>
    {
        private readonly IAppDbContext _db;
        public CreateWorkflowHandler(IAppDbContext db) => _db = db;

        public async Task<Result<Guid>> Handle(CreateWorkflowCommand cmd, CancellationToken ct)
        {
            var wf = Workflow.Create(cmd.Name);

            foreach (var s in cmd.Steps.OrderBy(x => x.Order))
            {
                var r = wf.AddStep(s.Order, s.ApproverRole, s.Label);
                if (!r.IsSuccess) return Result<Guid>.Fail(r.Error!.Code, r.Error!.Message);
            }

            _db.Workflows.Add(wf);
            await _db.SaveChangesAsync(ct);
            return Result<Guid>.Ok(wf.Id);
        }
    }
}
