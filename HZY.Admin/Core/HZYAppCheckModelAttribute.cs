using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZY.Admin.Core
{
    using HZY.Toolkit;
    using Microsoft.AspNetCore.Mvc.Filters;

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
                var values = context.ModelState.Values;
                foreach (var item in values)
                {
                    foreach (var error in item.Errors)
                    {
                        if (error.ErrorMessage.Contains("此内容不能为空"))
                        {
                            throw new MessageBox($"{item} {error.ErrorMessage}");
                        }
                        throw new MessageBox(error.ErrorMessage);
                    }
                }
            }

        }



    }
}
