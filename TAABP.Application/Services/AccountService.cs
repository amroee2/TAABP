using Microsoft.AspNetCore.Identity;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.TokenGenerators;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserMapper _userMapper;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly SignInManager<User> _signInManager;
        public AccountService(UserManager<User> userManager, IUserRepository userRepository,
            IUserMapper userMapper, ITokenGenerator tokenGenerator, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _userMapper = userMapper;
            _tokenGenerator = tokenGenerator;
            _signInManager = signInManager;
        }

        public async Task CreateUserAsync(RegisterDto registerDto)
        {
            var emailExists = await _userRepository.CheckEmailAsync(registerDto.Email);
            if (emailExists)
            {
                throw new EmailAlreadyExistsException("Email Already Exists");
            }

            var user = _userMapper.RegisterDtoToUser(registerDto);
            user.UserName = registerDto.UserName;
            user.EmailConfirmed = true;
            var isCreated = await _userRepository.CreateUserAsync(user, registerDto.Password);

            if (!isCreated)
            {
                throw new EntityCreationException("User Creation Failed");
            }
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new InvalidLoginException("Invalid Email or Password");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
            {
                throw new InvalidLoginException($"Invalid Email or Password");
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            return _tokenGenerator.GenerateToken(user.Id, userRoles);
        }

        public async Task ChangeEmailAsync(string userId, ChangeEmailDto changeEmailDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
            }
            var oldEmail = user.Email;
            var checkPassword = await _userManager.CheckPasswordAsync(user, changeEmailDto.Password);
            if (!checkPassword)
            {
                throw new Exception("Invalid Password.");
            }

            var emailExists = await _userRepository.CheckEmailAsync(changeEmailDto.NewEmail);
            if (emailExists)
            {
                throw new EmailAlreadyExistsException("Email Already Exists.");
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);
            var emailChangeResult = await _userManager.ChangeEmailAsync(user, changeEmailDto.NewEmail, token);
            if (!emailChangeResult.Succeeded)
            {
                throw new Exception("Failed to change email: " + string.Join(", ", emailChangeResult.Errors.Select(e => e.Description)));
            }

            if (user.UserName == oldEmail)
            {
                var usernameChangeResult = await _userManager.SetUserNameAsync(user, changeEmailDto.NewEmail);
                if (!usernameChangeResult.Succeeded)
                {
                    throw new Exception("Failed to change username: " + string.Join(", ", usernameChangeResult.Errors.Select(e => e.Description)));
                }
            }
        }

        public async Task ResetUserPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new EntityNotFoundException("User not found.");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPasswordResult.Succeeded)
            {
                throw new Exception("Failed to reset password: " + string.Join(", ", resetPasswordResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
