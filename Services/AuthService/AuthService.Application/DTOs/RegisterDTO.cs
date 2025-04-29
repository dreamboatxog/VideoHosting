namespace AuthService.Application.DTOs;

public record class RegisterDTO
{
    public required string UserName { get; set; }
    public required string  Email { get; set; }
    public required string Password { get; set; }
}
