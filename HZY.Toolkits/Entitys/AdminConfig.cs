using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Toolkits.Entitys
{
    public class AdminConfig
    {
        public string SqlServerConnStr { get; set; }

        public string JwtKeyName { get; set; }

        public string JwtSecurityKey { get; set; }

        public Guid AdminRoleID { get; set; }

        public Guid SysMenuID { get; set; }


    }
}
