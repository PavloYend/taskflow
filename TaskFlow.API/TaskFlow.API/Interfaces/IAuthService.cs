using TaskFlow.API.DTOs;

namespace TaskFlow.API.Interfaces;

public interface IAuthService
{
    Task<string?> RegisterAsync(RegisterDto registerDto);

    Task<string?> LoginAsync(LoginDto loginDto);
}
