using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Implement.Permissions.Identity;
using CDKX.Services.Implement.Permissions.Security;
using Microsoft.AspNet.Identity;
using OSharp.Core.Dependency;

namespace CDKX.Services.Implement
{
    public static class ServiceCollectionExtensions
    {
        public static void AddImplementServices(this IServiceCollection services)
        {
            //Identity
            services.AddScoped<UserStore>();
            services.AddScoped<UserManager<SysUser, int>, UserManager>();

            //Security
            services.AddScoped<FunctionMapStore>();
            services.AddScoped<EntityMapStore>();

            //OAuth
            //services.AddScoped<ClientStore>();
        }
    }
}
