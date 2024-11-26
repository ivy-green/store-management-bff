using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace ProjectBase.Insfracstructure.Data
{
    [ExcludeFromCodeCoverage]
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContext>
    {
        AppDBContext IDesignTimeDbContextFactory<AppDBContext>.CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("connectionStrings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AppDBContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString).UseLazyLoadingProxies(); 

            return new AppDBContext(builder.Options);
        }
    }
}
