using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers
{
    using HZY.Admin.Core;
    using HZY.Services.Sys;
    using HZY.Toolkit;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.IO;
    using System.Text;

    [ApiExplorerSettings(GroupName = nameof(ApiVersionsEnum.Admin)), Core.HZYApiAuthorizationCheck]
    public class ApiBaseController : BaseController
    {
        protected readonly Sys_MenuService menuService;
        public ApiBaseController(Sys_MenuService _menuService)
        {
            this.menuService = _menuService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var menu = menuService.GetMenuByPathAsync(context.HttpContext.Request.PathBase).Result;

            if (menu != null && !context.HttpContext.IsAjaxRequest())
            {
                var dicPower = menuService.GetPowerStateByMenuId(menu.Menu_ID).Result;

                if (!dicPower["Have"].ToBool())
                {
                    var result = new ContentResult();
                    result.Content = "无权访问!";
                    result.ContentType = "text/html; charset=utf-8";
                    context.Result = result;
                    return;
                }

                ViewData["power"] = dicPower;
            }




        }

    }
}