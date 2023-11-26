using Booking.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Server.DbContexts
{
    public class RoomContext : DbContext
    {
        public DbSet<HotelRoomEntity> Rooms { get; set; }
        public DbSet<RoomReservationEntity> Reservations { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }

        protected readonly IConfiguration Configuration;

        public RoomContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HotelRoomEntity>()
                .HasMany(e => e.Reservations)
                .WithOne(e => e.HotelRoom)
                .HasForeignKey(e => e.HotelRoomId)
                .IsRequired();

            modelBuilder.Entity<CustomerEntity>()
                .HasMany(e => e.Reservations)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.ReserverIdCode)
                .IsRequired();
                
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("BookingDatabase"));
        }
    }
}
