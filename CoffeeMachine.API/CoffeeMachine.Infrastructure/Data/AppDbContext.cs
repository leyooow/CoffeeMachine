using CoffeeMachine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeMachine.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CoffeeMachineState> CoffeeMachineStates => Set<CoffeeMachineState>();
    public DbSet<MachineLog> MachineLogs => Set<MachineLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoffeeMachineState>(entity =>
        {
            entity.ToTable("CoffeeMachineStates");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedNever();
            entity.Property(x => x.BrewCount).IsRequired();
            entity.Property(x => x.CreatedDate).IsRequired();
            entity.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<MachineLog>(entity =>
        {
            entity.ToTable("MachineLogs");
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.Endpoint).IsRequired().HasMaxLength(50);
            entity.Property(x => x.StatusCode).IsRequired();
            entity.Property(x => x.Message).IsRequired().HasMaxLength(500);
            entity.Property(x => x.CreatedDate).IsRequired();
            entity.HasIndex(x => x.CreatedDate);
            entity.Property(x => x.CreatedBy).IsRequired().HasMaxLength(100);
        });
    }
}
