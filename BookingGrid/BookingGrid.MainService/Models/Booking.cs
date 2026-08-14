using System.ComponentModel.DataAnnotations;

namespace BookingGrid.MainService.Models
{
    /// <summary>
    /// Represents a booking entity, tracking room reservations, dates, pricing, and cancellation details.
    /// </summary>
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(100)]
        public string HotelName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string RoomType { get; set; } = string.Empty;

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required]
        public string Status { get; set; } = "Confirmed";

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int NumberOfRooms { get; set; } = 1;

        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? CancellationDeduction { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? RefundAmount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
