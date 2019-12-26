using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entitys
{
    //
    using DbFrame.BaseClass;
    using Entitys.Attributes;

    [Table("Member")]
    public class Member : Class.BaseClass
    {

        [Field(IsKey = true)]
        public Guid Member_ID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [CSetNumber(0)]
        public string Member_Num { get; set; }

        /// <summary>
        /// 会员名称
        /// </summary>
        [CRequired(ErrorMessage = "{name}不能为空")]
        public string Member_Name { get; set; }

        /// <summary>
        /// 会员电话
        /// </summary>
        public int? Member_Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Member_Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Member_Birthday { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Member_Photo { get; set; }

        /// <summary>
        /// 帐户ID
        /// </summary>
        public Guid? Member_UserID { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Member_Introduce { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public string Member_FilePath { get; set; }

        [Field(IsIgnore = true)]
        public DateTime? Member_CreateTime { get; set; }


    }
}
