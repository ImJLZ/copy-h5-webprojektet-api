using System.Text.Json.Serialization;

namespace H5_Webprojektet_Api.Models.Database;

public class TodoEntry
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    [JsonIgnore] public AppUser User { get; set; }
}