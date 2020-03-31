﻿using System;
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

    public class ChangePasswordController : ApiBaseController
    {
        protected readonly AccountService service;

        public ChangePasswordController(AccountService _service)
        {
            this.service = _service;
        }

        #region 页面 Views

        [HttpGet(nameof(Index))]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost("Save"), Core.HZYApiAuthorizationCheck]
        public async Task<ApiResult> UpdatePassword([FromBody]UpdatePasswordDto Model)
        {
            await this.service.ChangePwd(Model.OldPwd, Model.NewPwd);
            return this.ResultOk();
        }


    }
}