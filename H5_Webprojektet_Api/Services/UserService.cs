using System.Security.Cryptography;
using System.Text;
using H5_Webprojektet_Api.Models;
using H5_Webprojektet_Api.Models.Database;
using H5_Webprojektet_Api.Models.DTOs;
using H5_Webprojektet_Api.Services.Interfaces;
using H5_Webprojektet_Api.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace H5_Webprojektet_Api.Services;

public class UserService : IUserService
{
    private readonly DbContext _dbContext;
    private readonly IOptions<AppSettings> _options;

    public UserService(DbContext dbContext, IOptions<AppSettings> options)
    {
        _dbContext = dbContext;
        _options = options;
    }

    public async Task<AppUser> CreateUser(AppUserDTO appUserDto, int encryptionMethod)
    {
        string hashedPassword, salt;
        switch (encryptionMethod)
        {
            case 1: // pbkdf2
                (hashedPassword, salt) = Cryptography.HashPasswordV2(appUserDto.Password, RandomNumberGenerator.Create());
                break;
            case 2: // bcrypt
                salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password, salt);
                break;
            case 3: // md5
                salt = Convert.ToBase64String(Cryptography.CreateSalt(32, RandomNumberGenerator.Create()));
                var hmacmd5 = new HMACMD5(Convert.FromBase64String(salt));
                hashedPassword = Convert.ToBase64String(hmacmd5.ComputeHash(Encoding.UTF8.GetBytes(appUserDto.Password)));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(encryptionMethod), encryptionMethod, "Invalid encryption method");
        }
        
        var user = new AppUser
        {
            Email = appUserDto.Email,
            Password = hashedPassword,
            Salt = salt,
            FakeToken = Guid.NewGuid().ToString(),
            EncryptionMethod = encryptionMethod
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public Task<AppUser?> GetUserByEmail(string email)
    {
        return Task.FromResult(_dbContext.Users.FirstOrDefault(x => x.Email.ToLower().Equals(email.ToLower())));
    }

    public Task<AppUser?> GetUserByToken(string token)
    {
        return Task.FromResult(_dbContext.Users.Include(x => x.TodoEntries).FirstOrDefault(x => x.FakeToken.Equals(token)));
    }

    public async Task<TodoEntry> CreateTodoEntry(AppUser user, TodoEntryDTO todoEntryDto)
    {
        var todoEntry = new TodoEntry
        {
            Title = Cryptography.EncryptString(todoEntryDto.Title, _options.Value.AesKey),
            Description = Cryptography.EncryptString(todoEntryDto.Description, _options.Value.AesKey),
            User = user
        };
        user.TodoEntries.Add(todoEntry);
        await _dbContext.SaveChangesAsync();

        return todoEntry;
    }
}