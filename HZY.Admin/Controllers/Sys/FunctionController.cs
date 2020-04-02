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
    using HZY.Services;
    using HZY.Services.Sys;

    /// <summary>
    /// 功能管理
    /// </summary>
    public class FunctionController : ApiBaseController
    {
        protected readonly Sys_FunctionService service;

        public FunctionController(Sys_FunctionService _service)
        {
            this.service = _service;
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
        public async Task<ApiResult> FindListAsync(int Page, int Rows, [FromBody] Sys_Function Search)
        {
            var tableVM = await this.service.FindListAsync(Page, Rows, Search);
            return this.ResultOk(tableVM);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("Save"), Core.HZYApiAuthorizationCheck, Core.HZYAppCheckModel]
        public async Task<ApiResult> SaveAsync([FromBody]Sys_Function Model)
        {
            return this.ResultOk(await this.service.SaveAsync(Model));
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        [HttpPost("Delete"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> DeleteAsync([FromBody] List<Guid> Ids)
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







    }
}