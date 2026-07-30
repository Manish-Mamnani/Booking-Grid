using System.ComponentModel.DataAnnotations;

namespace BookingGrid.AuthService.DTOs
{
    /// <summary>
    /// Data Transfer Object for login requests.
    /// </summary>
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
