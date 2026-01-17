using Application.Abstractions;
using Domain.Common;
using Domain.Requests;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests
{
    public sealed record CreateRequestCommand(string Title, string Description, decimal Amount, Guid WorkflowId)
     : IRequest<Result<Guid>>;

    public sealed class CreateRequestValidator : AbstractValidator<CreateRequestCommand>
    {
        public CreateRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.WorkflowId).NotEmpty();
        }
    }

    public sealed class CreateRequestHandler : IRequestHandler<CreateRequestCommand, Result<Guid>>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentUser _me;

        public CreateRequestHandler(IAppDbContext db, ICurrentUser me)
        {
            _db = db; _me = me;
        }

        public async Task<Result<Guid>> Handle(CreateRequestCommand cmd, CancellationToken ct)
        {
            var wfExists = await _db.Workflows.AnyAsync(w => w.Id == cmd.WorkflowId && w.IsActive, ct);
            if (!wfExists)
                return Result<Guid>.Fail("workflow.notFound", "Workflow not found.");

            var entity = Request.Create(_me.UserId, cmd.WorkflowId, cmd.Title, cmd.Description, cmd.Amount);
            _db.Requests.Add(entity);

            await _db.SaveChangesAsync(ct);
            return Result<Guid>.Ok(entity.Id);
        }
    }
}
