namespace BookingGrid.AuthService.DTOs
{
    /// <summary>
    /// Data Transfer Object for requesting a password reset.
    /// </summary>
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
