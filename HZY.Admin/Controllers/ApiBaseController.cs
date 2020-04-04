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
    using Newtonsoft.Json;
    using System.IO;
    using System.Text;

    [ApiExplorerSettings(GroupName = nameof(ApiVersionsEnum.Admin), IgnoreApi = true)]
    public class ApiBaseController : BaseController
    {
        protected readonly Guid MenuId;
        protected readonly Sys_MenuService menuService;

        public ApiBaseController(Guid menuId, Sys_MenuService _menuService)
        {
            this.MenuId = menuId;
            this.menuService = _menuService;
        }

        public ApiBaseController()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            #region 检查是否登录 授权

            //获取 token
            var token = AccountService.GetToken(context.HttpContext);

            if (string.IsNullOrWhiteSpace(token))
            {
                if (context.HttpContext.IsAjaxRequest())
                {
                    throw new MessageBox(StatusCodeEnum.未授权, $"{StatusCodeEnum.未授权.ToString()}");
                }
                else
                {
                    var Alert = $@"<script type='text/javascript'>
                                        alert('{StatusCodeEnum.未授权.ToString()}！请重新登录授权！');
                                        top.window.location='/Authorization/Index';
                                    </script>";
                    context.Result = new ContentResult() { Content = Alert, ContentType = "text/html;charset=utf-8;" };
                }
                return;
            }

            #endregion

            #region 检查页面权限信息

            if (MenuId == Guid.Empty) return;

            var power = this.menuService.GetPowerStateByMenuId(this.MenuId).Result;

            if (!power["Have"].ToBool())
            {
                context.Result = new ContentResult() { Content = "您无权访问!", ContentType = "text/html;charset=utf-8;" };
                return;
            }

            ViewData["power"] =JsonConvert.SerializeObject(power);

            #endregion

        }


    }
}