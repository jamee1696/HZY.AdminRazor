using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.DTO.Sys
{
    public class Sys_RoleMenuFunctionDto
    {

        public Guid RoleId { get; set; }

        public Guid MenuId { get; set; }

        public List<Guid> FunctionIds { get; set; }


    }
}
