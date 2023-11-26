using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Models.Responses
{
    public class BookedRoomsResponse
    {
        public List<Reservation> BookedRooms { get; set; } = new List<Reservation>();
    }

    public class Reservation
    {
        public int Id { get; set; }

        public int HotelRoomId { get; set; }

        public int RoomNumber { get; set; }

        public int SleepSpotCount { get; set; }

        public decimal NightlyPrice { get; set; }

        public long ReserverIdCode { get; set; }

        public DateTime ReservationStartDate { get; set; }

        public DateTime ReservationEndDate { get; set; }
    }
}
