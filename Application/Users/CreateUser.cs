using Application.Abstractions;
using Domain.Common;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public sealed record CreateUserCommand(string Email, string DisplayName, string Password, Role[] Roles)
    : IRequest<Result<Guid>>;

    public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IAppDbContext _db;

        public CreateUserHandler(IAppDbContext db) => _db = db;

        public async Task<Result<Guid>> Handle(CreateUserCommand cmd, CancellationToken ct)
        {
            var email = cmd.Email.Trim().ToLowerInvariant();
            if (await _db.Users.AnyAsync(u => u.Email == email, ct))
                return Result<Guid>.Fail("user.duplicate", "Email already exists.");

            var hash = BCrypt.Net.BCrypt.HashPassword(cmd.Password);

            var user = User.Create(email, cmd.DisplayName, hash);
            foreach (var role in cmd.Roles.Distinct())
                user.AssignRole(role);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return Result<Guid>.Ok(user.Id);
        }
    }
}
