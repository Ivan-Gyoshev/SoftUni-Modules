namespace P03_SalesDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P03_SalesDatabase.Data.Configuration;
    using P03_SalesDatabase.Data.Models;

    public class SalesContext : DbContext
    {
        public SalesContext()
        {

        }

        public SalesContext(DbContextOptions options)
            : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Connection.connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(e => e.Name)
                .IsUnicode();

                e.Property(e => e.Description)
                .HasDefaultValue("No description");
            });
            modelBuilder.Entity<Customer>(e =>
            {
                e.Property(e => e.Name)
                .IsUnicode();

                e.Property(e => e.Email)
                .IsUnicode(false);

                e.Property(e => e.CreditCardNumber)
                .IsUnicode(false);
            });
            modelBuilder.Entity<Store>(e =>
            {
                e.Property(e => e.Name)
                .IsUnicode();
            });
            modelBuilder.Entity<Sale>(e =>
            {
                e.Property(e => e.Date)
                .HasDefaultValueSql("GETDATE()");

            });
        }
    }
}
