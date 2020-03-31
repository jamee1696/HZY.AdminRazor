using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HZY.EFCore.Repository
{
    using HZY.EFCore.Repository.Interface;

    public class DefaultRepository<T> : HZYRepositoryBase<T, HZYAppContext>
        where T : class, new()
    {

        public DefaultRepository(HZYAppContext _context) : base(_context) { }


    }
}
