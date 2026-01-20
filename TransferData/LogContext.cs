using Microsoft.EntityFrameworkCore;
using TransferData.Shared;


namespace TransferData.Server
{
    internal class LogContext : DbContext
    { 
        public DbSet<AndroidLog> AndroidLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS01;Database=InternalProject;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AndroidLog>().ToTable("AndroidLog");

            modelBuilder.Entity<AndroidLog>(entity =>
            {
                entity.ToTable("AndroidLog");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                  .UseIdentityColumn()
                  .ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
