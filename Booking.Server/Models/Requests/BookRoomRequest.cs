using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Models.Requests
{
    public class BookRoomRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int RoomId { get; set; }

        //TODO actual Id number validation
        [Required]
        [Range(10000000000, 99999999999)]
        public long IdNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
