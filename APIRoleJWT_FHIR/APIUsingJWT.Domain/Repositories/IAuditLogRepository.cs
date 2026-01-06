using APIUsingJWT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Domain.Repositories
{
    public interface IAuditLogRepository
    {
        //actually Audit log returns anything
        Task AddAsync(AuditLogModel model);
    }
}
