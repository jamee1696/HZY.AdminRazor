using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Toolkits.HzyNetCoreUtil.Attributes
{
    /// <summary>
    /// 用户 事务 标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AppTransactionAttribute : Attribute
    {


    }
}
