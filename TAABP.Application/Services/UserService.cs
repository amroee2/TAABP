using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TAABP.Application.DTOs;
using TAABP.Application.DTOs.AccountDto;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.HotelMapping;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.TokenGenerators;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserMapper _userMapper;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHotelMapper _hotelMapper;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IHttpContextAccessor httpContextAccessor,
            IUserMapper userMapper, ITokenGenerator tokenGenerator, SignInManager<User> signInManager, UserManager<User> userManager
            , IHotelMapper hotelMapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userMapper = userMapper;
            _tokenGenerator = tokenGenerator;
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _hotelMapper = hotelMapper;
        }

        public async Task CreateUserAsync(RegisterDto registerDto)
        {
            var emailExists = await _userRepository.CheckEmailAsync(registerDto.Email);
            if (emailExists)
            {
                throw new EmailAlreadyExistsException("Email Already Exists");
            }

            var user = _userMapper.RegisterDtoToUser(registerDto);
            user.UserName = registerDto.Email;
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
            return _tokenGenerator.GenerateToken(user.Id);
        }

        public async Task<UserDto> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            return _userMapper.UserToUserDto(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            return user;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            return users.Select(user => _userMapper.UserToUserDto(user)).ToList();
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            await _userRepository.DeleteUserAsync(user);
        }

        public async Task UpdateUserAsync(string id, UserDto userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            _userMapper.UserDtoToUser(userDto, user);
            user.Id = id;
            await _userRepository.UpdateUserAsync(user);
        }

        public async Task<string> GetCurrentUsernameAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user.UserName;
        }
        public string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        public async Task<List<HotelDto>> GetLastHotelsVisitedAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new EntityNotFoundException("User Not Found");
            }
            var hotels = await _userRepository.GetLastHotelsVisitedAsync(userId);
            return hotels.Select(hotel => _hotelMapper.HotelToHotelDto(hotel)).ToList();
        }

        public async Task<bool> CheckEmailAsync(string email)
        {
            return await _userRepository.CheckEmailAsync(email);
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
