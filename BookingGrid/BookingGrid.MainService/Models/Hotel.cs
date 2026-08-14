using System.ComponentModel.DataAnnotations;

namespace BookingGrid.MainService.Models
{
    /// <summary>
    /// Represents a hotel entity with its status, manager information, rooms, images, and aggregate rating data.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Range(0, 5)]
        public double AverageRating { get; set; }

        [Range(0, int.MaxValue)]
        public int TotalReviews { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        [Required]
        public int CreatedByUserId { get; set; }

        [Required]
        [EmailAddress]
        public string ManagerEmail { get; set; } = string.Empty;

        public List<Room> Rooms { get; set; } = new();
    }
}
