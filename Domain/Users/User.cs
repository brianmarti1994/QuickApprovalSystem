using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users
{
    public sealed class User : AggregateRoot<Guid>
    {
        private User() { }

        public string Email { get; private set; } = default!;
        public string DisplayName { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!;
        public bool IsActive { get; private set; } = true;

        private readonly List<UserRole> _roles = new();
        public IReadOnlyCollection<UserRole> Roles => _roles;

        public static User Create(string email, string displayName, string passwordHash)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                Email = email.Trim().ToLowerInvariant(),
                DisplayName = displayName.Trim(),
                PasswordHash = passwordHash,
                IsActive = true
            };
        }

        public void Deactivate() => IsActive = false;

        public void AssignRole(Role role)
        {
            if (_roles.Any(r => r.Role == role)) return;
            _roles.Add(new UserRole(Id, role));
        }

        public void RemoveRole(Role role)
        {
            var existing = _roles.FirstOrDefault(r => r.Role == role);
            if (existing is not null) _roles.Remove(existing);
        }
    }

    public sealed class UserRole
    {
        private UserRole() { }
        public UserRole(Guid userId, Role role) { UserId = userId; Role = role; }

        public Guid UserId { get; private set; }
        public Role Role { get; private set; }
    }
}
