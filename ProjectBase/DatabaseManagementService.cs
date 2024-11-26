using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Insfracstructure.Data;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase
{
    [ExcludeFromCodeCoverage]
    public static class DatabaseManagementService
    {
        // Getting the scope of our database context
        public static void MigrationInitialisation(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var _Db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                if (_Db != null)
                {
                    if (_Db.Database.GetPendingMigrations().Any())
                    {
                        _Db.Database.Migrate();
                    }
                }
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;
        }
    }
}
