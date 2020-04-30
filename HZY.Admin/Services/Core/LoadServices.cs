/*
 * *******************************************************
 *
 * 作者：hzy
 *
 * 开源地址：https://gitee.com/hzy6
 *
 * *******************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HZY.Admin.Services.Core
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// 加载服务
    /// </summary>
    public static class LoadServices
    {

        public static void StartService(this IServiceCollection serviceCollection, Type type)
        {
            if (type == null) throw new ArgumentException(" 参数 type null 异常!");

            var classList = type.Assembly.ExportedTypes
                .Where(w => w.GetCustomAttribute(typeof(AppServiceAttribute)) != null || w.BaseType.GetCustomAttribute(typeof(AppServiceAttribute)) != null)
                .ToList();

            foreach (var item in classList)
            {
                var appService = item.GetCustomAttribute<AppServiceAttribute>();

                if (item.BaseType != null)
                {
                    appService = item.BaseType.GetCustomAttribute<AppServiceAttribute>();
                }

                if (appService == null) continue;

                switch (appService.serviceType)
                {
                    case ServiceType.Scoped:
                        serviceCollection.AddScoped(item);
                        break;
                    case ServiceType.Transient:
                        serviceCollection.AddTransient(item);
                        break;
                    case ServiceType.Singleton:
                        serviceCollection.AddSingleton(item);
                        break;
                    default:
                        throw new NotSupportedException();
                        //break;
                }

            }

        }

        /// <summary>
        /// 服务标记 用于 程序 启动 扫描到后自动 注册服务
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class AppServiceAttribute : Attribute
        {

            public ServiceType serviceType { get; set; } = ServiceType.Scoped;

            public AppServiceAttribute(ServiceType serviceType = ServiceType.Scoped)
            {

            }

        }

        public enum ServiceType
        {
            Scoped,
            Transient,
            Singleton
        }

    }
}
