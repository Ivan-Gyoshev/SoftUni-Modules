namespace P01_HospitalDatabase.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_HospitalDatabase.Data.Config;
    using P01_HospitalDatabase.Data.Models;

    public class HospitalContext : DbContext
    {
        public HospitalContext()
        {

        }

        public HospitalContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Visitation> Visitations { get; set; }
        public DbSet<Diagnose> Diagnoses { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<PatientMedicament> PatientMedicaments { get; set; }

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
            modelBuilder.Entity<Patient>(e =>
            {
                e.Property(e => e.FirstName)
                    .IsUnicode();

                e.Property(e => e.LastName)
                    .IsUnicode();

                e.Property(e => e.Address)
                    .IsUnicode();

                e.Property(e => e.Email)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<Visitation>(e =>
            {
                e.Property(e => e.Comments)
                .IsUnicode();
            });
            modelBuilder.Entity<Diagnose>(e =>
            {
                e.Property(e => e.Name)
                  .IsUnicode();

                e.Property(e => e.Comments)
                .IsUnicode();
            });
            modelBuilder.Entity<Medicament>(e =>
            {
                e.Property(e => e.Name)
                .IsUnicode();
            });
            modelBuilder.Entity<PatientMedicament>(e =>
            {
                e.HasKey(pm => new { pm.PatientId, pm.MedicamentId });

                e.HasOne(pm => pm.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(pm => pm.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(pm => pm.Medicament)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(pm => pm.MedicamentId)
                .OnDelete(DeleteBehavior.Restrict);


            });
            modelBuilder.Entity<Doctor>(e =>
            {
                e.Property(e => e.Name)
                .IsUnicode();

                e.Property(e => e.Specialty)
               .IsUnicode();
            });
        }
    }
}
