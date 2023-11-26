using Booking.Server.DbContexts;
using Booking.Server.Entities;
using Booking.Server.Exceptions;
using Booking.Server.Models.Requests;
using Booking.Server.Models.Responses;

namespace Booking.Server.Services
{
    public class BookingService : IBookingService
    {
        private readonly RoomContext _roomContext;

        public BookingService(RoomContext roomContext)
        {
            _roomContext = roomContext;
        }

        public IEnumerable<HotelRoomEntity> GetFreeRooms(GetFreeRoomsRequest request)
        {
            if (request.StartDate.Date < DateTime.UtcNow.Date)
                throw new Exception("Not possible to book rooms for past!");

            var free_rooms = _roomContext.Rooms.Where(
                room =>
                    room.Reservations.Count() == 0 || !room.Reservations.Any(
                        r => r.ReservationStartDate.Date >= request.StartDate.Date && r.ReservationEndDate.Date <= request.EndDate.Date))
                .ToList();
            var rooms = _roomContext.Rooms.OrderBy(r => r.Id)
                .FirstOrDefault();

            var reservations = _roomContext.Reservations.OrderBy(r => r.Id).FirstOrDefault();

            foreach (var room in free_rooms)
                Console.WriteLine($"Free room: {room.RoomNumber}");

            return free_rooms;
        }

        public async Task<int> BookRoom(BookRoomRequest request)
        {
            ValidateBookRoomRequest(request);

            var existingReservation = _roomContext.Reservations
                .FirstOrDefault(r => r.HotelRoomId == request.RoomId && r.ReservationStartDate.Date >= request.StartDate.Date && r.ReservationEndDate.Date <= request.EndDate.Date);

            if (existingReservation != null)
                throw new ReservationException("There already exists an active reservation for the room for selected time period!");

            var existingUser = _roomContext.Customers.FirstOrDefault(c => c.IdCode == request.IdNumber);
            if (existingUser == null)
                await _roomContext.Customers.AddAsync(
                    new CustomerEntity
                    {
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        IdCode = request.IdNumber
                    });
            await _roomContext.SaveChangesAsync();

            var users = _roomContext.Customers.OrderByDescending(e => e.IdCode);
            await _roomContext.Reservations.AddAsync(
                new RoomReservationEntity { 
                    HotelRoomId = request.RoomId, 
                    ReservationEndDate = request.EndDate, 
                    ReservationStartDate = request.StartDate, 
                    ReserverIdCode = request.IdNumber
                });
            await _roomContext.SaveChangesAsync();

            var addedReservation = _roomContext.Reservations
                .FirstOrDefault(r => r.HotelRoomId == request.RoomId && r.ReservationStartDate.Date >= request.StartDate.Date && r.ReservationEndDate.Date <= request.EndDate.Date);

            if (addedReservation == null)
                throw new ReservationException("Booking the room failed!");

            return addedReservation.Id;
        }

        public async Task<bool> CancelRoom(int reservationId, bool adminRequest = false)
        {
            var reservation = _roomContext.Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                throw new ReservationException("Reservation was not found!");

            if (!adminRequest && reservation.ReservationStartDate.Date <= DateTime.UtcNow.AddDays(3).Date)
                throw new ReservationException("It's not possible to cancel reservation less than 3 days before reservation");

            _roomContext.Reservations.Remove(reservation);
            await _roomContext.SaveChangesAsync();

            return true;
        }

        public BookedRoomsResponse GetBooked(long idNumber)
        {
            var bookedRooms = _roomContext.Rooms.Where(
                r => r.Reservations.Any(
                    reservation =>
                    reservation.ReserverIdCode == idNumber
                    && reservation.ReservationEndDate.Date >= DateTime.UtcNow.Date));
            foreach (var room in bookedRooms)
                room.Reservations = _roomContext.Reservations.Where(r => r.ReserverIdCode == idNumber && r.HotelRoomId == room.Id).ToList();

            var reservations = new List<Reservation>();
            foreach(var room in bookedRooms)
                foreach(var reservation in room.Reservations)
                {
                    reservation.HotelRoom = room;
                    reservations.Add(MapReservation(reservation));
                }

            return new BookedRoomsResponse { BookedRooms = reservations };
        }

        public BookedRoomsResponse GetReservations()
        {
            var reservationEntities = _roomContext.Reservations.ToList();
            var reservations = new List<Reservation>();

            foreach (var reservation in reservationEntities)
            {
                reservation.HotelRoom = _roomContext.Rooms.First(r => r.Id == reservation.HotelRoomId);
                reservations.Add(MapReservation(reservation));
            }

            return new BookedRoomsResponse { BookedRooms = reservations };
        }

        public async Task<bool> UpdateReservation(UpdateReservationRequest request)
        {
            var reservation = _roomContext.Reservations.FirstOrDefault(r => r.Id == request.ReservationId);

            if (reservation == null)
                throw new ReservationException($"Reservation with Id {request.ReservationId} was not found!");

            if (request.StartDate.HasValue)
                reservation.ReservationStartDate = request.StartDate.Value.Date;

            if (request.EndDate.HasValue)
                reservation.ReservationEndDate = request.EndDate.Value.Date;

            if (reservation.ReservationStartDate >= reservation.ReservationEndDate)
                throw new ReservationException($"Reservation Check-in must be before Checkout!");

            await _roomContext.SaveChangesAsync();

            return true;
        }

        //TODO replace with automapper
        private Reservation MapReservation(RoomReservationEntity reservation)
        {
            return new Reservation
            {
                HotelRoomId = reservation.HotelRoomId,
                ReservationStartDate = reservation.ReservationStartDate,
                ReservationEndDate = reservation.ReservationEndDate,
                Id = reservation.Id,
                NightlyPrice = reservation.HotelRoom.NightlyPrice,
                ReserverIdCode = reservation.ReserverIdCode,
                RoomNumber = reservation.HotelRoom.RoomNumber,
                SleepSpotCount = reservation.HotelRoom.SleepSpotCount
            };
        }

        //TODO: Consider using Mediatr to set up query validation pipeline
        private void ValidateBookRoomRequest(BookRoomRequest request)
        {
            if (request.StartDate.Date >= request.EndDate.Date)
                throw new ReservationException("Reservation check-in must be before Reservation check-out");
        }

    }
}
