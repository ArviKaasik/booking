using Booking.Server.Entities;
using Booking.Server.Models.Requests;
using Booking.Server.Models.Responses;

namespace Booking.Server.Services
{
    public interface IBookingService
    {
        public IEnumerable<HotelRoomEntity> GetFreeRooms(GetFreeRoomsRequest request);
        public Task<int> BookRoom(BookRoomRequest request);
        public Task<bool> CancelRoom(int reservationId, bool adminRequest = false);
        public BookedRoomsResponse GetBooked(long idNumber);
        public BookedRoomsResponse GetReservations();

        public Task<bool> UpdateReservation(UpdateReservationRequest request);
    }
}
