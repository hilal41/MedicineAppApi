using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Common.Helpers;
using MedicineAppApi.Common.Exceptions;

namespace MedicineAppApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string GenerateJwtToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password", "INVALID_CREDENTIALS");
            }

            if (!PasswordHelper.VerifyPassword(loginRequest.Password, user.Password))
            {
                throw new UnauthorizedException("Invalid email or password", "INVALID_CREDENTIALS");
            }

            var token = JwtHelper.GenerateJwtToken(user, _configuration);
            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = userInfo
            };
        }

        public string HashPassword(string password)
        {
            return PasswordHelper.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHelper.VerifyPassword(password, hashedPassword);
        }

        public string GenerateJwtToken(User user)
        {
            return JwtHelper.GenerateJwtToken(user, _configuration);
        }
    }
}
