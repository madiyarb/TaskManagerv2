using Microsoft.EntityFrameworkCore.Storage;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Infrastructure.Interfaces;
using TaskManager.Infrastructure.Persistance;
using ITaskUnitOfWork = TaskManager.Application.Interfaces.ITaskUnitOfWork;

namespace TaskManager.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

public class TaskDbContext : DbContext, ITaskUnitOfWork, ITaskUnitOfWorkInfr
{
    public DbSet<TaskDomainModel> Tasks { get; set; }
    
    
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TaskDbMap());
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }
}