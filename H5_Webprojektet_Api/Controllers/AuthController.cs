using System.Security.Cryptography;
using System.Text;
using H5_Webprojektet_Api.Models.DTOs;
using H5_Webprojektet_Api.Services.Interfaces;
using H5_Webprojektet_Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace H5_Webprojektet_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;

    public AuthController(ILogger<AuthController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("Login", Name = "AuthUser")]
    public async Task<ActionResult<string>> AuthUser(AppUserDTO appUserDto)
    {
        var user = await _userService.GetUserByEmail(appUserDto.Email);
        if (user is null)
        {
            // ideally you should not return the same message as if the user failed to authenticate
            // to prevent hackers from knowing if the user exists or not
            // but for the sake of this example i will return an explicit message instead
            return Unauthorized("User with this email does not exist");
        }

        var isVerified = false;
        switch (user.EncryptionMethod)
        {
            case 1: // pbkdf2
                isVerified = Cryptography.VerifyHashedPassword(user.Password, user.Salt, appUserDto.Password);
                break;
            case 2: // bcrypt
                isVerified = BCrypt.Net.BCrypt.Verify(appUserDto.Password, user.Password);
                break;
            case 3: // md5
                var hmacmd5 = new HMACMD5(Convert.FromBase64String(user.Salt));
                var hashedPassword = Convert.ToBase64String(hmacmd5.ComputeHash(Encoding.UTF8.GetBytes(appUserDto.Password)));
                
                isVerified = user.Password == hashedPassword;
                break;
        }

        if (isVerified == false)
        {
            return Unauthorized("Email or password is incorrect");
        }
        
        // here a JWT token is generated and returned to the user, but that is outside of the scope of this project
        return Ok(user.FakeToken);
    }
}