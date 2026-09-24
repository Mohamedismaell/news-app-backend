namespace NewsBackend.Application.DTOs.Users;

public class UserSummaryResponse
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string? ProfilePhotoUrl { get; set; }
}