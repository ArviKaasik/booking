using Booking.Server.Exceptions;
using Booking.Server.Models.Requests;
using Booking.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Controllers
{
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;

        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("booking/booked/{idNumber}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetBooked([Required][FromRoute] long idNumber)
        {
            try
            {
                var result = _bookingService.GetBooked(idNumber);
                return Ok(result);
            }
            catch (ReservationException re)
            {
                return BadRequest(re.Message);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost]
        [Route("booking/free")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetFree([Required][FromBody] GetFreeRoomsRequest request)
        {
            try
            {
                var result = _bookingService.GetFreeRooms(request);
                return Ok(result);
            }
            catch (ReservationException re)
            {
                return BadRequest(re.Message);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost]
        [Route("booking/book")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BookRoom([Required][FromBody] BookRoomRequest request)
        {
            try
            {
                var result = await _bookingService.BookRoom(request);
                return Ok(result);
            }
            catch (ReservationException re)
            {
                return BadRequest(re.Message);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost]
        [Route("booking/cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CancelRoom([Required][FromBody] BookingCancelRequest request)
        {
            try
            {
                var result = await _bookingService.CancelRoom(request.ReservationId);
                return Ok(result);
            }
            catch (ReservationException re)
            {
                return BadRequest(re.Message);
            }
            catch (Exception ex) {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}
