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
        /// ���
        /// </summary>
        [CSetNumber(0)]
        public string Member_Num { get; set; }

        /// <summary>
        /// ��Ա����
        /// </summary>
        [CRequired(ErrorMessage = "{name}����Ϊ��")]
        public string Member_Name { get; set; }

        /// <summary>
        /// ��Ա�绰
        /// </summary>
        public int? Member_Phone { get; set; }

        /// <summary>
        /// �Ա�
        /// </summary>
        public string Member_Sex { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public DateTime? Member_Birthday { get; set; }

        /// <summary>
        /// ͷ��
        /// </summary>
        public string Member_Photo { get; set; }

        /// <summary>
        /// �ʻ�ID
        /// </summary>
        public Guid? Member_UserID { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Member_Introduce { get; set; }

        /// <summary>
        /// �ļ�
        /// </summary>
        public string Member_FilePath { get; set; }

        [Field(IsIgnore = true)]
        public DateTime? Member_CreateTime { get; set; }


    }
}
