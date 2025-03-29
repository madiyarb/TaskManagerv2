using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.DbContext;
using TaskManager.Infrastructure.Extensions;
using TaskManager.Infrastructure.Interfaces;
using TaskManager.Infrastructure.Processors;
using TaskManager.Infrastructure.QueryHandlers;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Repositories.Decorators;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, WebApplicationBuilder builder)
    {
        #region Task
        services.AddMsSqlDbContext<TaskDbContext>(builder.Configuration);
        services.AddScoped<ITaskQueryHandler, TaskQueryHandler>();
        services.AddScoped<ITaskRepository, TaskRepository>()
            .Decorate<ITaskRepository, TaskCacheDecoratedRepository>()
            .Decorate<ITaskRepository, TaskConcurrencyDecoratedRepository>();
        services.AddScoped<ITaskUnitOfWork>(c => c.GetRequiredService<TaskDbContext>());
        services.AddScoped<ITaskUnitOfWorkInfr>(c => c.GetRequiredService<TaskDbContext>());
        #endregion
        
        #region Cache
        services.AddScoped<ICacheProcessor, CacheProcessor>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<ICacheService, HybridCacheService>();
        #endregion
    }
}