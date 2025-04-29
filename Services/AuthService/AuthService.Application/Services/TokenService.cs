using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AuthService.Application.Interfaces;

namespace AuthService.Application.Services;

public class TokenService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : ITokenService
{
    public async Task<string> GenerateToken(ApplicationUser user)
    {

        var tokenKey = configuration["Jwt:SecretKey"] ?? throw new Exception("Can't access TokenKey from appsettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));


        var claims = await userManager.GetClaimsAsync(user);

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
