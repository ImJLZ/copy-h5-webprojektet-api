using H5_Webprojektet_Api.Models.Database;
using H5_Webprojektet_Api.Models.DTOs;

namespace H5_Webprojektet_Api.Services.Interfaces;

public interface IUserService
{
    Task<AppUser> CreateUser(AppUserDTO appUserDto, int encryptionMethod);
    Task<AppUser?> GetUserByEmail(string email);
    Task<AppUser?> GetUserByToken(string token);
    Task<TodoEntry> CreateTodoEntry(AppUser user, TodoEntryDTO todoEntry);
}