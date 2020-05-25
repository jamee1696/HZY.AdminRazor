using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Base
{
    using HZY.Admin.Hubs;
    using HZY.Admin.Services.Sys;
    using Microsoft.AspNetCore.SignalR;

    public class SignalRChatController : ApiBaseController
    {
        public readonly IHubContext<ChatHub> chatHub;
        public SignalRChatController(IHubContext<ChatHub> _chatHub, Sys_MenuService menuService)
            : base("", menuService)
        {
            this.chatHub = _chatHub;
        }

        [HttpGet(nameof(Index))]
        public IActionResult Index() => View();












    }
}