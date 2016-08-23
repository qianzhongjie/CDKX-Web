using System;
using System.ComponentModel;
using CDKX.Services.Core.Models.Identity;
using OSharp.Core.Data;
using OSharp.Utility.Develop.T4;
using System.Collections.Generic;
using System.Configuration;

namespace CDKX.Services.Core.Models.User
{
    [Generate]
    [Description("用户-用户信息")]
    public class UserInfo : EntityBase<int>
    {

        private string headPic;
        [Description("头像地址")]
        public string HeadPic
        {
            get
            {
                if (string.IsNullOrEmpty(this.headPic))
                {
                    try
                    {
                        return ConfigurationManager.AppSettings["ServerHost"] + "Content/images/defaultPic.png";
                    }
                    catch
                    {
                    }
                    return "";
                }
                else
                {
                    return this.headPic;
                }
            }
            set
            {
                this.headPic = value;
            }
        }

        [Description("真实姓名")]
        public string RealName { get; set; }

        [Description("设备唯一标识号")]
        public string RegistKey { get; set; }

        [Description("性别")]
        public Sex Sex { get; set; }

        [Description("系统用户")]
        public virtual SysUser SysUser { get; set; }

        // [Description("生日")]
        public DateTime? Birthday { get; set; }

        [Description("qq")]
        public string Qq { get; set; }

        /// <summary>
        /// 登录凭据
        /// </summary>
        public string Token { get; set; }
    }

    public enum Sex
    {
        不限 = 0,
        男 = 1,
        女 = 2
    }
}
