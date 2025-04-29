using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Controllers;

[Route("auth-api/[controller]")]
[ApiController]
public class AuthController(UserManager<ApplicationUser> _userManager,
                            SignInManager<ApplicationUser> _signInManager,
                            ITokenService _tokenService) : Controller
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.UserName)) return BadRequest("Username is taken");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser() { UserName = registerDTO.UserName, Email = registerDTO.Email };
        var result = await _userManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(user.Id);
    }

    [HttpGet("login")]
    public async Task<ActionResult<String>> Login(LoginDTO loginDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, false);
        if (!result.Succeeded)
            return Unauthorized("Invalid credentials");

        var token = await _tokenService.GenerateToken(user);
        return Ok(new { Token = token });
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(user => user.NormalizedUserName == username.ToUpper());
    }
}
