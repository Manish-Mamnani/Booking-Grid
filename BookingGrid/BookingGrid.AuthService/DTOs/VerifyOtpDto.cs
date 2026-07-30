namespace BookingGrid.AuthService.DTOs
{
    /// <summary>
    /// Data Transfer Object for verifying an OTP during password reset.
    /// </summary>
    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}
