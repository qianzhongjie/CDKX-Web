// -----------------------------------------------------------------------
//  <copyright file="UserLogin.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2015-06-25 14:39</last-date>
// -----------------------------------------------------------------------

using System.ComponentModel;
using OSharp.Core.Identity.Models;
using CDKX.Services.Core.Models.User;

namespace CDKX.Services.Core.Models.Identity
{
    /// <summary>
    /// 实体类——用户第三方登录（OAuth，如facebook,google）信息
    /// </summary>
    [Description("认证-第三方登录")]
    public class SysUserLogin : UserLoginBase<int, SysUser, int>
    {
        [Description("三方类型")]
        public ThirdProvider ThridProvider { get; set; }
    }

    /// <summary>
    /// 三方类型
    /// </summary>
    public enum ThirdProvider
    {
        QQ = 0,
        微信 = 1,
        新浪微博 = 2
    }
}