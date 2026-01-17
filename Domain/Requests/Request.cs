using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Requests
{
    public sealed class Request : AggregateRoot<Guid>
    {
        private Request() { }

        public Guid CreatedByUserId { get; private set; }
        public Guid WorkflowId { get; private set; }

        public string Title { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public decimal Amount { get; private set; }

        public RequestStatus Status { get; private set; } = RequestStatus.Draft;

        public int CurrentStepOrder { get; private set; } = 1;

        private readonly List<ApprovalDecision> _decisions = new();
        public IReadOnlyCollection<ApprovalDecision> Decisions => _decisions;

        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? SubmittedAtUtc { get; private set; }
        public DateTime? CompletedAtUtc { get; private set; }

        public static Request Create(Guid createdByUserId, Guid workflowId, string title, string description, decimal amount)
        {
            return new Request
            {
                Id = Guid.NewGuid(),
                CreatedByUserId = createdByUserId,
                WorkflowId = workflowId,
                Title = title.Trim(),
                Description = description.Trim(),
                Amount = amount,
                Status = RequestStatus.Draft,
                CurrentStepOrder = 1,
                CreatedAtUtc = DateTime.UtcNow
            };
        }

        public Result Submit()
        {
            if (Status != RequestStatus.Draft)
                return Result.Fail("request.invalidState", "Only draft requests can be submitted.");

            if (string.IsNullOrWhiteSpace(Title))
                return Result.Fail("request.validation.title", "Title is required.");

            if (Amount <= 0)
                return Result.Fail("request.validation.amount", "Amount must be > 0.");

            Status = RequestStatus.Submitted;
            SubmittedAtUtc = DateTime.UtcNow;
            Status = RequestStatus.PendingApproval;

            return Result.Ok();
        }

        public Result Approve(Guid actorUserId, string? comment = null)
        {
            if (Status != RequestStatus.PendingApproval)
                return Result.Fail("request.invalidState", "Request is not pending approval.");

            _decisions.Add(ApprovalDecision.Approve(Id, CurrentStepOrder, actorUserId, comment));
            CurrentStepOrder += 1; // application will check if there is next step
            return Result.Ok();
        }

        public Result Reject(Guid actorUserId, string reason)
        {
            if (Status != RequestStatus.PendingApproval)
                return Result.Fail("request.invalidState", "Request is not pending approval.");

            if (string.IsNullOrWhiteSpace(reason))
                return Result.Fail("request.validation.rejectReason", "Reject reason is required.");

            _decisions.Add(ApprovalDecision.Reject(Id, CurrentStepOrder, actorUserId, reason));
            Status = RequestStatus.Rejected;
            CompletedAtUtc = DateTime.UtcNow;
            return Result.Ok();
        }

        public void MarkApproved()
        {
            Status = RequestStatus.Approved;
            CompletedAtUtc = DateTime.UtcNow;
        }
    }

    public sealed class ApprovalDecision
    {
        private ApprovalDecision() { }

        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public int StepOrder { get; private set; }
        public Guid ActorUserId { get; private set; }
        public bool IsApproved { get; private set; }
        public string Comment { get; private set; } = string.Empty;
        public DateTime AtUtc { get; private set; }

        public static ApprovalDecision Approve(Guid requestId, int stepOrder, Guid actorUserId, string? comment)
            => new()
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                StepOrder = stepOrder,
                ActorUserId = actorUserId,
                IsApproved = true,
                Comment = comment?.Trim() ?? string.Empty,
                AtUtc = DateTime.UtcNow
            };

        public static ApprovalDecision Reject(Guid requestId, int stepOrder, Guid actorUserId, string reason)
            => new()
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                StepOrder = stepOrder,
                ActorUserId = actorUserId,
                IsApproved = false,
                Comment = reason.Trim(),
                AtUtc = DateTime.UtcNow
            };
    }
}
