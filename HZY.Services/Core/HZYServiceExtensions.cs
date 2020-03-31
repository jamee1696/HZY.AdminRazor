using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Services.Core
{
    using Microsoft.Extensions.DependencyInjection;
    using HZY.Services.Sys;
    
    public static class HZYServiceExtensions
    {

        /// <summary>
        /// 注入 HZYServices 服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseHZYServices(this IServiceCollection services)
        {

            //注入 业务 服务

            //base
            services.AddScoped<AccountService>();
            services.AddScoped<CCTService>();
            services.AddScoped<Sys_AppLogService>();
            services.AddScoped<Sys_FunctionService>();
            services.AddScoped<Sys_MenuService>();
            services.AddScoped<Sys_RoleMenuFunctionService>();
            services.AddScoped<Sys_RoleService>();
            services.AddScoped<Sys_UserService>();
            //
            services.AddScoped<MemberService>();

            return services;
        }




    }
}
