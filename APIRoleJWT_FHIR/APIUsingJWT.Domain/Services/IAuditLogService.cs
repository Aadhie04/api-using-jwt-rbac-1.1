using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, int entityId, string entity, string performedBy, string role);
    }
}
