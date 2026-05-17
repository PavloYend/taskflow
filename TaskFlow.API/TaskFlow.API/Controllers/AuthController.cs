using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.DTOs;
using TaskFlow.API.Interfaces;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        string? token = await _authService.RegisterAsync(registerDto);

        if (token == null)
        {
            return BadRequest("Email already exists");
        }

        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        string? token = await _authService.LoginAsync(loginDto);

        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(token);
    }
}
