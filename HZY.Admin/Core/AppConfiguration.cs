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
        public readonly IConfiguration configuration;

        public AppConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
            //AppConfig 
            this.Mapping($"AppConfig");

        }

        /// <summary>
        /// 映射数据 到 属性
        /// </summary>
        /// <param name="key"></param>
        private void Mapping(string key)
        {
            var propertes = this.GetType().GetProperties();
            foreach (var item in propertes)
            {
                var value = configuration[$"{key}:{item.Name}"];

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