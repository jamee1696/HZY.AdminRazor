using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HZY.Admin.Controllers.Sys
{
    using HZY.Models.Sys;
    using HZY.DTO;
    using HZY.DTO.Sys;
    using Microsoft.AspNetCore.Hosting;
    using HZY.Toolkit;
    using System.Text;
    using System.IO;
    using HZY.Services.Sys;
    using HZY.EFCore.Repository;
    using HZY.EFCore;

    /// <summary>
    /// 代码创建 工具
    /// </summary>
    public class CCTController : ApiBaseController
    {
        private string _WebRootPath { get; } = string.Empty;
        protected readonly CCTService service;
        protected readonly HZYAppContext db;

        public CCTController(
            CCTService _service,
            HZYAppContext _db,
            IWebHostEnvironment IWebHostEnvironment)
        {
            this.service = _service;
            this.db = _db;
            this._WebRootPath = IWebHostEnvironment.WebRootPath;
        }

        #region 页面 Views

        [HttpGet(nameof(Index))]
        public IActionResult Index()
        {
            return View();
        }

        #endregion

        /// <summary>
        /// 获取所有的 表名 及对应的 字段
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(GetTableNameAndFields))]
        public async Task<ApiResult> GetTableNameAndFields()
        {
            return this.ResultOk(await this.service.GetTableNameAndFields());
        }

        /// <summary>
        /// 获取 Model 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetModelCode) + "/{TableName}")]
        public async Task<ApiResult> GetModelCode([FromRoute]string TableName)
        {
            var TempUrl = _WebRootPath + "/Content/CodeTemp/Model.txt";

            if (!System.IO.File.Exists(TempUrl))
                throw new MessageBox("模板文件不存在");

            return this.ResultOk(data: await this.service.CreateModelCode(TableName, await System.IO.File.ReadAllTextAsync(TempUrl, Encoding.UTF8)));
        }

        /// <summary>
        /// 获取 注册 数据库 表 代码
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(GetDbSetCode))]
        public async Task<ApiResult> GetDbSetCode() => this.ResultOk(data: await this.service.CreateDbSetCode());

        /// <summary>
        /// 获取 Logic 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetLogicCode) + "/{TableName}")]
        public async Task<ApiResult> GetLogicCode([FromRoute]string TableName)
        {
            var TempUrl = _WebRootPath + "/Content/CodeTemp/Logic.txt";

            if (!System.IO.File.Exists(TempUrl))
                throw new MessageBox("模板文件不存在");

            return this.ResultOk(data: await this.service.CreateLogicCode(TableName, await System.IO.File.ReadAllTextAsync(TempUrl, Encoding.UTF8)));
        }

        /// <summary>
        /// 获取 Logic 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetControllersCode) + "/{TableName}")]
        public async Task<ApiResult> GetControllersCode([FromRoute]string TableName)
        {
            var TempUrl = _WebRootPath + "/Content/CodeTemp/Controllers.txt";

            if (!System.IO.File.Exists(TempUrl))
                throw new MessageBox("模板文件不存在");

            return this.ResultOk(data: await this.service.CreateControllersCode(TableName, await System.IO.File.ReadAllTextAsync(TempUrl, Encoding.UTF8)));
        }

        /// <summary>
        /// 获取 Form 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Fields"></param>
        /// <returns></returns>
        [HttpPost(nameof(GetFormCode) + "/{FromRoute}")]
        public async Task<ApiResult> GetFormCode([FromRoute]string TableName, [FromBody]List<string> Fields)
        {
            var TempUrl = _WebRootPath + "/Content/CodeTemp/Form.txt";

            if (!System.IO.File.Exists(TempUrl))
                throw new MessageBox("模板文件不存在");

            if (Fields?.Count == 0)
            {
                Fields = new List<string>();
                var _Cols = await this.db.GetColsByTableNameAsync(TableName);
                foreach (var _Col in _Cols)
                {
                    Fields.Add($"{TableName}/{_Col.ColName}");
                }
            }

            return this.ResultOk(data: await this.service.CreateFormCode(Fields, await System.IO.File.ReadAllTextAsync(TempUrl, Encoding.UTF8)));
        }

        /// <summary>
        /// 下载当前代码
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(Download))]
        public IActionResult Download(string TableName, string CodeType, string Content)
        {
            var Suffix = string.Empty;
            // var FileType = ".cs";

            if (CodeType == "Model") Suffix = ".cs";

            if (CodeType == "Logic") Suffix = "Logic.cs";

            if (CodeType == "Controller") Suffix = "Controller.cs";

            if (CodeType == "Form")
            {
                Suffix = ".vue";
                // FileType = ".vue";
            }

            var _Bytes = Encoding.UTF8.GetBytes(Content);
            return File(_Bytes, Tools.GetFileContentType[".cs"], $"{TableName}{Suffix}");
        }

        /// <summary>
        /// 下载所有 代码
        /// </summary>
        /// <param name="CodeType">代码类型</param>
        /// <returns></returns>
        [HttpGet(nameof(DownloadAll) + "/{CodeType}")]
        public async Task<IActionResult> DownloadAll([FromRoute]string CodeType)
        {
            var Suffix = string.Empty;
            var Temp = string.Empty;
            var TempUrl = string.Empty;

            if (CodeType == "Model")
            {
                Suffix = ".cs";
                TempUrl = _WebRootPath + "/Content/CodeTemp/Model.txt";
            }

            if (CodeType == "Logic")
            {
                Suffix = "Logic.cs";
                TempUrl = _WebRootPath + "/Content/CodeTemp/Logic.txt";
            }

            if (CodeType == "Controller")
            {
                Suffix = "Controller.cs";
                TempUrl = _WebRootPath + "/Content/CodeTemp/Controllers.txt";
            }

            if (CodeType == "Form")
            {
                Suffix = ".vue";
                TempUrl = _WebRootPath + "/Content/CodeTemp/Form.txt";
            }

            Temp = await System.IO.File.ReadAllTextAsync(TempUrl, Encoding.UTF8);

            //List<StringBuilder> _Codes = new List<StringBuilder>();
            Dictionary<string, Stream> _DicStream = new Dictionary<string, Stream>();

            var _TableNames = await this.db.GetAllTableAsync();
            foreach (var item in _TableNames)
            {
                StringBuilder _StringBuilder = new StringBuilder();

                if (CodeType == "Model")
                {
                    _StringBuilder.Append(await this.service.CreateModelCode(item, Temp));
                }

                if (CodeType == "Logic")
                {
                    _StringBuilder.Append(await this.service.CreateLogicCode(item, Temp));
                }

                if (CodeType == "Controller")
                {
                    _StringBuilder.Append(await this.service.CreateControllersCode(item, Temp));
                }

                if (CodeType == "Form")
                {
                    //获取表下面的所有 字段
                    var _Cols = await this.db.GetColsByTableNameAsync(item);
                    var list = new List<string>();
                    foreach (var _Col in _Cols)
                    {
                        list.Add($"{item}/{_Col.ColName}");
                    }
                    _StringBuilder.Append(await this.service.CreateFormCode(list, Temp));
                }

                //_Codes.Add(_StringBuilder);
                _DicStream[$"{item}{Suffix}"] = new MemoryStream(Encoding.UTF8.GetBytes(_StringBuilder.ToString()));
            }

            var _Bytes = new byte[] { };

            await Task.Run(() =>
            {
                _Bytes = Tools.PackageManyZip(_DicStream);
            });

            return File(_Bytes, Tools.GetFileContentType[".zip"], $"{CodeType}.zip");
        }




    }
}
