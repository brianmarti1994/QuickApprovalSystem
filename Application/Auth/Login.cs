using Application.Abstractions;
using BCrypt.Net;
using Domain.Common;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;
    public sealed record LoginResponse(string Token, string Email, string DisplayName, Role[] Roles);

    public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
    {
        private readonly IAppDbContext _db;
        private readonly IJwtTokenService _jwt;

        public LoginHandler(IAppDbContext db, IJwtTokenService jwt)
        {
            _db = db; _jwt = jwt;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user is null || !user.IsActive)
                return Result<LoginResponse>.Fail("auth.invalid", "Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Result<LoginResponse>.Fail("auth.invalid", "Invalid credentials.");

            var roles = user.Roles.Select(r => r.Role).Distinct().ToArray();
            var token = _jwt.CreateToken(user.Id, user.Email, roles);

            return Result<LoginResponse>.Ok(new LoginResponse(token, user.Email, user.DisplayName, roles));
        }
    }
}
