namespace Booking.Server.Exceptions
{
    public class ReservationException : Exception
    {
        public ReservationException(string message) : base (message)
        {
        }
    }
}
