using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Messaging
{
    public static class IdentityHelpers
    {
        public static Task EnableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: true);
        public static Task DisableIdentityInsert<T>(this DbContext context) => SetIdentityInsert<T>(context, enable: false);

        private static Task SetIdentityInsert<T>(DbContext context, bool enable)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";
            return context.Database.ExecuteSqlRawAsync(
                $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
        }

        public static async Task SaveChangesWithIdentityInsertAsync(this DbContext context)
        {
            await using var transaction = await context.Database.BeginTransactionAsync();
            var entityTypes = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity.GetType())
                .Distinct()
                .ToList();

            foreach (var type in entityTypes)
            {
                var enableMethod = typeof(IdentityHelpers)
                    .GetMethod(nameof(EnableIdentityInsert))?
                    .MakeGenericMethod(type);

                if (enableMethod is not null)
                {
                    var enableTask = (Task?)enableMethod.Invoke(null, new object[] { context });
                    if (enableTask is not null)
                        await enableTask.ConfigureAwait(false);
                }

                var entries = context.ChangeTracker.Entries()
                    .Where(e => e.Entity.GetType() == type && (e.State == EntityState.Added || e.State == EntityState.Modified))
                    .ToList();

                if (entries.Count > 0)
                {
                    var otherEntries = context.ChangeTracker.Entries()
                        .Where(e => e.Entity.GetType() != type && (e.State == EntityState.Added || e.State == EntityState.Modified))
                        .Select(e => (e, e.State)).ToList();

                    foreach (var entryTup in otherEntries)
                    {
                        var (entry, _) = entryTup;
                        entry.State = EntityState.Detached;
                    }

                    await context.SaveChangesAsync();

                    foreach (var entryTup in otherEntries)
                    {
                        var (entry, state) = entryTup;
                        context.Attach(entry.Entity);
                        entry.State = state;
                    }
                }

                var disableMethod = typeof(IdentityHelpers)
                    .GetMethod(nameof(DisableIdentityInsert))?
                    .MakeGenericMethod(type);

                if (disableMethod is not null)
                {
                    var disableTask = (Task?)disableMethod.Invoke(null, new object[] { context });
                    if (disableTask is not null)
                        await disableTask.ConfigureAwait(false);
                }
            }

            await transaction.CommitAsync();
        }

    }
}
