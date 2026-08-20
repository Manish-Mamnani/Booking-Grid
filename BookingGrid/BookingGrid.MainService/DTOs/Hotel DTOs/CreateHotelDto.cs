using System.ComponentModel.DataAnnotations;

namespace BookingGrid.MainService.DTOs
{
    /// <summary>Request payload used to create or update a hotel's editable details.</summary>
    public class CreateHotelDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

    }
}
