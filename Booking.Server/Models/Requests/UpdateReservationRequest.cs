using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Models.Requests
{
    public class UpdateReservationRequest
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public int ReservationId { get; set; }
    }
}
