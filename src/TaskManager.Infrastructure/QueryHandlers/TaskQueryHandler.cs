using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;

namespace TaskManager.Infrastructure.QueryHandlers;

public class TaskQueryHandler : ITaskQueryHandler
{
    private readonly string _connectionString;
    private readonly ILogger<TaskQueryHandler> _logger;

    public TaskQueryHandler(IConfiguration configuration,
        ILogger<TaskQueryHandler> logger)
    {
        _connectionString = configuration.GetConnectionString("ConnectionString") ??
                            throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskDomainModel?> GetByIdAsync(int id)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            var parameters = new { a = id };
            var result = await db.QueryFirstOrDefaultAsync<TaskDomainModel>("SELECT * FROM dbo.GetTaskById(@a)", parameters);
            _logger.LogInformation("Calling function: dbo.GetTaskById(@a) with parameters: {@Parameters}", parameters);
            return result;
        }
    }
}