using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Requests
{
    public enum RequestStatus
    {
        Draft = 1,
        Submitted = 2,
        PendingApproval = 3,
        Approved = 4,
        Rejected = 5
    }
}
