using System.ComponentModel.DataAnnotations;

namespace NewsBackend.Application.DTOs.Users;

public class UpdateUserRequest
{
    [StringLength(100)]
    public string? DisplayName { get; set; }

    [StringLength(100)]
    public string? Username { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }
}