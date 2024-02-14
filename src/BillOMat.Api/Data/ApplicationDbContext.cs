using BillOMat.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillOMat.Api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Institute> Institutes { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, FirstName = "Josef", LastName = "Fürlinger", Nickname = "Joe", Email = "josef.fuerlinger@gmail.com" },
                new Patient { Id = 2, FirstName = "Daniela", LastName = "Fürlinger", Nickname = "Dani", Email = "daniela.fuerlingerther@gmail.com" },
                new Patient { Id = 3, FirstName = "Lea Marie", LastName = "Fürlinger", Nickname = "Lea", Email = "lea.fuerlinger@gmail.com" },
                new Patient { Id = 4, FirstName = "Finja Sophie", LastName = "Fürlinger", Nickname = "Finja", Email = "finja.fuerlinger@gmail.com" });

            modelBuilder.Entity<Institute>().HasData(
                new Institute { Id = 1, Name = "Dr. Melanie Eichberger", AddressLine1 = "Linzerstraße 44", City = "Marchtrenk", Postalcode = "4614" });

            modelBuilder.Entity<Invoice>()
                .Property(invoice => invoice.Amount).HasColumnType("decimal(19,4)");
        }
    }
}
