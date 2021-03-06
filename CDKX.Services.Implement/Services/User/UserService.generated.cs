﻿// <autogenerated>
//   This file was generated by T4 code generator ServicesCodeScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using OSharp.Core;
using OSharp.Core.Data;
using OSharp.Utility;
using OSharp.Utility.Data;
using OSharp.Utility.Extensions;
using System.Linq.Expressions;
using System.Threading.Tasks;

using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.User;

namespace CDKX.Services.Implement.Services
{
	public partial class UserService : IUserContract
	{
		                #region FeedBack信息业务

                public IRepository<FeedBack, int> FeedBackRepo { protected get; set; }

                public IQueryable<FeedBack> FeedBacks
                {
                    get { return FeedBackRepo.Entities.Where(p => !p.IsDeleted); }
                }

                /// <summary>
                /// 保存FeedBack信息(新增/更新)
                /// </summary>
                /// <param name="updateForeignKey">更新时是否更新外键信息</param>
                /// <param name="dtos">要保存的FeedBackDto信息</param>
                /// <returns>业务操作集合</returns>
                public async Task<OperationResult> SaveFeedBacks(bool updateForeignKey=false,params FeedBackDto[] dtos)
                {
                    try
                    {
                        dtos.CheckNotNull("dtos");
                        var addDtos = dtos.Where(p => p.Id == 0).ToArray();
                        var updateDtos = dtos.Where(p => p.Id != 0).ToArray();

                        FeedBackRepo.UnitOfWork.TransactionEnabled = true;

                        Action<FeedBackDto> checkAction=null;
                        Func<FeedBackDto, FeedBack, FeedBack> updateFunc=(dto, entity) => 
                        {
                            if(dto.Id==0||updateForeignKey)
                            {
                                                                        entity.UserInfo = UserInfoRepo.GetByKey(dto.UserInfoId);
                                                                    }
                            return entity; 
                        };
                        if (addDtos.Length > 0)
                        {
                            FeedBackRepo.Insert(addDtos,checkAction,updateFunc);
                        }
                        if (updateDtos.Length > 0)
                        {
                            FeedBackRepo.Update(updateDtos,checkAction,updateFunc);
                        }
                        await FeedBackRepo.UnitOfWork.SaveChangesAsync();
                        return new OperationResult(OperationResultType.Success, "保存成功");
                    }
                    catch(Exception e)
                    {
                        return new OperationResult(OperationResultType.Error, e.Message);
                    }
                }

                /// <summary>
                /// 删除FeedBack信息
                /// </summary>
                /// <param name="ids">要删除的Id编号</param>
                /// <returns>业务操作结果</returns>
                public async Task<OperationResult> DeleteFeedBacks(params int[] ids)
                {
                    ids.CheckNotNull("ids");
                    await FeedBackRepo.RecycleAsync(p=>ids.Contains(p.Id));
                    return new OperationResult(OperationResultType.Success, "删除成功");
                }

                #endregion

                                #region UserInfo信息业务

                public IRepository<UserInfo, int> UserInfoRepo { protected get; set; }

                public IQueryable<UserInfo> UserInfos
                {
                    get { return UserInfoRepo.Entities.Where(p => !p.IsDeleted); }
                }

                /// <summary>
                /// 保存UserInfo信息(新增/更新)
                /// </summary>
                /// <param name="updateForeignKey">更新时是否更新外键信息</param>
                /// <param name="dtos">要保存的UserInfoDto信息</param>
                /// <returns>业务操作集合</returns>
                public async Task<OperationResult> SaveUserInfos(bool updateForeignKey=false,params UserInfoDto[] dtos)
                {
                    try
                    {
                        dtos.CheckNotNull("dtos");
                        var addDtos = dtos.Where(p => p.Id == 0).ToArray();
                        var updateDtos = dtos.Where(p => p.Id != 0).ToArray();

                        UserInfoRepo.UnitOfWork.TransactionEnabled = true;

                        Action<UserInfoDto> checkAction=null;
                        Func<UserInfoDto, UserInfo, UserInfo> updateFunc=(dto, entity) => 
                        {
                            if(dto.Id==0||updateForeignKey)
                            {
                                                                        entity.SysUser = SysUserRepo.GetByKey(dto.SysUserId);
                                                                    }
                            return entity; 
                        };
                        if (addDtos.Length > 0)
                        {
                            UserInfoRepo.Insert(addDtos,checkAction,updateFunc);
                        }
                        if (updateDtos.Length > 0)
                        {
                            UserInfoRepo.Update(updateDtos,checkAction,updateFunc);
                        }
                        await UserInfoRepo.UnitOfWork.SaveChangesAsync();
                        return new OperationResult(OperationResultType.Success, "保存成功");
                    }
                    catch(Exception e)
                    {
                        return new OperationResult(OperationResultType.Error, e.Message);
                    }
                }

                /// <summary>
                /// 删除UserInfo信息
                /// </summary>
                /// <param name="ids">要删除的Id编号</param>
                /// <returns>业务操作结果</returns>
                public async Task<OperationResult> DeleteUserInfos(params int[] ids)
                {
                    ids.CheckNotNull("ids");
                    await UserInfoRepo.RecycleAsync(p=>ids.Contains(p.Id));
                    return new OperationResult(OperationResultType.Success, "删除成功");
                }

                #endregion

                                #region ValidateCode信息业务

                public IRepository<ValidateCode, int> ValidateCodeRepo { protected get; set; }

                public IQueryable<ValidateCode> ValidateCodes
                {
                    get { return ValidateCodeRepo.Entities.Where(p => !p.IsDeleted); }
                }

                /// <summary>
                /// 保存ValidateCode信息(新增/更新)
                /// </summary>
                /// <param name="updateForeignKey">更新时是否更新外键信息</param>
                /// <param name="dtos">要保存的ValidateCodeDto信息</param>
                /// <returns>业务操作集合</returns>
                public async Task<OperationResult> SaveValidateCodes(bool updateForeignKey=false,params ValidateCodeDto[] dtos)
                {
                    try
                    {
                        dtos.CheckNotNull("dtos");
                        var addDtos = dtos.Where(p => p.Id == 0).ToArray();
                        var updateDtos = dtos.Where(p => p.Id != 0).ToArray();

                        ValidateCodeRepo.UnitOfWork.TransactionEnabled = true;

                        Action<ValidateCodeDto> checkAction=null;
                        Func<ValidateCodeDto, ValidateCode, ValidateCode> updateFunc=null;
                        if (addDtos.Length > 0)
                        {
                            ValidateCodeRepo.Insert(addDtos,checkAction,updateFunc);
                        }
                        if (updateDtos.Length > 0)
                        {
                            ValidateCodeRepo.Update(updateDtos,checkAction,updateFunc);
                        }
                        await ValidateCodeRepo.UnitOfWork.SaveChangesAsync();
                        return new OperationResult(OperationResultType.Success, "保存成功");
                    }
                    catch(Exception e)
                    {
                        return new OperationResult(OperationResultType.Error, e.Message);
                    }
                }

                /// <summary>
                /// 删除ValidateCode信息
                /// </summary>
                /// <param name="ids">要删除的Id编号</param>
                /// <returns>业务操作结果</returns>
                public async Task<OperationResult> DeleteValidateCodes(params int[] ids)
                {
                    ids.CheckNotNull("ids");
                    await ValidateCodeRepo.RecycleAsync(p=>ids.Contains(p.Id));
                    return new OperationResult(OperationResultType.Success, "删除成功");
                }

                #endregion

                	}
}
