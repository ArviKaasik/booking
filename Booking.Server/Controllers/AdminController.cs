using Booking.Server.Exceptions;
using Booking.Server.Models.Requests;
using Booking.Server.Models.Responses;
using Booking.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Booking.Server.Controllers
{

    //TODO add endpoint authentication
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;

        public AdminController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("admin/reservations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult GetReservations()
        {
            try
            {
                var result = _bookingService.GetReservations();
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
        [Route("admin/cancel")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CancelReservation([Required][FromBody] BookingCancelRequest request)
        {
            try
            {
                var result = await _bookingService.CancelRoom(request.ReservationId, true);
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
        [Route("admin/update")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateReservation([Required][FromBody] UpdateReservationRequest request)
        {
            try
            {
                var result = await _bookingService.UpdateReservation(request);
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
    }
}
