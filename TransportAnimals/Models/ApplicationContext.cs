using Microsoft.EntityFrameworkCore;
using TransportAnimals.ViewModels.Request;

namespace TransportAnimals.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<LocationPoint> Locations { get; set; }
        public DbSet<LocationPoint> Locations2 { get; set; }
        public DbSet<AnimalType> AnimalTypes { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<AnimalVisitedLocation> AnimalVisitedLocations { get; set; }
        public ApplicationContext() : base()
        {
            Database.EnsureCreated();
        }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(acc => acc.Id);
            modelBuilder.Entity<Account>().HasIndex(acc => acc.Email).IsUnique();

            modelBuilder.Entity<LocationPoint>().HasKey(p => p.Id);

            modelBuilder.Entity<AnimalType>().HasKey(t => t.Id);

            modelBuilder.Entity<Area>().HasKey(t => t.Id);
            modelBuilder.Entity<LocationPoint>().HasMany(v => v.Areas).WithMany(a => a.AreaPoints);
            modelBuilder.Entity<AnimalVisitedLocation>().HasKey(v => v.Id);

            modelBuilder.Entity<Animal>().HasKey(a => a.Id);
            modelBuilder.Entity<Animal>().HasOne(v => v.ChippingLocation).WithMany(a => a.Animals).HasForeignKey(v => v.ChippingLocationId);
            modelBuilder.Entity<Animal>().HasOne(v => v.Chipper).WithMany(a => a.Animals).HasForeignKey(v => v.ChipperId);
            modelBuilder.Entity<Animal>().HasMany(v => v.VisitedLocations).WithMany(a => a.Animals).UsingEntity<AnimalAnimalVisitedLocation>(
                j => j
                .HasOne(pt => pt.VisitedLocation)
                .WithMany(t => t.AnimalVisitedLocations)
                .HasForeignKey(pt => pt.AnimalId),
                j => j
                .HasOne(pt => pt.Animal)
                .WithMany(p => p.AnimalVisitedLocations)
                .HasForeignKey(pt => pt.VisitedLocationId),
                j =>
                {
                    j.HasKey(c => c.Id);
                });
        }
    }
}
