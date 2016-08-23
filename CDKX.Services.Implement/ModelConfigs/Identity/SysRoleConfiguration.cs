// -----------------------------------------------------------------------
//  <copyright file="RoleConfiguration.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2015-03-24 17:02</last-date>
// -----------------------------------------------------------------------

using CDKX.Services.Core.Models.Identity;
using OSharp.Data.Entity;

namespace CDKX.Services.Implement.ModelConfigs.Identity
{
    public class SysRoleConfiguration : EntityConfigurationBase<SysRole, int>
    {
        public SysRoleConfiguration()
        {
            HasOptional(m => m.Organization);
        }
    }
}