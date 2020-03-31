using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.EFCore.Repository.Interface
{
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        bool SaveState { get; set; }

        #region 设置保存、不保存
        void CommitOpen();
        int Commit();

        Task<int> CommitAsync();
        #endregion

        int Save();
        Task<int> SaveAsync();













    }
}
