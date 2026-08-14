using System.ComponentModel.DataAnnotations;

namespace BookingGrid.MainService.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new booking request.
    /// </summary>
    public class CreateBookingDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Number of rooms must be greater than 0.")]
        public int NumberOfRooms { get; set; } = 1;
    }
}