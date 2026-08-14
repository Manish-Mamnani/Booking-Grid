using System.ComponentModel.DataAnnotations;

namespace BookingGrid.MainService.Models
{
    /// <summary>
    /// Represents a room type within a hotel, including pricing and availability tracking.
    /// </summary>
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public int HotelId { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue)]
        public int TotalCount { get; set; }

        [Range(0, int.MaxValue)]
        public int AvailableCount { get; set; }

        public Hotel? Hotel { get; set; }
    }
}
