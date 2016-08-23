// -----------------------------------------------------------------------
//  <copyright file="UserClaimConfiguration.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2015-06-25 14:42</last-date>
// -----------------------------------------------------------------------

using CDKX.Services.Core.Models.Identity;
using OSharp.Data.Entity;

namespace CDKX.Services.Implement.ModelConfigurations.Identity
{
    public class UserClaimConfiguration : EntityConfigurationBase<SysUserClaim, int>
    {
        public UserClaimConfiguration()
        {
            HasRequired(m => m.User).WithMany();
        }
    }
}