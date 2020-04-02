using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Services.Sys
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Microsoft.AspNetCore.Http;
    using HZY.EFCore.Repository;
    using HZY.Models.Sys;
    using HZY.Toolkit;
    using System.Linq;
    using HZY.EFCore.Base;
    using HZY.Services.Core;
    using HZY.EFCore;
    using NPOI.SS.Formula.Functions;

    public class Sys_FunctionService : ServiceBase
    {
        protected readonly EFCoreContext db;
        protected readonly DefaultRepository<Sys_Function> functionDb;

        public Sys_FunctionService(
            EFCoreContext _db,
            DefaultRepository<Sys_Function> _functionDb
            )
        {
            this.db = _db;
            this.functionDb = _functionDb;
        }


        #region CURD 基础

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        public async Task<TableViewModel> FindListAsync(int Page, int Rows, Sys_Function Search)
        {
            var query = functionDb.Query()
                .WhereIF(w => w.Function_Name.Contains(Search.Function_Name), !string.IsNullOrWhiteSpace(Search?.Function_Name))
                .OrderBy(w => w.Function_Num)
                .Select(w => new
                {
                    w.Function_Num,
                    w.Function_Name,
                    w.Function_ByName,
                    Function_CreateTime = w.Function_CreateTime.ToString("yyyy-MM-dd"),
                    _ukid = w.Function_ID
                })
                ;

            return await this.db.AsTableViewModelAsync(query, Page, Rows, typeof(Sys_Function));
        }

        /// <summary>
        /// 新增\修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Guid> SaveAsync(Sys_Function model)
        {
            await functionDb.InsertOrUpdateAsync(model);

            return model.Function_ID;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>
        public async Task DeleteAsync(List<Guid> Ids)
            => await functionDb.DeleteAsync(w => Ids.Contains(w.Function_ID));

        /// <summary>
        /// 加载表单 数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<object> LoadFormAsync(Guid Id)
        {
            var res = new Dictionary<string, object>();

            var Model = await functionDb.FindByIdAsync(Id);

            res[nameof(Id)] = Id;
            res[nameof(Model)] = Model.ToNewByNull();

            return res;
        }


        #endregion





    }
}
