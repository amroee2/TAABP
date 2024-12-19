using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.PasswordHashing;
using TAABP.Application.Profile;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.TokenGenerators;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserMapper _userMapper;
        private readonly ITokenGenerator _tokenGenerator;
        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserMapper userMapper, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userMapper = userMapper;
            _tokenGenerator = tokenGenerator;
        }

        public async Task CreateUserAsync(RegisterDto registerDto)
        {
            var emailExists = await _userRepository.CheckEmailAsync(registerDto.Email);
            if (emailExists)
            {
                throw new EmailAlreadyExistsException("Email Already Exists");
            }
            var hashedPassword = _passwordHasher.HashPassword(registerDto.Password);

            var user = _userMapper.RegisterDtoToUser(registerDto);
            user.PasswordHash = hashedPassword;

            await _userRepository.CreateUserAsync(user);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new InvalidLoginException("Invalid Email or Password");
            }
            var passwordValid = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);
            if (!passwordValid)
            {
                throw new InvalidLoginException("Invalid Email or Password");
            }
            return _tokenGenerator.GenerateToken(loginDto.Email);
        }
    }
}
