using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Infrastructure.DbContext;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(TaskDbContext context, ILogger<TaskRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Create(TaskDomainModel task, CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(TaskDomainModel task, CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TaskDomainModel?> Get(int id, CancellationToken cancellationToken)
    {
        return await _context.Tasks.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<TaskDomainModel?> GetByName(string name, CancellationToken cancellationToken)
    {
        return await _context.Tasks.FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }

    public async Task<TaskDomainModel?> GetByPhone(string phone, CancellationToken cancellationToken)
    {
        return await _context.Tasks.FirstOrDefaultAsync(a => a.Phone == phone, cancellationToken);
    }
}