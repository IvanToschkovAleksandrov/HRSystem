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
            base.OnModelCreating(builder);
        }
    }
}