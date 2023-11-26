using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Models.Requests
{
    public class GetFreeRoomsRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
