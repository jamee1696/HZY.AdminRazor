using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Sys
{
    using HZY.Toolkit;
    using HZY.Models.Sys;
    using HZY.DTO;
    using HZY.DTO.Sys;
    using HZY.Services.Sys;

    /// <summary>
    /// 菜单管理
    /// </summary>
    public class MenusController : ApiBaseController
    {
        protected readonly Sys_MenuService service;
        protected readonly AccountService accountService;

        public MenusController(Sys_MenuService _service, AccountService _accountService)
        {
            this.service = _service;
            this.accountService = _accountService;
        }

        #region 页面 Views

        [HttpGet(nameof(Index))]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Info/{Id?}")]
        public IActionResult Info(Guid? Id, Guid? PId)
        {
            ViewData["PId"] = PId;
            return View(Id);
        }

        #endregion

        #region 基础 CURD

        /// <summary>
        /// 查询数据列表
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        [HttpPost("FindList/{Page}/{Rows}"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> FindListAsync(int Page, int Rows, [FromBody] Sys_Menu Search)
        {
            var tableVM = await this.service.FindListAsync(Page, Rows, Search);
            return this.ResultOk(tableVM);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("Save"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> SaveAsync([FromBody]Sys_MenuDto Model)
        {
            return this.ResultOk(await this.service.SaveAsync(Model));
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpPost("Delete"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> DeleteAsync([FromBody]List<Guid> Ids)
        {
            await this.service.DeleteAsync(Ids);
            return this.ResultOk();
        }

        /// <summary>
        /// 根据Id 加载表单数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost("LoadForm/{Id?}"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> LoadFormAsync(Guid Id)
        {
            return this.ResultOk(await this.service.LoadFormAsync(Id));
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取菜单列表 以及 页面按钮权限
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(SysTree)), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> SysTree()
        {
            var allList = await service.GetMenuByRoleIDAsync();

            return this.ResultOk(new
            {
                userName = this.accountService.info.UserName,
                list = this.service.CreateMenus(Guid.Empty, allList),
                allList = allList,
                powerState = await this.service.GetPowerState(allList)
            });
        }

        /// <summary>
        /// 获取菜单功能树
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(MenuFunctionTree)), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> MenuFunctionTree()
        {
            var menuFunctionTree = await this.service.GetMenuFunctionTreeAsync();

            return this.ResultOk(new
            {
                treeData = menuFunctionTree.Item1,
                defaultExpandedKeys = menuFunctionTree.Item2,
                defaultCheckedKeys = menuFunctionTree.Item3
            });
        }

        #endregion

    }
}