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
        private readonly IUserMapper _userMapper;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHotelMapper _hotelMapper;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,
            IUserMapper userMapper, UserManager<User> userManager, IHotelMapper hotelMapper)
        {
            _userRepository = userRepository;
            _userMapper = userMapper;

            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _hotelMapper = hotelMapper;
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
            if (user.UserName != userDto.UserName)
            {
                var userNameExists = await _userRepository.CheckIfUserNameExists(userDto.UserName);
                if (userNameExists)
                {
                    throw new InvalidOperationException("Username already exists");
                }
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
    }
}
