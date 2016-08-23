using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.Identity;
using System.Linq;
using Bode.Plugin.Core.SMS;
using CDKX.Services.Core.Models.User;
using CDKX.Services.Implement.Permissions.Identity;
using OSharp.Core.Data;
using OSharp.Utility;
using OSharp.Utility.Data;

namespace CDKX.Services.Implement.Services
{
    public partial class UserService
    {
        public IRepository<SysUser, int> SysUserRepo { protected get; set; }

        public IIdentityContract IdentityContract { protected get; set; }

        public IRepository<SysUserLogin, int> SysUserLoginRepo { protected get; set; }


        /// <summary>
        /// 获取或设置 用户管理器
        /// </summary>
        public UserManager UserManager { get; set; }

        /// <summary>
        /// 获取或设置 用户存储器
        /// </summary>
        //public UserStore UserStore { get; set; }

        public ISms Sms { get; set; }


        /// <summary>
        /// 编辑UserInfo信息
        /// </summary>
        /// <param name="dtos">要更新的UserInfoEditDto信息</param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> EditUserInfos(params UserInfoEditDto[] dtos)
        {
            dtos.CheckNotNull("dtos");

            var result = UserInfoRepo.Update(dtos, updateFunc: (dto, userInfo) =>
            {
                var sysUser = userInfo.SysUser;
                sysUser.NickName = dto.NickName;
                sysUser.PhoneNumber = dto.PhoneNumber;
                sysUser.Email = dto.Email;
                return userInfo;
            });
            return await Task.FromResult(result);
        }
        /// <summary>
        /// 修改用户昵称和头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="headPic"></param>
        /// <returns></returns>
        public async Task<OperationResult> EditUserInfo(int userId, string nickName, string headPic)
        {
            UserInfoRepo.UnitOfWork.TransactionEnabled = true;
            var info = UserInfoRepo.GetByKey(userId);
            info.HeadPic = headPic;
            await UserInfoRepo.UpdateAsync(info);
            var sys = info.SysUser;
            sys.NickName = nickName;
            var result = await SysUserRepo.UpdateAsync(sys);
            await UserInfoRepo.UnitOfWork.SaveChangesAsync();
            return new OperationResult(OperationResultType.Success, "修改成功");
        }
    }
}
