namespace AuthService.Application.DTOs;

public record class LoginDTO
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
