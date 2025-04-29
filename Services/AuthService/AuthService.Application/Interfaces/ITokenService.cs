using System;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface ITokenService
{
    Task<string> GenerateToken(ApplicationUser user);
}
