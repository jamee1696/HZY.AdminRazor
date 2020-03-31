using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.DTO.Sys
{
    using AutoMapper;
    using HZY.DTO.Core;
    using HZY.Models.Sys;

    public class Sys_MenuDto
    {
        public Sys_Menu Model { get; set; }
        public List<Guid> FunctionIds { get; set; }

    }
}
