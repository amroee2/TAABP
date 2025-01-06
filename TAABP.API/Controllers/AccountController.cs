﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.TokenGenerators;
using TAABP.Core;

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
        public AccountController(ITokenGenerator tokenGenerator, IUserService userService, IEmailService emailService, IStorageService storageService)
        {
            _userService = userService;
            _emailService = emailService;
            _storageService = storageService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegisterDto registerDto)
        {
            try
            {
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
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginAsync(loginDto);
                return Ok(new { token });
            }
            catch (InvalidLoginException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync(string token)
        {
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

                await _userService.CreateUserAsync(registerDto);

                await _storageService.DeleteTokenAsync(token);

                return Ok(new { message = "Email confirmed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("change-email")]
        [Authorize]
        public async Task<IActionResult> ChangeEmailAsync(ChangeEmailDto changeEmailDto)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(_userService.GetCurrentUserId());

                var emailExists = await _userService.CheckEmailAsync(changeEmailDto.NewEmail);
                if (emailExists)
                {
                    return Conflict(new { message = "Email already exists" });
                }
                await _userService.ChangeEmailAsync(_userService.GetCurrentUserId(), changeEmailDto);
                await _emailService.SendEmailAsync(
                    changeEmailDto.NewEmail,
                    "Confirm Your Email Change",
                    $"<p>Your Email has neem changed</p>"
                );
                return Accepted(new { message = "Please check your email to confirm your email change." });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (EmailAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
