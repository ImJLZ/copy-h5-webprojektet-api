using Autofac;
using H5_Webprojektet_Api.Services;
using H5_Webprojektet_Api.Services.Interfaces;

namespace H5_Webprojektet_Api.Modules;

public class CommonModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        #region APIs
        
        builder.RegisterType<UserService>().As<IUserService>();
        // builder.RegisterType<TodoEntryService>().As<ITodoEntryService>();
        
        #endregion
    }
}