using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using OSharp.Core.Context;
using OSharp.Utility.Data;
using OSharp.Utility.Extensions;
using OSharp.Web.Http.Messages;
using OSharp.Web.Http;
using System.ComponentModel;
using OSharp.Web.Http.Authentication;
using System.Linq;
using OSharp.Core.Data.Extensions;
using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Core.Models.User;
using CDKX.Services.Implement.Helper;
using OSharp.Core.Data;

namespace CDKX.Web.Areas.Api.Controllers
{
    [Description("API账户接口")]
    public class AccountController : BaseApiController
    {
        public IUserContract UserContract { get; set; }
        public IIdentityContract IdentityContract { get; set; }
        public IRepository<UserInfo, int> UserInfoRepo { get; set; }
        public IRepository<SysUser, int> SysUserRepo { get; set; }
        public IRepository<SysUserLogin, int> SysUserLoginRepo { get; set; }
        public IRepository<SysUserClaim, int> SysUserClaimRepo { get; set; }
        public IRepository<SysUserRoleMap, int> SysUserRoleMapRepo { get; set; }
        #region 账户相关业务

        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="phoneNo">手机号</param>
        /// <param name="codeType">1:注册;2:修改密码;3:修改手机号</param>
        [HttpPost]
        [Description("获取短信验证码")]
        public async Task<IHttpActionResult> GetSmsCode(string phoneNo, CodeType codeType = CodeType.用户注册)
        {
            if (!phoneNo.IsMobileNumber(true)) return Json(new ApiResult(OperationResultType.ValidError, "请输入正确的手机号"));

            var result = await UserContract.GetSmsValidateCode(phoneNo, codeType);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("获取邮箱验证码")]
        public async Task<IHttpActionResult> GetEmailCode(string email, CodeType codeType = CodeType.用户注册)
        {
            if (!email.IsEmail()) return Json(new ApiResult(OperationResultType.ValidError, "请输入正确的邮箱地址"));

            var result = await UserContract.GetEmailValidateCode(email, codeType);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("验证注册码是否正确")]
        public async Task<IHttpActionResult> ValidateCode(string phoneNo, CodeType codeType, string validateCode)
        {
            return Json((await UserContract.ValidateCode(phoneNo, codeType, validateCode)).ToApiResult());
        }

