﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Areas.Admin.Controllers
{
    using DbFrame.DbContext.SqlServer;
    using Entitys;
    using Entitys.SysClass;
    using Logic;
    using Logic.SysClass;
    using Microsoft.Extensions.Configuration;
    using System.Text;
    using Toolkit;

    public class HomeController : AdminBaseController
    {
        Sys_MenuLogic _Logic = new Sys_MenuLogic();

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            StringBuilder _StringBuilder = new StringBuilder();
            _Logic.CreateMenus(Guid.Empty, await _Logic.GetMenuByRoleIDAsync(this._Account), _StringBuilder);
            ViewBag.Menus = _StringBuilder.ToString();
            ViewBag.UsreName = this._Account.UserName;
            return View();
        }

        [HttpGet]
        public IActionResult Main()
        {
            return View();
        }

        /// <summary>
        /// 保持连接
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Connetct()
        {
            return this.Success();
        }

        [HttpGet]
        public IActionResult Chat()
        {
            var _Ip = HttpContext.Connection.RemoteIpAddress.ToString();
            ViewBag.IP = _Ip;
            return View();
        }

        [HttpPost]
        public IActionResult ChatInit()
        {
            var _Sessions = WebSocketWork.GetAllAppSessions()?
                .OrderByDescending(w => w.LastActiveTime)
                .Select(w => w.HttpContext.Connection.RemoteIpAddress.ToString())
                .Distinct()
                .ToList();

            return Json(new
            {
                status = 1,
                oldMsg = WebSocketWork.OldMessages.ToStr(),
                userList = _Sessions
            });
        }


    }
}