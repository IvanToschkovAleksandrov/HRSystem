using HRSystem.Data.Configurations;
using HRSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Data
{
    public class HRSystemDbContext : IdentityDbContext
    {
        public HRSystemDbContext(DbContextOptions<HRSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<House> Houses { get; init; } = null!;
        public DbSet<Agent> Agents { get; init; } = null!;
        public DbSet<Category> Categories { get; init; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new AgentConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new HouseConfiguration());

            builder.Entity<House>()
                .Property("PricePerMonth")
                .HasColumnType("decimal(6,2)");

            base.OnModelCreating(builder);
        }
    }
}