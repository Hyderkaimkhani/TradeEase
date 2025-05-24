using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Repositories.Helpers
{
    public class AuditHelper
    {
        public static List<AuditLog> CreateAuditLogs(ChangeTracker changeTracker, Func<string> getCurrentUsername)
        {
            var auditLogs = new List<AuditLog>();

            var auditEntries = changeTracker
                .Entries()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Deleted)
                .Where(e => !(e.Entity is AuditLog))  // prevent self-auditing
                .ToList();

            foreach (var entry in auditEntries)
            {
                if (entry.Entity is UserAuth) // Skip audit for UserAuth
                    continue;

                var tableName = entry.Metadata.Name;  // usually namespace-qualified entity class name
                var primaryKey = entry.Properties.First(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString() ?? "N/A";
                var action = entry.State.ToString();

                var oldValues = new Dictionary<string, object>();
                var newValues = new Dictionary<string, object>();
                var changedColumns = new List<string>();

                foreach (var prop in entry.Properties)
                {
                    if (entry.State == EntityState.Modified && !Equals(prop.OriginalValue, prop.CurrentValue))
                    {
                        changedColumns.Add(prop.Metadata.Name);
                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                        newValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (entry.State == EntityState.Added)
                    {
                        newValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                }

                auditLogs.Add(new AuditLog
                {
                    TableName = tableName,
                    RecordId = primaryKey,
                    Action = action,
                    ChangedColumns = string.Join(",", changedColumns),
                    OldValues = oldValues.Count > 0 ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues.Count > 0 ? JsonSerializer.Serialize(newValues) : null,
                    CreatedBy = getCurrentUsername(),
                    CreatedDate = DateTime.UtcNow
                });
            }

            return auditLogs;
        }
    }
}