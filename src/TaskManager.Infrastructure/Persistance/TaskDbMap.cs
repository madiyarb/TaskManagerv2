using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.DomainModels;

namespace TaskManager.Infrastructure.Persistance;

public class TaskDbMap : IEntityTypeConfiguration<TaskDomainModel>
{
    public void Configure(EntityTypeBuilder<TaskDomainModel> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(jt => jt.Id);
        builder.Property(jt => jt.Id).HasColumnName("Id")
            .ValueGeneratedOnAdd();
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Phone)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasIndex(x => x.Phone)
            .IsUnique(true);
    }
}