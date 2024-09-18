
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoAPI.Models;

namespace TodoAPI.AppDataContext
{

    public class TodoDbContext : DbContext
    {
        private readonly DbSettings _dbsettings;

        public TodoDbContext(DbContextOptions<TodoDbContext> options, IOptions<DbSettings> dbsettings) : base(options)
        {
            _dbsettings = dbsettings.Value;
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_dbsettings.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .ToTable("TodoAPI")
                .HasKey(x => x.id);
        }
    }

}