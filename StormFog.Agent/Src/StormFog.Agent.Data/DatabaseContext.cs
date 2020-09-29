using Microsoft.EntityFrameworkCore;
using StormFog.Agent.Data.Configurations;
using StormFog.Agent.Entity;

namespace StormFog.Agent.Data
{
    public class DatabaseContext : DbContext
    {
        /// <summary>  
        /// Initializes a new instance of the <see cref="DatabaseContext"/> class.  
        /// </summary>  
        /// <param name="option">Database context options.</param>  
        public DatabaseContext(DbContextOptions option) : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ParameterConfiguration());
            modelBuilder.ApplyConfiguration(new JobConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<SFParameter> Configurations { get; set; }
        public DbSet<SFJob> Jobs { get; set; }
    }
}
