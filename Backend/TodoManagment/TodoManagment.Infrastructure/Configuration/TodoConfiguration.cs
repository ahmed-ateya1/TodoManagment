using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoManagment.Core.Domain.Entities;

namespace TodoManagment.Infrastructure.Configuration
{
    public class TodoConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier");


            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);
            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            builder.Property(t => t.DueDate)
                .HasColumnType("datetime");

            builder.Property(t => t.CreatedDate)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            builder.Property(t => t.LastModifiedDate)
                .IsRequired()
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETDATE()");

            builder.HasIndex(t => t.Title)
                .HasDatabaseName("IX_Todo_Title");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Todo_Status");

            builder.HasIndex(t => t.Priority)
                .HasDatabaseName("IX_Todo_Priority");

            builder.ToTable("Todos");
        }
    }
}
