using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Services.Sys
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;
    using HZY.EFCore.Repository;
    using HZY.Services.Core;
    using HZY.EFCore;
    using System.Linq;

    public class CCTService : ServiceBase
    {

        protected readonly EFCoreContext db;

        public CCTService(EFCoreContext _db)
        {
            this.db = _db;

        }

        /// <summary>
        /// 获取表名和字段
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, List<TABLES_COLUMNS>>> GetTableNameAndFields()
        {
            var _TableNames = await db.GetAllTableAsync();

            var dic = new Dictionary<string, List<TABLES_COLUMNS>>();
            var _TableAll = ModelCache.All();

            for (int i = 0; i < _TableNames.Count; i++)
            {
                var item = _TableNames[i];
                var _Cols = await db.GetColsByTableNameAsync(item.Name);

                var fieldInfoList = _TableAll[item.Name].ToList();
                if (fieldInfoList == null) continue;

                foreach (var _Col in _Cols)
                {
                    var _FieldDescribe = fieldInfoList.FirstOrDefault(w => w.Name == _Col.ColName);
                    if (_FieldDescribe != null) _Col.ColRemark = _FieldDescribe.Remark;
                }
                dic[item.Name] = _Cols;
            }

            return dic;
        }

        /// <summary>
        /// 创建 Model 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public async Task<string> CreateModelCode(string TableName, string Temp)
        {
            var _Cols = await db.GetColsByTableNameAsync(TableName);

            var _TableAll = ModelCache.All();
            var fieldInfoList = _TableAll[TableName].ToList();

            var _Code = Temp.ToString();
            var _ClassName = TableName;
            var _Fields = new StringBuilder();

            foreach (var item in _Cols)
            {
                var _Type = string.Empty;
                var _Key = item.ColIsKey;
                switch (item.ColType)
                {
                    case "uniqueidentifier":
                        _Type = _Key == 1 ? "Guid" : "Guid?";
                        break;
                    case "bit":
                    case "int":
                        _Type = _Key == 1 ? "int" : "int?";
                        break;
                    case "datetime":
                        _Type = "DateTime?";
                        break;
                    case "float":
                        _Type = "float?";
                        break;
                    case "money":
                        _Type = "double?";
                        break;
                    case "decimal":
                        _Type = "decimal?";
                        break;
                    default:
                        _Type = "string";
                        break;
                }

                if (_Key == 1)
                {
                    _Fields.Append($"\t[Key]\r\n");
                }
                else
                {
                    if (item.ColName.Contains("_CreateTime") && _Type == "DateTime?")
                    {
                        _Fields.Append($@"
        /// <summary>
        /// 创建时间
        /// </summary>
");
                        _Fields.Append("\t[DatabaseGenerated(DatabaseGeneratedOption.Computed)]\r\n");
                    }
                    else
                    {
                        if (fieldInfoList == null)
                        {
                            if (string.IsNullOrWhiteSpace(item.ColRemark))
                            {
                                _Fields.Append($@"
        /// <summary>
        /// 请设置 {item.ColName} 的显示名称 => 备注:{item.ColName}
        /// </summary>
");
                            }
                            else
                            {
                                _Fields.Append($@"
        /// <summary>
        /// {item.ColRemark} => 备注:{item.ColName}
        /// </summary>
");
                            }
                        }
                        else
                        {
                            var _FieldDescribe = fieldInfoList.FirstOrDefault(w => w.Name == item.ColName);
                            if (_FieldDescribe == null)
                            {
                                _Fields.Append($@"
        /// <summary>
        /// 请设置 {item.ColName} 的显示名称 => 备注:{item.ColName}
        /// </summary>
");
                            }
                            else
                            {
                                _Fields.Append($@"
        /// <summary>
        /// {_FieldDescribe.Remark} => 备注:{item.ColName}
        /// </summary>
");
                            }

                        }

                    }
                }

                _Fields.Append($"\tpublic {_Type} {item.ColName} {{ get; set; }}\r\n");
            }

            _Code = _Code.Replace("<#ClassName#>", _ClassName);
            _Code = _Code.Replace("<#TableName#>", TableName);
            _Code = _Code.Replace("<#Fields#>", _Fields.ToString());

            return _Code.ToString();
        }

        /// <summary>
        /// 创建 Logic 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Temp"></param>
        /// <returns></returns>
        public async Task<string> CreateServiceCode(string TableName, string Temp)
        {
            var _Cols = await db.GetColsByTableNameAsync(TableName);

            var _Code = Temp.ToString();
            var _ClassName = TableName;
            var _KeyName = _Cols.Find(w => w.ColIsKey == 1);
            //
            var _NameCol = _Cols.Count > 2 ? _Cols[1] : null;
            var _Name = _NameCol == null ? "" : _NameCol.ColName;

            var _Select = _Cols.FindAll(w => w.ColIsKey == 0);

            //var _QueryCode = new StringBuilder().Append($@"
            //    var query = db.Query<{TableName}>()
            //        .Where(w => w.t1.Role_Name.Contains(Search.{_Name}), !string.IsNullOrWhiteSpace(Search.{_Name}))
            //        .Select(w => new
            //        {{
            //            {(_Select == null ? "" : "w.t1." + string.Join(",w.t1.", _Select.Select(w => w.ColName)))},
            //            _ukid = w.t1.{_KeyName.ColName}
            //        }})
            //        .TakePage(Page, Rows, out int TotalCount)
            //        ;
            //");



            var _SelectString = $@"{(_Select == null ? "" : "w.t1." + string.Join(",w.t1.", _Select.Select(w => w.ColName)))},
                        _ukid = w.t1.{ _KeyName.ColName}
            ";

            _Code = _Code.Replace("<#Select#>", _SelectString);
            _Code = _Code.Replace("<#ClassName#>", _ClassName);
            _Code = _Code.Replace("<#className#>", _ClassName.First().ToString().ToLower() + _ClassName.Substring(1));
            _Code = _Code.Replace("<#TableName#>", TableName);
            _Code = _Code.Replace("<#KeyName#>", _KeyName.ColName);
            //_Code = _Code.Replace("<#QueryCode#>", _QueryCode.ToString());

            return _Code.ToString();
        }

        /// <summary>
        /// Services Register
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateServicesRegister()
        {
            StringBuilder _StringBuilder = new StringBuilder();
            var _TableNames = await db.GetAllTableAsync();
            foreach (var item in _TableNames)
            {
                _StringBuilder.Append($@"services.AddScoped<{item.Name}Service>();
");
            }
            return _StringBuilder.ToString();
        }

        /// <summary>
        /// 创建 Controllers 代码
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Temp"></param>
        /// <returns></returns>
        public async Task<string> CreateControllersCode(string TableName, string Temp)
        {
            //var _Cols = await this.GetColsByTableName(TableName);
            var _Code = Temp.ToString();

            await Task.Run(() =>
            {
                var _ClassName = TableName + "Controller";

                _Code = _Code.Replace("<#ClassName#>", _ClassName);
                _Code = _Code.Replace("<#TableName#>", TableName);
            });

            return _Code.ToString();
        }

        /// <summary>
        /// 生产 DbSet 代码
        /// </summary>
        /// <returns></returns>
        public async Task<string> CreateDbSetCode()
        {
            StringBuilder _StringBuilder = new StringBuilder();
            var _TableNames = await db.GetAllTableAsync();
            foreach (var item in _TableNames)
            {
                _StringBuilder.Append($@"public DbSet<{item.Name}> {item.Name}s {{ get; set; }}
");
            }
            return _StringBuilder.ToString();
        }

        /// <summary>
        /// 创建 Form 代码
        /// </summary>
        /// <param name="Fields"></param>
        /// <param name="Temp"></param>
        /// <returns></returns>
        public async Task<string> CreateFormCode(List<string> Fields, string Temp)
        {
            StringBuilder _Codes = new StringBuilder();
            await Task.Run(() =>
            {
                var _TableAll = ModelCache.All();
                foreach (var item in Fields)
                {
                    var _TableName = item.Split('/')[0];
                    var _FieldName = item.Split('/')[1];
                    var fieldInfos = _TableAll[item].ToList();

                    if (fieldInfos == null) continue;

                    var fieldInfo = fieldInfos.FirstOrDefault(w => w.Name == _FieldName);
                    if (fieldInfo == null) continue;

                    var _Code = Temp;
                    _Code = _Code.Replace("<#FieldAlias#>", fieldInfo.Remark);
                    _Code = _Code.Replace("<#Field#>", fieldInfo.Name);
                    _Codes.Append(_Code + "\r\n");
                }
            });
            return _Codes.ToString();
        }
















    }
}
