namespace H5_Webprojektet_Api.Models;

public class AppSettings
{
    public string Host { get; set; }
    public string AesKey { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; }
}

public class ConnectionStrings
{
    public string Default { get; set; }
}