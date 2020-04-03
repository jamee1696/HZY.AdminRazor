using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZY.Admin.Core
{
    using HZY.Toolkit;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    /// <summary>
    /// 实体模型验证
    /// </summary>
    public class HZYAppCheckModelAttribute : ActionFilterAttribute
    {


        /// <summary>
        /// 每次请求Action之前发生，，在行为方法执行前执行
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                var messages = new List<string>();
                var keys = context.ModelState.Keys;
                var values = context.ModelState.Values;
                foreach (var item in keys)
                {
                    var value = context.ModelState.FirstOrDefault(w => w.Key == item).Value;
                    foreach (var err in value.Errors)
                    {
                        if (err.ErrorMessage.Contains("内容不能为空"))
                        {
                            messages.Add($"{item} {err.ErrorMessage}");
                        }
                        else
                        {
                            messages.Add($"{err.ErrorMessage}");
                        }
                    }
                }

                context.Result = new JsonResult(new ApiResult((int)StatusCodeEnum.失败, string.Join("<br /><br />", messages)));

            }

        }



    }
}
