using TAABP.Application.DTOs;
using TAABP.Application.PasswordHashing;
using TAABP.Application.Profile;
using TAABP.Application.RepositoryInterfaces;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserMapper _userMapper;
        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserMapper userMapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _userMapper = userMapper;
        }

        public async Task CreateUserAsync(RegisterDto registerDto)
        {
            var hashedPassword = _passwordHasher.HashPassword(registerDto.Password);

            var user = _userMapper.RegisterDtoToUser(registerDto);

            user.PasswordHash = hashedPassword;

            await _userRepository.CreateUserAsync(user);
        }
    }
}
