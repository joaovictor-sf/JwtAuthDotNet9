namespace JwtAuthDotNet9.Entities.DTOs {
    public class TokenResponseDTO {
        public required string AcessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
