using System.ComponentModel.DataAnnotations;
using H5_Webprojektet_Api.Models;
using H5_Webprojektet_Api.Models.Database;
using H5_Webprojektet_Api.Models.DTOs;
using H5_Webprojektet_Api.Services.Interfaces;
using H5_Webprojektet_Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace H5_Webprojektet_Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserTodosController : ControllerBase
{
    private readonly ILogger<UserTodosController> _logger;
    private readonly IUserService _userService;
    private readonly IOptions<AppSettings> _options;

    public UserTodosController(ILogger<UserTodosController> logger, IUserService userService, IOptions<AppSettings> options)
    {
        _logger = logger;
        _userService = userService;
        _options = options;
    }

    [HttpPost("", Name = "CreateTodoEntry")]
    public async Task<ActionResult<string>> CreateTodoEntry([Required] TodoEntryDTO todoEntry, [Required] string userToken)
    {
        var user = await _userService.GetUserByToken(userToken);

        if (user is null)
        {
            return Unauthorized("Invalid user token");
        }

        await _userService.CreateTodoEntry(user, todoEntry);

        return Ok("Todo entry created");
    }

    [HttpGet("", Name = "GetTodoEntries")]
    public async Task<ActionResult<List<TodoEntry>>> GetTodoEntries([Required] string userToken)
    {
        var user = await _userService.GetUserByToken(userToken);

        if (user is null)
        {
            return Unauthorized("Invalid user token");
        }

        var todoEntries = user.TodoEntries.Select(entry => new TodoEntry
        {
            Id = entry.Id,
            Title = Cryptography.DecryptString(entry.Title, _options.Value.AesKey),
            Description = Cryptography.DecryptString(entry.Description, _options.Value.AesKey),
        });

        return Ok(todoEntries);
    }
}