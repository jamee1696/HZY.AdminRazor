﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HZY.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(Member))]
    public class Member
    {
        [Key]
        public Guid Member_ID { get; set; } = Guid.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        public string Member_Num { get; set; }

        /// <summary>
        /// 会员名称
        /// </summary>
        public string Member_Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public int Member_Phone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Member_Sex { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Member_Birthday { get; set; } = DateTime.Now;

        /// <summary>
        /// 头像
        /// </summary>
        public string Member_Photo { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid Member_UserID { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Member_Introduce { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Member_FilePath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Member_CreateTime { get; set; }
    }

}