namespace TAABP.Application.DTOs.AccountDto
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
