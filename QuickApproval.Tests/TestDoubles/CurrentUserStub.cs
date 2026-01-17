using Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickApproval.Tests.TestDoubles
{
    public sealed class CurrentUserStub : ICurrentUser
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = "test@local";
        public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

        public bool IsInRole(string role)
            => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
