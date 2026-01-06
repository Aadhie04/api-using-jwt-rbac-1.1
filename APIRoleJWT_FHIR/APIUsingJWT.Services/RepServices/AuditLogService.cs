using APIUsingJWT.Domain.Models;
using APIUsingJWT.Domain.Repositories;
using APIUsingJWT.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Services.RepServices
{
    public class AuditLogService: IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository auditLogRepository, ILogger<AuditLogService> logger)
        {
            this._auditLogRepository = auditLogRepository;
            this._logger = logger;
        }
        public async Task LogAsync(string action, int entityId, string entity, string performedBy, string role)
        {
            _logger.LogInformation("Audit: {Action} on {Entity}({Id}) by {User} ({Role})",
            action, entity, entityId, performedBy, role);
            await _auditLogRepository.AddAsync(new AuditLogModel
            {
                Action = action,
                EntityName = entity,
                EntityId = entityId,
                PerformedBy = performedBy,
                Role = role
            });
        }
    }
}
