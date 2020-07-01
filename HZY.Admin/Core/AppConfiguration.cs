using System;

namespace HZY.Admin.Core
{
    using HZY.Toolkits;
    using HZY.Toolkits.HzyNetCoreUtil.Attributes;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// 程序配置信息映射类 appsettings.json
    /// </summary>
    [AppService(ServiceType.Singleton)]
    public class AppConfiguration
    {
        private string AppConfigKey = "AppConfig";
        public AppConfiguration(IConfiguration configuration)
        {
            //AppConfig
            var propertes = this.GetType().GetProperties();
            foreach (var item in propertes)
            {
                var value = configuration[$"{AppConfigKey}:{item.Name}"];

                if (item.PropertyType == typeof(Guid))
                {
                    item.SetValue(this, value.ToGuid());
                }
                else if (item.PropertyType == typeof(int))
                {
                    item.SetValue(this, value.ToInt32());
                }
                else
                {
                    item.SetValue(this, value);
                }
            }

        }

        public string JwtKeyName { get; set; }
        public string JwtSecurityKey { get; set; }
        public Guid AdminRoleID { get; set; }
        public Guid SysMenuID { get; set; }



    }
}