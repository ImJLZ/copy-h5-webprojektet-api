using Autofac;
using H5_Webprojektet_Api.Models;
using H5_Webprojektet_Api.Modules;
using Microsoft.EntityFrameworkCore;

namespace H5_Webprojektet_Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var appSettings = new AppSettings();
        _configuration.Bind(appSettings);

        services.AddSingleton(_configuration);
        services.AddSingleton((IConfigurationRoot)_configuration);
        services.Configure<AppSettings>(_configuration);

        if (string.IsNullOrEmpty(appSettings.ConnectionStrings.Default))
        {
            throw new InvalidOperationException("Invalid or missing connection string in appsettings.json");
        }
        
        if (string.IsNullOrEmpty(appSettings.AesKey))
        {
            throw new InvalidOperationException("Missing aeskey in appsettings.json");
        }

        var mysqlVersion = new MySqlServerVersion(new Version(8, 0, 29));

        services.AddDbContext<DbContext>(options =>
            options.UseMySql(appSettings.ConnectionStrings.Default, mysqlVersion));
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app)
    {
        var isProduction = _environment.IsEnvironment("production");
        if (!isProduction)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "H5 Webprojektet Api Martin"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<CommonModule>();
    }
}