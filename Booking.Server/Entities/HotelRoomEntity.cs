namespace Booking.Server.Entities
{
    public class HotelRoomEntity
    {
        public int Id { get; set; }

        public int RoomNumber { get; set; }

        public int SleepSpotCount { get; set; }

        public decimal NightlyPrice { get; set; }

        public IEnumerable<RoomReservationEntity> Reservations { get; set; } = new List<RoomReservationEntity>();
    }
}
