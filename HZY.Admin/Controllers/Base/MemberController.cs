using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Base
{
    using HZY.Toolkit;
    using HZY.Models.Sys;
    using HZY.DTO;
    using HZY.DTO.Sys;
    using HZY.Services;
    using HZY.Services.Sys;
    using HZY.Models;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// 会员管理
    /// </summary>
    public class MemberController : ApiBaseController
    {


        protected readonly MemberService service;
        protected readonly IWebHostEnvironment webHostEnvironment;
        protected readonly string webRootPath;

        public MemberController(MemberService _service, IWebHostEnvironment _webHostEnvironment)
        {
            this.service = _service;
            this.webHostEnvironment = _webHostEnvironment;
            this.webRootPath = _webHostEnvironment.WebRootPath;
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
        public async Task<ApiResult> FindListAsync(int Page, int Rows, [FromBody] Member Search)
        {
            var tableVM = await this.service.FindListAsync(Page, Rows, Search);
            return this.ResultOk(tableVM);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("Save"), Core.HZYApiAuthorizationCheck, Core.HZYAppCheckModel]
        public async Task<ApiResult> SaveAsync([FromForm]Member Model)
        {
            var Files = new List<IFormFile>();
            IFormFile Photo = null;
            if (Request.Form.Files.Count > 0)
            {
                Files = Request.Form.Files.Where(w => w.Name.Contains("Files")).ToList();
                Photo = Request.Form.Files.FirstOrDefault(w => w.Name == "Photo");
            }
            return this.ResultOk(await this.service.SaveAsync(Model, this.webRootPath, Photo, Files));
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



    }
}