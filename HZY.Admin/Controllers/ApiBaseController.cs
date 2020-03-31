using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers
{
    using HZY.Admin.Core;
    using HZY.Toolkit;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using System.IO;

    [ApiController]
    //[Microsoft.AspNetCore.Authorization.Authorize]
    [ApiExplorerSettings(GroupName = nameof(ApiVersionsEnum.Admin)), Core.HZYApiAuthorizationCheck]
    public class ApiBaseController : BaseController
    {
        

    }
}