﻿using HZY.Services.Core;
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

    public class Sys_AppLogService : ServiceBase
    {
        protected readonly EFCoreContext db;
        protected readonly DefaultRepository<Sys_AppLog> appLogDb;
        protected readonly DefaultRepository<Sys_User> userDb;

        public Sys_AppLogService(
            EFCoreContext _db,
            DefaultRepository<Sys_AppLog> _appLogDb,
            DefaultRepository<Sys_User> _userDb
            )
        {
            this.db = _db;
            this.appLogDb = _appLogDb;
            this.userDb = _userDb;

        }

        #region CURD 基础

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <param name="Search"></param>
        /// <returns></returns>
        public async Task<TableViewModel> FindListAsync(int Page, int Rows, Sys_AppLog Search)
        {
            var query = (
                    from appLog in db.Sys_AppLogs
                    //左连接
                    from user in db.Sys_Users.Where(user => user.User_ID == appLog.AppLog_UserID).DefaultIfEmpty()
                    select new { appLog, user.User_Name }
                )
                .WhereIF(w => w.appLog.AppLog_Api.Contains(Search.AppLog_Api), !string.IsNullOrWhiteSpace(Search?.AppLog_Api))
                .WhereIF(w => w.appLog.AppLog_IP.Contains(Search.AppLog_IP), !string.IsNullOrWhiteSpace(Search?.AppLog_IP))
                .OrderByDescending(w => w.appLog.AppLog_CreateTime)
                .Select(w => new
                {
                    w.appLog.AppLog_IP,
                    w.appLog.AppLog_Api,
                    w.appLog.AppLog_Form,
                    w.appLog.AppLog_FormBody,
                    w.appLog.AppLog_QueryString,
                    操作人 = w.User_Name,
                    AppLog_CreateTime = w.appLog.AppLog_CreateTime.ToString("yyyy-MM-dd"),
                    _ukid = w.appLog.AppLog_ID
                })
                ;

            return await this.db.AsTableViewModelAsync(query, Page, Rows,typeof(Sys_AppLog));
        }

        /// <summary>
        /// 新增\修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Guid> SaveAsync(Sys_AppLog model)
        {
            await appLogDb.InsertOrUpdateAsync(model);

            return model.AppLog_ID;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>
        public async Task DeleteAsync(List<Guid> Ids)
            => await appLogDb.DeleteAsync(w => Ids.Contains(w.AppLog_ID));

        /// <summary>
        /// 加载表单 数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<object> LoadFormAsync(Guid Id)
        {
            var res = new Dictionary<string, object>();

            var Model = await appLogDb.FindByIdAsync(Id);
            var _Sys_User = await userDb.FindByIdAsync(Model.AppLog_UserID);

            res[nameof(Id)] = Id;
            res[nameof(Model)] = Model.ToNewByNull();
            res[nameof(_Sys_User.User_Name)] = _Sys_User.User_Name;

            return res;
        }


        #endregion










    }
}
