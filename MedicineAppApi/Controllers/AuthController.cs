using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.Services;
using MedicineAppApi.DTOs;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IUserRepository userRepository, IMapper mapper)
        {
            _authService = authService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginRequest);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] LoginRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if user already exists
            if (await _userRepository.EmailExistsAsync(registerRequest.Email))
            {
                return BadRequest(new { Message = "User with this email already exists" });
            }

            // Create new user
            var user = new User
            {
                Email = registerRequest.Email,
                Password = _authService.HashPassword(registerRequest.Password),
                FirstName = "User",
                LastName = "Name",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            // Return login response
            var loginRequest = new LoginRequestDto
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };

            return await Login(loginRequest);
        }
    }
}
