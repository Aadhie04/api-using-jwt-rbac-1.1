using APIUsingJWT.Data.DbEntities;
using APIUsingJWT.Data.Entities;
using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Data.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApiDbContext _context;

        public AuditLogRepository(ApiDbContext context)
        {
            this._context = context;
        }
        public async Task AddAsync(AuditLogModel model)
        {
            var entity = new AuditLog
            {
                Action = model.Action,
                EntityName = model.EntityName,
                EntityId = model.EntityId,
                PerformedBy = model.PerformedBy,
                Role = model.Role

            };
            _context.AuditLogs.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
