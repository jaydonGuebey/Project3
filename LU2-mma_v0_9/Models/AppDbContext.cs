using Microsoft.EntityFrameworkCore;

namespace LU2_software_testen.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PrescriptionMedicine>()
                .HasKey(pm => new { pm.PrescriptionID, pm.MedicineID });

            modelBuilder.Entity<Medicine>()
                .Property(m => m.ActiveSubstances)
                .HasConversion(
                    v => string.Join(';', v),
                    v => v.Split(';', System.StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        }
    }
}
