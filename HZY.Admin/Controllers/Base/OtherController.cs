using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Base
{
    public class OtherController : ApiBaseController
    {


        #region 页面 Views

        [HttpGet(nameof(Index))]
        public IActionResult Index()
        {
            return View();
        }

        #endregion




    }
}