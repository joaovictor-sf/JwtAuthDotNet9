using JwtAuthDotNet9.Entities;
using JwtAuthDotNet9.Entities.DTOs;
using JwtAuthDotNet9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthDotNet9.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase {
        public static User user = new();

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request) {
            var user = await authService.RegisterAsync(request);

            if (user is null) {
                return BadRequest("User already exists");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login(UserDTO request) {
            var result = await authService.LoginAsync(request);
            if (result is null) {
                return Unauthorized("Invalid username or password");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request) {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AcessToken is null || result.RefreshToken is null) {
                return Unauthorized("Invalid or expired refresh token");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> GetUser() {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (userName is null) {
                return Unauthorized("User not found");
            }
            return Ok($"Hello {userName}");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public ActionResult<string> AdminOnlyEndPoint() {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (userName is null) {
                return Unauthorized("User not found");
            }
            return Ok($"Hello {userName}, you are an Admin");
        }
    }
}
