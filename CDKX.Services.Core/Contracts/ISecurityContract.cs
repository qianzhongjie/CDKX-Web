﻿// -----------------------------------------------------------------------
//  <copyright file="ISecurityContract.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2015 OSharp. All rights reserved.
//  </copyright>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2015-07-14 23:06</last-date>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CDKX.Services.Core.Dtos.Security;
using CDKX.Services.Core.Models.Security;
using OSharp.Core.Dependency;
using OSharp.Core.Security;
using OSharp.Utility.Data;

namespace CDKX.Services.Core.Contracts
{
    /// <summary>
    /// 业务契约——功能模块
    /// </summary>
    public interface ISecurityContract : IScopeDependency
    {
        /// <summary>
        /// 获取 角色功能映射信息查询数据集
        /// </summary>
        IQueryable<FunctionRoleMap> FunctionRoleMaps { get; }

        #region 功能信息业务

        /// <summary>
        /// 获取 功能信息查询数据集
        /// </summary>
        IQueryable<Function> Functions { get; }

        /// <summary>
        /// 检查功能信息信息是否存在
        /// </summary>
        /// <param name="predicate">检查谓语表达式</param>
        /// <param name="id">更新的功能信息编号</param>
        /// <returns>功能信息是否存在</returns>
        bool CheckFunctionExists(Expression<Func<Function, bool>> predicate, Guid id = default(Guid));

        /// <summary>
        /// 添加功能信息信息
        /// </summary>
        /// <param name="dtos">要添加的功能信息DTO信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult AddFunctions(params FunctionDto[] dtos);

        /// <summary>
        /// 更新功能信息信息
        /// </summary>
        /// <param name="dtos">包含更新信息的功能信息DTO信息</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> EditFunctions(params FunctionDto[] dtos);

        /// <summary>
        /// 删除功能信息信息
        /// </summary>
        /// <param name="ids">要删除的功能信息编号</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> DeleteFunctions(params Guid[] ids);

        /// <summary>
        /// 设置角色功能
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="functionIds">功能Id集合</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> SetRoleFunctions(int roleId, Guid[] functionIds);

        #endregion

        #region 实体数据信息业务

        /// <summary>
        /// 获取 实体数据信息查询数据集
        /// </summary>
        IQueryable<EntityInfo> EntityInfos { get; }

        /// <summary>
        /// 更新实体数据信息信息
        /// </summary>
        /// <param name="dtos">包含更新信息的实体数据信息DTO信息</param>
        /// <returns>业务操作结果</returns>
        OperationResult EditEntityInfos(params EntityInfoDto[] dtos);

        #endregion

    }
}