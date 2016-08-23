using System;
using System.ComponentModel;
using CDKX.Services.Core.Models.Identity;
using OSharp.Core.Security;
using OSharp.Core.Security.Models;

namespace CDKX.Services.Core.Models.Security
{
    /// <summary>
    /// 实体类——数据用户映射
    /// </summary>
    [Description("权限-数据用户映射")]
    public class EntityUserMap : EntityUserMapBase<int, EntityInfo, Guid, SysUser, int>
    { }
}