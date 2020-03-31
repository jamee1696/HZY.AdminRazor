using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZY.Admin.Core
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using HZY.Toolkit;
    using HZY.Services;
    using HZY.Services.Sys;

    public class HZYApiAuthorizationCheckAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 忽略本特性
        /// </summary>
        public bool Ignore { get; set; } = false;

        /// <summary>
        /// 每次请求Action之前发生，，在行为方法执行前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (Ignore) return;

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

        }

    }
}
