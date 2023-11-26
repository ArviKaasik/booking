using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Booking.Server.Entities
{
    public class RoomReservationEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [ForeignKey(nameof(CustomerEntity))]
        [Required]
        public long ReserverIdCode { get; set; }

        [Required]
        public CustomerEntity Customer { get; set; }

        [Required]
        public DateTime ReservationStartDate { get; set; }

        [Required]
        public DateTime ReservationEndDate { get; set; }

        [ForeignKey(nameof(HotelRoomEntity))]
        [Required]
        public int HotelRoomId { get; set; }

        [JsonIgnore]
        public HotelRoomEntity HotelRoom { get; set; }
    }
}
