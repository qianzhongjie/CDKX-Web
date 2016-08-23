using System.Threading.Tasks;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Core.Models.User;
using OSharp.Core.Context;
using OSharp.Utility.Data;

namespace CDKX.Services.Core.Contracts
{
    public partial interface IUserContract
    {
        #region 账户相关

        /// <summary>
        /// 三方登录
        /// </summary>
        /// <param name="thirdKey">三方key</param>
        /// <param name="thirdProvider">微博?QQ?微信?</param>
        /// <param name="loginDevice">登录设备 IOS?Android?</param>
        /// <param name="clientVersion">客户端版本</param>
        /// <param name="registKey">极光推送Key</param>
        /// <returns></returns>
        Task<OperationResult> LoginByThird(string thirdKey, ThirdProvider thirdProvider, LoginDevice loginDevice, string clientVersion, string registKey = "");

        /// <summary>
        /// 获取用户注册验证码
        /// </summary>
        /// <param name="phoneNo">手机号</param>
        /// <param name="codeType">验证码类型</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> GetSmsValidateCode(string phoneNo, CodeType codeType);

        /// <summary>
        /// 获取邮箱验证码
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="codeType">验证码类型</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> GetEmailValidateCode(string email, CodeType codeType);

        /// <summary>
        /// 单独验证手机验证码
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        OperationResult ValidatePhone(string phoneNo, string validateCode, CodeType type);

        /// <summary>
        /// 验证用户注册
        /// </summary>
        /// <param name="dto">注册信息</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> ValidateRegister(UserInfoRegistDto dto, string validateCode);

        /// <summary>
        /// 验证注册码
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        Task<OperationResult> ValidateCode(string phoneNo, CodeType codeType, string validateCode);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="registKey">极光注册Key</param>
        /// <param name="loginDevice">登录设备</param>
        /// <param name="clientVersion">客户端当前版本</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> Login(string userName, string password, string registKey, LoginDevice loginDevice, string clientVersion, int type);


        /// <summary>
        /// 重置用户Token有效期
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="loginDevice">登录设备</param>
        /// <param name="clientVersion">客户端版本</param>
        /// <returns></returns>
        Task<OperationResult> ResetToken(UserInfo user, LoginDevice loginDevice, string clientVersion);

        /// <summary>
        /// 更改用户名
        /// </summary>
        /// <param name="userName">原用户名</param>
        /// <param name="newUserName">新用户名</param>
        /// <param name="password">登录密码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> ChangeUserName(string userName, string newUserName, string password, string validateCode);


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPsw">原密码</param>
        /// <param name="newPsw">新密码</param>
        /// <returns></returns>
        Task<OperationResult> ChangePassword(string userName, string oldPsw, string newPsw);

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userName">电话号码</param>
        /// <param name="newPsw">新密码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns></returns>
        Task<OperationResult> ResetPassword(string userName, string newPsw, string validateCode);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<SysUser> GetUserInfo(string userName);

        /// <summary>
        /// 获取临时密码
        /// </summary>
        /// <param name="phoneNo">手机号</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> GetSmsPassword(string phoneNo);
        #endregion

        /// <summary>
        /// 编辑UserInfo信息
        /// </summary>
        /// <param name="dtos">要更新的UserInfoEditDto信息</param>
        /// <returns>业务操作结果</returns>
        Task<OperationResult> EditUserInfos(params UserInfoEditDto[] dtos);

        /// <summary>
        /// 邮箱绑定
        /// </summary>
        /// <param name="validateCode"></param>
        /// <param name="nweEmail"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        OperationResult ChangeEmail(string validateCode, string nweEmail, string userName);

        /// <summary>
        /// 用model更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int UpdateUserInfo(SysUser model);


        /// <summary>
        /// 修改用户昵称和头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="nickName"></param>
        /// <param name="headPic"></param>
        /// <returns></returns>
        Task<OperationResult> EditUserInfo(int userId, string nickName, string headPic);
    }
}
