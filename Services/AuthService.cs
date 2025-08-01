﻿using JwtAuthDotNet9.Data;
using JwtAuthDotNet9.Entities;
using JwtAuthDotNet9.Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthDotNet9.Services {
    public class AuthService(UserDbContext context, IConfiguration configuration) : IAuthService {
        public async Task<TokenResponseDTO?> LoginAsync(UserDTO request) {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user is null) {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed) {
                return null;
            }
            return await CreateTokenResponse(user);
        }

        private async Task<TokenResponseDTO> CreateTokenResponse(User user) {
            return new TokenResponseDTO
            {
                AcessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<User?> RegisterAsync(UserDTO request) {
            if (await context.Users.AnyAsync(u => u.Username == request.Username)) {
                return null; // User already exists
            }

            var user = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request) {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null) {
                return null; // Invalid or expired refresh token
            }
            
            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userID,string refreshToken) {
            var user = await context.Users.FirstAsync(u => u.Id == userID);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow) {
                return null; // Invalid or expired refresh token
            }
            return user;
        }

        private string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user) {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set expiry time for the refresh token
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User") // Default role if not set
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<String>("AppSettings:Issuer"),
                audience: configuration.GetValue<String>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

    }
}
