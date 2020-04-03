using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Base
{
    using HZY.Services.Sys;

    public class OtherController : ApiBaseController
    {
        public OtherController(Sys_MenuService _menuService)
            : base(_menuService)
        {

        }

        #region 页面 Views

        [HttpGet(nameof(Index))]
        public IActionResult Index()
        {
            return View();
        }

        #endregion




    }
}