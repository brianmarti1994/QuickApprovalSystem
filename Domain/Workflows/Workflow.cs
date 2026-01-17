using Domain.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Workflows
{
    public sealed class Workflow : AggregateRoot<Guid>
    {
        private Workflow() { }

        public string Name { get; private set; } = default!;
        public bool IsActive { get; private set; } = true;

        private readonly List<WorkflowStep> _steps = new();
        public IReadOnlyList<WorkflowStep> Steps => _steps.OrderBy(s => s.Order).ToList();

        public static Workflow Create(string name)
        {
            return new Workflow
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                IsActive = true
            };
        }

        public Result AddStep(int order, Role approverRole, string label)
        {
            if (_steps.Any(s => s.Order == order))
                return Result.Fail("workflow.step.duplicateOrder", "Step order already exists.");

            _steps.Add(new WorkflowStep(Id, order, approverRole, label));
            return Result.Ok();
        }

        public Result RemoveStep(int order)
        {
            var step = _steps.FirstOrDefault(s => s.Order == order);
            if (step is null) return Result.Fail("workflow.step.notFound", "Step not found.");
            _steps.Remove(step);
            return Result.Ok();
        }

        public void Rename(string name) => Name = name.Trim();
        public void Deactivate() => IsActive = false;
    }

    public sealed class WorkflowStep
    {
        private WorkflowStep() { }
        public WorkflowStep(Guid workflowId, int order, Role approverRole, string label)
        {
            Id = Guid.NewGuid();
            WorkflowId = workflowId;
            Order = order;
            ApproverRole = approverRole;
            Label = label.Trim();
        }

        public Guid Id { get; private set; }
        public Guid WorkflowId { get; private set; }
        public int Order { get; private set; }
        public Role ApproverRole { get; private set; }
        public string Label { get; private set; } = default!;
    }
}
