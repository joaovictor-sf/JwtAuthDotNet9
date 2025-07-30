﻿using JwtAuthDotNet9.Entities;
using JwtAuthDotNet9.Entities.DTOs;

namespace JwtAuthDotNet9.Services {
    public interface IAuthService {
        Task<User?> RegisterAsync(UserDTO request);
        Task<TokenResponseDTO?> LoginAsync(UserDTO request);
        Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
    }
}
