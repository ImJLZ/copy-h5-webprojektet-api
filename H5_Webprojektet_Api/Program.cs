using System.Net;
using Autofac.Extensions.DependencyInjection;

namespace H5_Webprojektet_Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);
        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(opt =>
            {
                opt.Listen(IPAddress.Loopback, 80);
                opt.Listen(IPAddress.Loopback, 443, listenOptions =>
                {
                    // store password securely in a vault or keystore, not in code like this
                    listenOptions.UseHttps("api.martin.pfx", "123456");
                });
            });
        });
        builder.ConfigureHostConfiguration(hostCfg =>
        { 
            hostCfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        });
        builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        return builder;
    }
}