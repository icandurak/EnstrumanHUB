using InstrumentHUB.Models;
using Microsoft.EntityFrameworkCore;

namespace InstrumentHUB.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Instrument> Instruments { get; set; }
        public DbSet<Guitar> Guitars { get; set; }
        public DbSet<Drum> Drums { get; set; }
        public DbSet<BassGuitar> BassGuitars { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instrument>()
                .HasDiscriminator<string>("InstrumentType")
                .HasValue<Guitar>("Guitar")
                .HasValue<Drum>("Drum")
                .HasValue<BassGuitar>("BassGuitar");

        }
    }

}
