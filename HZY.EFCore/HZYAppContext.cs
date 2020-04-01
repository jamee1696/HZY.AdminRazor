using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.EFCore
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;
    using HZY.EFCore.Base;
    using HZY.EFCore.Repository;
    using HZY.Models;
    using HZY.Models.Sys;
    using System.Data;
    using Microsoft.Data.SqlClient;
    using System.Data.Common;
    using HZY.EFCore.Repository.Interface;
    using Microsoft.Extensions.Logging;
    using System.Collections;

    public class HZYAppContext : DbContext, IUnitOfWork
    {
        public HZYAppContext(DbContextOptions<HZYAppContext> options)
            : base(options)
        {

        }

        #region DbSet

        public DbSet<Sys_AppLog> Sys_AppLogs { get; set; }
        public DbSet<Sys_Function> Sys_Functions { get; set; }
        public DbSet<Sys_Menu> Sys_Menus { get; set; }
        public DbSet<Sys_MenuFunction> Sys_MenuFunctions { get; set; }
        public DbSet<Sys_Role> Sys_Roles { get; set; }
        public DbSet<Sys_RoleMenuFunction> Sys_RoleMenuFunctions { get; set; }
        public DbSet<Sys_User> Sys_Users { get; set; }
        public DbSet<Sys_UserRole> Sys_UserRoles { get; set; }

        //
        public DbSet<Member> Members { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Sys_AppLog>().ToTable("Sys_AppLog");
            //modelBuilder.Entity<User>().ToTable("User");

            #region 缓存表及属性信息
            var types = modelBuilder.Model.GetEntityTypes().Select(item => item.ClrType).ToList();
            ModelCache.Set(types);
            #endregion

        }

        #region IUnitOfWork

        public bool SaveState { get; set; } = true;

        public virtual void CommitOpen() => this.SaveState = false;

        public int Commit()
        {
            this.SaveState = true;
            return this.Save();
        }

        public Task<int> CommitAsync()
        {
            this.SaveState = true;
            return this.SaveAsync();
        }

        public int Save()
        {
            if (this.SaveState)
                return this.SaveChanges();
            else
                return 1;
        }

        public Task<int> SaveAsync()
        {
            if (this.SaveState)
                return this.SaveChangesAsync();
            else
                return Task.FromResult(1);
        }

        #endregion

        public async Task<TableViewModel> AsTableViewModelAsync<T>(IQueryable<T> query, int Page, int Rows, params Type[] EntityTypes)
        {
            var _tableViewModel = new TableViewModel();

            _tableViewModel.Page = Page;
            _tableViewModel.Rows = Rows;
            _tableViewModel.TotalCount = await query.CountAsync();
            _tableViewModel.TotalPage = (_tableViewModel.TotalCount / Rows);

            var Datas = await query.Skip((Page - 1) * Rows).Take(Rows).ToListAsync();

            var type = typeof(T);
            var fields = type.GetProperties();

            #region 组合一下 类型的属性和类型名称
            var TabAndField = new List<(string, string)>();
            foreach (var item in EntityTypes)
            {
                foreach (var prop in item.GetProperties())
                {
                    TabAndField.Add((prop.Name, item.Name));
                }
            }
            #endregion

            #region 填充 列 信息

            if (TabAndField.Count > 0)
            {
                foreach (var field in fields)
                {
                    var tableName = TabAndField.Find(w => w.Item1 == field.Name).Item2;

                    var title = string.Empty;

                    if (!string.IsNullOrWhiteSpace(tableName))
                    {
                        var modelInfos = ModelCache.GetModelInfos(tableName);
                        title = modelInfos.FirstOrDefault(w => w.Name == field.Name)?.Remark;
                    }

                    _tableViewModel.Cols.Add(new TableViewCol()
                    {
                        DataIndex = field.Name,
                        Show = field.Name != "_ukid",
                        Title = string.IsNullOrWhiteSpace(title) ? field.Name : title
                    });
                }
            }

            #endregion

            #region 和数据转换为 list Hashtable
            foreach (var item in Datas)
            {
                var hashTable = new Hashtable();
                foreach (var field in fields) hashTable[field.Name] = field.GetValue(item);
                _tableViewModel.DataSource.Add(hashTable);
            }
            #endregion

            return _tableViewModel;
        }

        /// <summary>
        /// 根据表名称 获取列
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public async Task<List<TABLES_COLUMNS>> GetColsByTableNameAsync(string TableName)
        {
            // SELECT 
            //     表名       = case when a.colorder=1 then d.name else '' end,
            //     表说明     = case when a.colorder=1 then isnull(f.value,'') else '' end,
            //     字段序号   = a.colorder,
            //     字段名     = a.name,
            //     标识       = case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '√'else '' end,
            //     主键       = case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
            //                      SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then '√' else '' end,
            //     类型       = b.name,
            //     占用字节数 = a.length,
            //     长度       = COLUMNPROPERTY(a.id,a.name,'PRECISION'),
            //     小数位数   = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0),
            //     允许空     = case when a.isnullable=1 then '√'else '' end,
            //     默认值     = isnull(e.text,''),
            //     字段说明   = isnull(g.[value],'')
            // FROM 
            //     syscolumns a
            // left join 
            //     systypes b 
            // on 
            //     a.xusertype=b.xusertype
            // inner join 
            //     sysobjects d 
            // on 
            //     a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
            // left join 
            //     syscomments e 
            // on 
            //     a.cdefault=e.id
            // left join 
            // sys.extended_properties   g 
            // on 
            //     a.id=G.major_id and a.colid=g.minor_id  
            // left join
            // sys.extended_properties f
            // on 
            //     d.id=f.major_id and f.minor_id=0
            // where  
            //     d.name='member' 
            // order by  
            //     a.id,a.colorder

            string SqlString = $@"
SELECT 
    d.name AS TabName,
	isnull(f.value,'') AS TabNameRemark,
	a.colorder AS ColOrder,
	a.name AS ColName,
	case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then 1 else 0 end ColIsIdentity,
	case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in (
                     SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then 1 else 0 end ColIsKey,
	b.name AS ColType,
	a.length AS ColLength,
	COLUMNPROPERTY(a.id,a.name,'PRECISION') AS ColMaxLength,
	isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0) AS ColScale,
	case when a.isnullable=1 then 1 else 0 end ColIsNull,
	isnull(e.text,'') AS ColDefaultValue,
    isnull(g.[value],'') AS ColRemark
FROM syscolumns a
left join systypes b on a.xusertype=b.xusertype
inner join sysobjects d on a.id=d.id  and d.xtype='U' and  d.name<>'dtproperties'
left join syscomments e on a.cdefault=e.id
left join sys.extended_properties g on a.id=G.major_id and a.colid=g.minor_id  
left join sys.extended_properties f on d.id=f.major_id and f.minor_id=0
where d.name=@TableName
order by a.id,a.colorder
";

            var dbParameter = new SqlParameter("TableName", TableName);

            return await this.AsListAsync<TABLES_COLUMNS>(SqlString, dbParameter);
        }

        /// <summary>
        /// 获取所有的表名称
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAllTableAsync()
        {
            string SqlString = @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES order by TABLE_NAME";

            return (await this.AsListAsync<string>(SqlString));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Sql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public async Task<List<T>> AsListAsync<T>(string Sql, params DbParameter[] sqlParameter)
        {
            var list = new List<T>();

            using (var conn = (SqlConnection)this.Database.GetDbConnection())
            {
                if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

                var command = new SqlCommand(Sql, conn);
                command.Parameters.Add(sqlParameter);
                var sqlDataReader = await command.ExecuteReaderAsync();

                List<string> field = new List<string>(sqlDataReader.FieldCount);
                for (int i = 0; i < sqlDataReader.FieldCount; i++)
                {
                    field.Add(sqlDataReader.GetName(i));
                }

                if (typeof(T).IsValueType)
                {
                    while (await sqlDataReader.ReadAsync())
                    {
                        var model = HZYEFCoreExtensions.CreateInstance<T>();

                        var propertyInfos = model.GetType().GetPropertyInfos();

                        foreach (var item in propertyInfos)
                        {
                            if (!field.Contains(item.Name)) continue;
                            item.SetValue(model, sqlDataReader[item.Name]);
                        }
                        list.Add(model);
                    }
                }
                else
                {
                    while (await sqlDataReader.ReadAsync())
                    {
                        list.Add((T)sqlDataReader[0]);
                    }
                }

                await sqlDataReader.CloseAsync();
                await conn.CloseAsync();
            }

            return list;
        }


    }
}
