using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Sys
{
    using HZY.Toolkit;
    using HZY.Models.Sys;
    using HZY.DTO;
    using HZY.DTO.Sys;
    using HZY.Services.Sys;

    public class UserController : ApiBaseController
    {
        protected readonly Sys_UserService service;
        protected readonly AccountService accountService;

        public UserController(
            Sys_MenuService _menuService,
            Sys_UserService _service,
            AccountService _accountService)
            : base(_menuService)
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
        public IActionResult Info(Guid Id)
        {
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
        public async Task<ApiResult> FindListAsync(int Page, int Rows, [FromBody]Sys_User Search)
        {
            var tableVM = await this.service.FindListAsync(Page, Rows, Search);
            return this.ResultOk(tableVM);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("Save"), Core.HZYApiAuthorizationCheck, Core.HZYAppCheckModel]
        public async Task<ApiResult> SaveAsync(Sys_UserDto Model)
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

        #region 导出 Excel

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="Search"></param>
        /// <returns></returns>
        [HttpPost("ExportExcel"), Core.HZYApiAuthorizationCheck]
        public async Task<FileContentResult> ExportExcel([FromBody] Sys_User Search)
        {
            var bytes = await this.service.ExportExcel(Search);
            return File(bytes, Tools.GetFileContentType[".xls"].ToStr(), $"{Guid.NewGuid()}.xls");
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="MenuId"></param>
        /// <returns></returns>
        [HttpPost("GetPowerState/{MenuId}"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> GetPowerState(Guid MenuId)
        {
            if (MenuId == Guid.Empty) throw new MessageBox("参数MenuId不能为空!");
            return this.ResultOk(new { powerState = await this.menuService.GetPowerStateByMenuId(MenuId) });
        }

        #endregion

    }
}