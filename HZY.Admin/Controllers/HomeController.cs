using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers
{
    using HZY.Admin.Core;
    using HZY.EFCore.Repository;
    using HZY.Services.Sys;

    //该标记用于 忽略添加到 swagger 接口文档中
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : ApiBaseController
    {
        protected readonly AccountService accountService;

        public HomeController(
            Sys_MenuService _menuService,
            AccountService _accountService
            )
            : base(_menuService)
        {
            this.accountService = _accountService;
        }

        [HttpGet(nameof(Index)), Route(""), Route("/")]
        public async Task<IActionResult> Index()
        {
            var allList = await this.menuService.GetMenuByRoleIDAsync();

            ViewData["menuList"] = this.menuService.CreateMenus(Guid.Empty, allList);
            ViewData["UserName"] = this.accountService.info.UserName;

            return View();
        }

        [HttpGet(nameof(Main))]
        public IActionResult Main()
        {
            return View();
        }

    }
}