        [HttpPost]
        [Description("验证并注册")]
        public async Task<IHttpActionResult> ValidateRegister(string phoneNo, string password, string validateCode, LoginDevice loginDevice, string clientVersion, string email, string registKey = "")
        {
            var dto = new UserInfoRegistDto()
            {
                UserName = phoneNo,
                Password = password,
                NickName = "xbxd" + phoneNo,
                Email = email,
            };
            var result = await UserContract.ValidateRegister(dto, validateCode);

            if (result.Successed)
            {
                return Json((await UserContract.Login(phoneNo, password, registKey, loginDevice, clientVersion, 1)).ToApiResult());
            }
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("用户登录")]
        public async Task<IHttpActionResult> Login(string userName, string password, LoginDevice loginDevice, string clientVersion, string registKey = "")
        {
            var result = await UserContract.Login(userName, password, registKey, loginDevice, clientVersion, 1);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("验证码登录")]
        public async Task<IHttpActionResult> LoginByCode(string phoneNo, string code, LoginDevice loginDevice, string clientVersion, string registKey = "")
        {
            var result = await UserContract.Login(phoneNo, code, registKey, loginDevice, clientVersion, 2);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("第三方登录")]
        public async Task<IHttpActionResult> LoginByThird(string thirdKey, ThirdProvider thirdProvider, LoginDevice loginDevice, string clientVersion, string registKey = "")
        {
            var result = await UserContract.LoginByThird(thirdKey, thirdProvider, loginDevice, clientVersion, registKey);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [TokenAuth]
        [Description("启动时重置Token过期时间")]
        public async Task<IHttpActionResult> ResetTokenValidityPeriod(LoginDevice loginDevice, string clientVersion)
        {
            var user = await UserContract.UserInfos.SingleOrDefaultAsync(p => p.Id == OperatorId);
            if (user == null) return Json(new ApiResult("用户不存在", OperationResultType.QueryNull));

            var result = await UserContract.ResetToken(user, loginDevice, clientVersion);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [Description("重置密码")]
        public async Task<IHttpActionResult> ResetPassword(string userName, string newPsw, string validateCode)
        {
            var result = await UserContract.ResetPassword(userName, newPsw, validateCode);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [TokenAuth]
        [Description("修改密码")]
        public async Task<IHttpActionResult> ChangePassword(string userName, string oldPsw, string newPsw)
        {
            var result = await UserContract.ChangePassword(userName, oldPsw, newPsw);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [TokenAuth]
        [Description("修改用户名")]
        public async Task<IHttpActionResult> ChangeUserName(string newUserName, string password, string validateCode, string userName = "")
        {
            userName = UserContract.UserInfos.Single(x => x.Id == OperatorId).SysUser.UserName;
            var result = await UserContract.ChangeUserName(userName, newUserName, password, validateCode);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [TokenAuth]
        [Description("编辑用户头像昵称")]
        public async Task<IHttpActionResult> EditUserInfo(string nickName, string headPic = "")
        {
            var result = await UserContract.EditUserInfo(OperatorId, nickName, headPic);
            return Json(result.ToApiResult());
        }

        [HttpPost]
        [TokenAuth]
        [Description("获取自己个人信息")]
        public async Task<IHttpActionResult> GetUserInfo()
        {
            var theUser = await UserContract.UserInfos.Unrecycled().Include(p => p.SysUser).Select(x => new
            {
                x.Id,
                x.Birthday,
                x.HeadPic,
                x.Qq,
                x.Sex,
                x.SysUser.Email,
                x.SysUser.NickName,
                x.SysUser.PhoneNumber,
                x.SysUser.UserName
            }).SingleOrDefaultAsync(p => p.Id == OperatorId);
            if (theUser == null) return Json(new ApiResult(OperationResultType.QueryNull, "用户不存在"));
            return Json(new ApiResult("获取成功", theUser));
        }

        [HttpPost]
        [TokenAuth]
        [Description("意见反馈")]
        public async Task<IHttpActionResult> AddFeedBack(string content)
        {
            var dto = new FeedBackDto { UserInfoId = OperatorId, Content = content };
            var result = await UserContract.SaveFeedBacks(dtos: dto);

            return Json(new ApiResult(result.ResultType, "反馈成功"));
        }

        [HttpPost]
        [Description("Delete a user all the Information ")]
        public async Task<IHttpActionResult> DelUserInfo(string phoneNo)
        {
            var sysUser = SysUserRepo.Entities.SingleOrDefault(m => m.UserName == phoneNo);

            var userInfo = UserInfoRepo.Entities.SingleOrDefault(m => m.SysUser.UserName == phoneNo);

            SysUserRepo.Delete(sysUser.Id);
            return Json(CdkxResult.Success());

            // UserContract.UserInfos.SingleOrDefault(m => m.SysUser.UserName == phoneNo);
            //var afterService = AfterServiceRepo.Entities.SingleOrDefault(m => m.User.UserName == phoneNo);
            //var sysUserLogin = SysUserLoginRepo.Entities.SingleOrDefault(m => m.User.UserName == phoneNo);
            //var sysUserRoleMap = SysUserRoleMapRepo.Entities.SingleOrDefault(m => m.User.UserName == phoneNo);
            //var sysUserClaim = SysUserClaimRepo.Entities.SingleOrDefault(m => m.User.UserName == phoneNo);
            //SysUserRepo.UnitOfWork.TransactionEnabled = true;


            //if (sysUserClaim != null)
            //{

            //}
            //if (sysUserClaim != null)
            //{
            //    SysUserClaimRepo.Delete(sysUserClaim.Id);
            //}
            //if (sysUserLogin != null)
            //{
            //    SysUserLoginRepo.Delete(sysUserLogin.Id);
            //}
            //if (afterService != null)
            //{
            //    AfterServiceRepo.Delete(afterService.Id);
            //}
            //if (userInfo != null)
            //{
            //    UserInfoRepo.Delete(userInfo.Id);
            //}
            //SysUserRepo.Delete(sysUser.Id);

            //return Json((SysUserRepo.UnitOfWork.SaveChanges() == 0 ? BodeResult.NoChanged(): BodeResult.Success()).ToApiResult());
        }

        #endregion

        #region 关联用户的业务



        #endregion

    }
}
