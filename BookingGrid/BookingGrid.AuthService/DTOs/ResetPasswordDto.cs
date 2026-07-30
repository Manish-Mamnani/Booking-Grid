namespace BookingGrid.AuthService.DTOs
{
    /// <summary>
    /// Data Transfer Object for resetting the password after OTP verification.
    /// </summary>
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
