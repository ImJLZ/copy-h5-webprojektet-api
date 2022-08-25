using System.Text.Json.Serialization;

namespace H5_Webprojektet_Api.Models.Database;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string FakeToken { get; set; }
    public int EncryptionMethod { get; set; }
    [JsonIgnore] public ICollection<TodoEntry> TodoEntries { get; set; } = new List<TodoEntry>();
}