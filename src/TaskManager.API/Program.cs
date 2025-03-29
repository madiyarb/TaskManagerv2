using System.Reflection;
using System.Text.Json;
using Confluent.Kafka;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Sinks.Kafka;
using StackExchange.Redis;
using TaskManager.API.Attributes;
using TaskManager.API.Extensions;
using TaskManager.API.Kafka;
using TaskManager.API.Middlewares;
using TaskManager.Application;
using TaskManager.Application.Abstractions;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Extensions;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.NewtonsoftJson;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;


try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddMemoryCache();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "TaskManager",
            Version = "v1"
        });
        c.OperationFilter<HeaderOperationFilter>();
        c.OperationFilter<HeadersExtensions>();
    });


    #region Mediatr
    // builder.Services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(TaskManager.Application.Extensions.AssemblyMarker))!);
    builder.Services.AddMediatR(Assembly.GetAssembly(typeof(TaskManager.Application.Extensions.AssemblyMarker))!);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingPipelineBehavior<,>));
    #endregion
    
    #region Serilog

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    builder.Host.UseSerilog();

    #endregion

    #region OpenTelemetry

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRedisInstrumentation()
                .AddElasticsearchClientInstrumentation(a => a.SetDbStatementForRequest = true)
                .AddSqlClientInstrumentation(a => a.SetDbStatementForText = true)
                .AddEntityFrameworkCoreInstrumentation(a => a.SetDbStatementForText = true)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("TaskManager"))
                .AddOtlpExporter(options => { options.Endpoint = new Uri("http://localhost:4317"); });
        })
        .WithMetrics(metricsBuilder =>
        {
            metricsBuilder.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddOtlpExporter(options => { options.Endpoint = new Uri("http://localhost:4317"); });
        });

    builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
    }));

    #endregion

    #region MemoryCache

    builder.Services.AddMemoryCache();

    #endregion

    #region Redis

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
        options.InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value;
    });

    #endregion

    #region HybridCache

    FusionCacheOptions opt = new FusionCacheOptions()
    {
        CacheKeyPrefix = "TaskManager"
    };
    builder.Services.AddFusionCache()
        .WithDistributedCache(_ =>
        {
            var options = new RedisCacheOptions
            {
                Configuration = builder.Configuration.GetSection("Redis:Configuration").Value,
                InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value,
            };

            return new RedisCache(options);
        })
        .WithSerializer(new FusionCacheSystemTextJsonSerializer())
        .AsHybridCache();

    #endregion
    
    #region Kafka
    KafkaInitializerHelper.Initialize(builder.Services, builder.Configuration);
    builder.Services.AddScoped<IProduceService, KafkaProduceService>();
    #endregion

    #region Infrastructure

    builder.Services.AddInfrastructure(builder);

    #endregion

    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager"); });
        app.MapScalarApiReference(options => { });
    }

    #region Middlewares
    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<CorrelationLogMiddleware>();
    #endregion
    app.MapControllers();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal("Fatal exception, {exception}", ex);
}