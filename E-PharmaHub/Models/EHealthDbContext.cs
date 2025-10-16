using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Models
{
    public class EHealthDbContext : IdentityDbContext<AppUser>
    {
        public EHealthDbContext(DbContextOptions<EHealthDbContext> opts) : base(opts) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<DonorProfile> DonorProfiles { get; set; }
        public DbSet<DoctorProfile> DoctorProfiles { get; set; }
        public DbSet<PharmacistProfile> Pharmacists { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<FavoriteMedication> FavoriteMedications { get; set; }
        public DbSet<FavoriteClinic> FavoriteClinics { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<DonorMatch> DonorMatches { get; set; }
        public DbSet<MessageThread> MessageThreads { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AlternativeMedication> AlternativeMedications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                        .HasOne(r => r.User)
                        .WithMany()
                        .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Medication>()
                .HasIndex(m => new { m.GenericName, m.Strength });

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Pharmacy).WithMany(p => p.Inventory)
                .HasForeignKey(i => i.PharmacyId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InventoryItem>()
                .HasOne(i => i.Medication).WithMany(m => m.Inventories)
                .HasForeignKey(i => i.MedicationId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order).WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Medication).WithMany()
                .HasForeignKey(oi => oi.MedicationId);

            modelBuilder.Entity<PrescriptionItem>()
                .HasOne(pi => pi.Prescription).WithMany(p => p.Items)
                .HasForeignKey(pi => pi.PrescriptionId);

            modelBuilder.Entity<MessageThreadParticipant>()
                .HasIndex(m => new { m.ThreadId, m.UserId }).IsUnique();

            modelBuilder.Entity<AlternativeMedication>()
                .HasOne(am => am.Medication).WithMany(m => m.Alternatives)
                .HasForeignKey(am => am.MedicationId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlternativeMedication>()
                .HasOne(am => am.Alternative).WithMany()
                .HasForeignKey(am => am.AlternativeId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DonorProfile>()
                 .HasOne(d => d.AppUser)
                 .WithOne(u => u.DonorProfile)
                 .HasForeignKey<DonorProfile>(d => d.AppUserId);


            modelBuilder.Entity<Address>()
                .HasIndex(a => new { a.City });

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.PatientAppointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(u => u.DoctorAppointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
