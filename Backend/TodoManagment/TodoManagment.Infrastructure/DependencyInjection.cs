using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoManagment.Core.Domain.RepositoryContract;
using TodoManagment.Infrastructure.Data;
using TodoManagment.Infrastructure.Repositories;

namespace TodoManagment.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddDbContext<TodoDbContext>(options =>
            {
                string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
                string connectionString = connectionStringTemplate
                    .Replace("$MSSQL_SERVER", Environment.GetEnvironmentVariable("MSSQL_SERVER"))
                    .Replace("$MSSQL_DB", Environment.GetEnvironmentVariable("MSSQL_DB"))
                    .Replace("$MSSQL_PASSWORD", Environment.GetEnvironmentVariable("MSSQL_PASSWORD"));

                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
