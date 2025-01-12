using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.TokenGenerators;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IStorageService _storageService;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<ChangeEmailDto> _changeEmailValidator;
        private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
        public AccountController(ITokenGenerator tokenGenerator,
            IUserService userService,
            IEmailService emailService,
            IStorageService storageService,
            IAccountService accountService,
            IValidator<LoginDto> loginValidator,
            IValidator<ChangeEmailDto> changeEmailValidator,
            IValidator<ResetPasswordDto> resetPasswordValidator,
            IValidator<RegisterDto> registerValidator)
        {
            _userService = userService;
            _emailService = emailService;
            _storageService = storageService;
            _tokenGenerator = tokenGenerator;
            _accountService = accountService;
            _logger = Log.ForContext<AccountController>();
            _loginValidator = loginValidator;
            _changeEmailValidator = changeEmailValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegisterDto registerDto)
        {
            _logger.Information("Registering user with email {Email}", registerDto.Email);
            try
            {
                await _registerValidator.ValidateAndThrowAsync(registerDto);
                var emailExists = await _userService.CheckEmailAsync(registerDto.Email);
                if (emailExists)
                {
                    return Conflict(new { message = "Email already exists" });
                }
                var token = _tokenGenerator.GenerateToken(registerDto.Email);

                await _storageService.StoreUserAsync(token, registerDto);

                var confirmationLink = $"https://localhost:7210/api/Account/confirm-email?token={token}";

                await _emailService.SendEmailAsync(
                    registerDto.Email,
                    "Confirm Your Email",
                    $"<p>Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a></p>"
                );

                return Accepted(new { message = "Please check your email to confirm your registration." });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while registering user with email {Email}", registerDto.Email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            _logger.Information("Logging in user with email {Email}", loginDto.Email);
            try
            {
                await _loginValidator.ValidateAndThrowAsync(loginDto);
                var token = await _accountService.LoginAsync(loginDto);
                _logger.Information("User with email {Email} logged in successfully", loginDto.Email);
                return Ok(new { token });
            }
            catch (InvalidLoginException ex)
            {
                _logger.Warning("Invalid login attempt for user with email {Email}", loginDto.Email);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while logging in user with email {Email}", loginDto.Email);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(string token)
        {
            _logger.Information("Confirming email with token {Token}", token);
            try
            {
                if (!_tokenGenerator.ValidateToken(token))
                {
                    return BadRequest(new { message = "Invalid or expired token." });
                }
                var registerDto = await _storageService.RetrieveUserAsync(token);
                if (registerDto == null)
                {
                    return NotFound(new { message = "Token not found" });
                }

                await _accountService.CreateUserAsync(registerDto);

                await _storageService.DeleteTokenAsync(token);
                _logger.Information("Email confirmed successfully");
                return Ok(new { message = "Email confirmed successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while confirming email with token {Token}", token);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("change-email")]
        [Authorize]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            _logger.Information("Changing email for user with ID {UserId}", _userService.GetCurrentUserId());
            try
            {
                await _changeEmailValidator.ValidateAndThrowAsync(changeEmailDto);
                var user = await _userService.GetUserByIdAsync(_userService.GetCurrentUserId());

                var emailExists = await _userService.CheckEmailAsync(changeEmailDto.NewEmail);
                if (emailExists)
                {
                    return Conflict(new { message = "Email already exists" });
                }
                await _accountService.ChangeEmailAsync(_userService.GetCurrentUserId(), changeEmailDto);
                await _emailService.SendEmailAsync(
                    changeEmailDto.NewEmail,
                    "Confirm Your Email Change",
                    $"<p>Your Email has neem changed</p>"
                );
                _logger.Information("Email changed successfully");
                return Accepted(new { message = "Please check your email to confirm your email change." });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", _userService.GetCurrentUserId());
                return NotFound(new { message = ex.Message });
            }
            catch (EmailAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while changing email for user with ID {UserId}", _userService.GetCurrentUserId());
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            _logger.Information("Forgot password for user with email {Email}", forgotPasswordDto.Email);
            try
            {
                var user = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email);
                var token = _tokenGenerator.GenerateToken(user.Email);
                await _storageService.StoreUserAsync(token, new RegisterDto { Email = user.Email });
                var resetLink = $"https://localhost:7210/api/Account/reset-password?token={token}";
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Reset Your Password",
                    $"<p>Please reset your password by clicking this link: <a href='{resetLink}'>Reset Password</a></p>"
                );
                return Accepted(new { message = "Please check your email to reset your password." });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with email {Email} not found", forgotPasswordDto.Email);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while resetting password for user with email {Email}", forgotPasswordDto.Email);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            _logger.Information("Resetting password for user with token {Token}", resetPasswordDto.Token);
            try
            {
                await _resetPasswordValidator.ValidateAndThrowAsync(resetPasswordDto);
                if (!_tokenGenerator.ValidateToken(resetPasswordDto.Token))
                {
                    return BadRequest(new { message = "Invalid or expired token." });
                }
                var registerDto = await _storageService.RetrieveUserAsync(resetPasswordDto.Token);
                if (registerDto == null)
                {
                    return NotFound(new { message = "Token not found" });
                }
                await _accountService.ResetUserPasswordAsync(registerDto.Email, resetPasswordDto.Password);
                await _storageService.DeleteTokenAsync(resetPasswordDto.Token);
                _logger.Information("Password reset successfully");
                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while resetting password for user with token {Token}", resetPasswordDto.Token);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
