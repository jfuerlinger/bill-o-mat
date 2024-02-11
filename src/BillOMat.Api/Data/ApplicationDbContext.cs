using BillOMat.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillOMat.Api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasData(
                new Patient { Id = 1, Firstname = "Josef", Lastname = "Fürlinger", Nickname = "Joe" },
                new Patient { Id = 2, Firstname = "Daniela", Lastname = "Fürlinger", Nickname = "Dani" },
                new Patient { Id = 3, Firstname = "Lea Marie", Lastname = "Fürlinger", Nickname = "Lea" },
                new Patient { Id = 4, Firstname = "Finja Sophie", Lastname = "Fürlinger", Nickname = "Finja" }
                );
        }
    }
}
