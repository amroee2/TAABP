using Microsoft.AspNetCore.Identity;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
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

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher,
            IUserMapper userMapper, ITokenGenerator tokenGenerator, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userMapper = userMapper;
            _tokenGenerator = tokenGenerator;
            _signInManager = signInManager;
            _userManager = userManager;
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
    }
}
