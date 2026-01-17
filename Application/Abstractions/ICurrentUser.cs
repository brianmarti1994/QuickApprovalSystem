using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Email { get; }
        IReadOnlyCollection<string> Roles { get; }
        bool IsInRole(string role);
    }
}
