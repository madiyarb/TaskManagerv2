using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TaskManager.Infrastructure.Extensions;

public static class MsSqlDbContextServiceCollectionExtensions
{
    public static IServiceCollection AddMsSqlDbContext<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        return services.AddDbContext<TContext>(
            opts => { opts.UseSqlServer(configuration.GetConnectionString("ConnectionString"))
                .LogTo(Log.Logger.Information, LogLevel.Information); });
    }
}