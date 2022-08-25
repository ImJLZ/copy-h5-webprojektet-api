using System.ComponentModel.DataAnnotations;
using H5_Webprojektet_Api.Models.DTOs;
using H5_Webprojektet_Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace H5_Webprojektet_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("", Name = "CreateUser")]
    public async Task<ActionResult> CreateUser(AppUserDTO appUserDto, [Required] int encryptionMethod)
    {
        await _userService.CreateUser(appUserDto, encryptionMethod);

        return Ok();
    }
}