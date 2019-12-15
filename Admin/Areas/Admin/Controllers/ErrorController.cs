using Microsoft.AspNetCore.Mvc;
//
using Aop;
using Toolkit;

namespace Admin.Areas.Admin.Controllers
{
    public class ErrorController : AdminBaseController
    {
        public ErrorController()
        {
            this.IgnoreSessionCheck = true;
        }

        public IActionResult Index(ErrorModel _ErrorModel)
        {
            return View(_ErrorModel);
        }

    }
}