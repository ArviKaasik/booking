using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Entities
{
    public class CustomerEntity
    {
        [Key]
        [Required]
        public long IdCode { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public IEnumerable<RoomReservationEntity> Reservations { get; set; } = new List<RoomReservationEntity>();
    }
}
