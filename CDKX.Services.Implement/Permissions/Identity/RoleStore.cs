﻿// -----------------------------------------------------------------------
//  <copyright file="RoleStore.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2015-08-03 11:48</last-date>
// -----------------------------------------------------------------------

using CDKX.Services.Core.Models.Identity;
using OSharp.Core.Identity;

namespace CDKX.Services.Implement.Permissions.Identity
{
    /// <summary>
    /// 角色存储实现
    /// </summary>
    public class RoleStore : RoleStoreBase<SysRole, int>
    { }
}