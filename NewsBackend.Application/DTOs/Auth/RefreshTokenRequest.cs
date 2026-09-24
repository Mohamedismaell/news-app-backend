using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}