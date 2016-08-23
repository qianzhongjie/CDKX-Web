using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CDKX.Services.Core.Dtos.User;
using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Core.Models.User;
using Microsoft.AspNet.Identity;
using OSharp.Core.Context;
using OSharp.Core.Identity;
using OSharp.Utility;
using OSharp.Utility.Data;
using OSharp.Utility.Drawing;
using OSharp.Utility.Extensions;
using OSharp.Utility.Secutiry;
using OSharp.Utility.Net.Mail;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Configuration;
using CDKX.Common.Data;
using CDKX.Common.Result;
using CDKX.Services.Core.Contracts;
using OSharp.Core.Data;
using CDKX.Services.Implement.Helper;

namespace CDKX.Services.Implement.Services
{
    public partial class UserService
    {
        // public IUserContract UserContract { get; set; }
        /// <summary>
        /// 获取手机验证码
        /// </summary>
        /// <param name="phoneNo">手机号</param>
        /// <param name="codeType"></param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> GetSmsValidateCode(string phoneNo, CodeType codeType)
        {
            var user = UserInfos.Count(x => x.SysUser.PhoneNumber == phoneNo);
            string validateCode = new ValidateCoder().GetCode(6, ValidateCodeType.Number);
            var codeEntity = new ValidateCode()
            {
                CodeKey = phoneNo,
                Code = validateCode,
                ValidateType = ValidateType.手机,
                CodeType = codeType
            };
            await ValidateCodeRepo.InsertAsync(codeEntity);

            if (codeType == CodeType.临时密码 || codeType == CodeType.找回密码)
            {
                if (user == 0)
                {
                    return new OperationResult(OperationResultType.NoChanged, "此帐号还未注册", 0);
                }
                var results = SendMsg53kf.HttpGet((int)codeType, validateCode, phoneNo);
                if (results.code == 0) return new OperationResult(OperationResultType.Success, "验证码发送成功，注意查看短信", 0);
                return new OperationResult(OperationResultType.Error, "短信系统繁忙，请稍后再试", 0);
            }
            else
            {
                if (user > 0)
                {
                    return new OperationResult(OperationResultType.NoChanged, "此帐号已注册", 0);
                }
                var result = SendMsg53kf.HttpGet((int)codeType, validateCode, phoneNo);
                if (result.code == 0) return new OperationResult(OperationResultType.Success, "验证码发送成功，注意查看短信", 0);
                return new OperationResult(OperationResultType.Error, "短信系统繁忙，请稍后再试", 0);
            }
        }

        /// <summary>
        /// 获取临时密码
        /// </summary>
        /// <param name="phoneNo">手机号</param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> GetSmsPassword(string phoneNo)
        {
            return await SendValidateCode(phoneNo, ValidateType.手机, CodeType.临时密码, code =>
            {
                var smsContent = "您本次临时密码为" + code + "，工作人员不会向您索要此密码，请勿向任何人泄露。[西部鞋都]";
                // Sms.Send(phoneNo, 1, smsContent);
            });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<SysUser> GetUserInfo(string userName)
        {
            SysUser user = await UserManager.FindByNameAsync(userName);
            return user;
        }

        /// <summary>
        /// 用model更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateUserInfo(SysUser model)
        {
            return SysUserRepo.Update(model);
        }

        /// <summary>
        /// 获取邮箱验证码
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <param name="codeType">验证码类型</param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> GetEmailValidateCode(string email, CodeType codeType)
        {
            return await SendValidateCode(email, ValidateType.邮箱, codeType, code =>
            {
                //smtp.163.com
                string senderServerIp = "123.125.50.133";
                //smtp.gmail.com
                //string senderServerIp = "74.125.127.109";
                //string senderServerIp = "smtp.qq.com";
                //string senderServerIp = "58.251.149.147";
                //string senderServerIp = "smtp.sina.com";
                string fromMailAddress = "maixiaohao001@163.com";
                string subjectInfo = "验证码";
                string bodyInfo = "您本次的验证码为" + code + "，工作人员不会向您索要此验证码，请勿向任何人泄露。[女鞋之都]";
                string mailUsername = "maixiaohao001";
                string mailPassword = "meiyoumima001"; //发送邮箱的密码
                string mailPort = "25";

                MailSender emailSender = new MailSender(senderServerIp, email, fromMailAddress, subjectInfo, bodyInfo, mailUsername, mailPassword, mailPort, false, false);
                emailSender.Send();
            });
        }

        /// <summary>
        /// 邮箱绑定
        /// </summary>
        /// <param name="validateCode"></param>
        /// <param name="nweEmail"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public OperationResult ChangeEmail(string validateCode, string nweEmail, string userName)
        {
            var severCode = GetValidateCode(userName, CodeType.找回密码);
            if (severCode == null || severCode.Code != validateCode)
            {
                return new OperationResult(OperationResultType.ValidError, "验证码错误", 0);
            }
            var userInfo = UserInfoRepo.GetByPredicate(x => x.SysUser.UserName == userName);
            var userInfos = userInfo as UserInfo[] ?? userInfo.ToArray();
            if (!userInfos.Any())
            {
                return new OperationResult(OperationResultType.ValidError, "身份信息错误", 0);
            }
            //smtp.163.com
            string senderServerIp = "123.125.50.133";
            //string senderServerIp = "smtp.163.com";
            //smtp.gmail.com
            //string senderServerIp = "74.125.127.109";
            //string senderServerIp = "smtp.qq.com";
            //string senderServerIp = "58.251.149.147";
            //string senderServerIp = "smtp.sina.com";
            string url = ConfigurationManager.AppSettings["ServerHost"] + "Web/Home/EmailBack?e=" + DataCode.Base64Encrypt(nweEmail, Encoding.UTF8) + "&u=" + userInfos.Single().Id + "&p=" + userInfos.Single().SysUser.PasswordHash + "";
            url = url.Replace("+", "%2B");
            string fromMailAddress = ConfigurationManager.AppSettings["Email"];//"maixiaohao001@163.com";
            string subjectInfo = "邮箱绑定【西部鞋都】";
            string bodyInfo = "你正在进行邮箱绑定操作，如果是你本人操作，请点击下面的链接完成邮箱绑定。【西部鞋都】<br/> <a href=\"" + url + "\">" + url + "</a>";
            string mailUsername = "heikexiaoqian";
            string mailPassword = ConfigurationManager.AppSettings["EmailPwd"]; //发送邮箱的密码
            string mailPort = "25";

            MailSender emailSender = new MailSender(senderServerIp, nweEmail, fromMailAddress, subjectInfo, bodyInfo, mailUsername, mailPassword, mailPort, false, false);
            emailSender.Send();
            return new OperationResult(OperationResultType.Success, "请到邮箱完成验证", 0);
        }

        /// <summary>
        /// 单独验证手机验证码
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public OperationResult ValidatePhone(string phoneNo, string validateCode, CodeType type)
        {
            phoneNo.CheckNotNull("phoneNo");
            validateCode.CheckNotNullOrEmpty("validateCode");
            //验证码
            var severCode = GetValidateCode(phoneNo, CodeType.用户注册);
            if (severCode == null || severCode.Code != validateCode)
            {
                return new OperationResult(OperationResultType.ValidError, "手机验证码有误误", 0);
            }
            return new OperationResult(OperationResultType.Success, "验证通过", 1);
        }

        /// <summary>
        /// 验证注册码是否正确
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <param name="validateCode"></param>
        /// <returns></returns>
        public async Task<OperationResult> ValidateCode(string phoneNo,CodeType codeType, string validateCode)
        {
            var severCode = GetValidateCode(phoneNo, codeType);
            if (severCode == null || severCode.Code != validateCode)
            {
                return new OperationResult(OperationResultType.ValidError, "验证码错误", 0);
            }
            if (SysUserRepo.CheckExists(p => p.UserName == phoneNo))
            {
                if (codeType == CodeType.用户注册)
                {
                    return new OperationResult(OperationResultType.ValidError, "账号已被使用", 0);
                }
            }
            else
            {
                if (codeType == CodeType.更换手机 || codeType == CodeType.找回密码 || codeType == CodeType.临时密码)
                {
                    return CdkxResult.ValidError("帐号不存在.");
                }
            }
            return CdkxResult.Success();
        }

        /// <summary>
        /// 验证用户注册
        /// </summary>
        /// <param name="dto">用户注册信息</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> ValidateRegister(UserInfoRegistDto dto, string validateCode)
        {
            dto.CheckNotNull("dto");
            validateCode.CheckNotNullOrEmpty("validateCode");
            //验证码
            var severCode = GetValidateCode(dto.UserName, CodeType.用户注册);
            if (severCode == null || severCode.Code != validateCode)
            {
                return new OperationResult(OperationResultType.ValidError, "验证码错误", 0);
            }
            if (SysUserRepo.CheckExists(p => p.UserName == dto.UserName))
            {
                return new OperationResult(OperationResultType.ValidError, "账号已被使用", 0);
            }
            try
            {
                UserInfoRepo.UnitOfWork.TransactionEnabled = true;
                //验证密码格式
                IdentityResult result = await UserManager.PasswordValidator.ValidateAsync(dto.Password);
                if (!result.Succeeded) return result.ToOperationResult();

                SysUser sUser = new SysUser()
                {
                    UserName = dto.UserName,
                    NickName = dto.NickName,
                    Email = dto.Email,
                    PasswordHash = UserManager.PasswordHasher.HashPassword(dto.Password),//密码加密
                    UserType = UserType.App用户
                };
                if (severCode.ValidateType == ValidateType.手机)
                {
                    sUser.PhoneNumber = dto.UserName;
                    sUser.PhoneNumberConfirmed = true;
                }
                else
                {
                    sUser.Email = dto.UserName;
                    sUser.EmailConfirmed = true;
                }
                await UserManager.CreateAsync(sUser);

                var userInfo = Mapper.Map<UserInfo>(dto);
                userInfo.SysUser = sUser;
                await UserInfoRepo.InsertAsync(userInfo);

                await UserInfoRepo.UnitOfWork.SaveChangesAsync();
                var sysUser = await UserManager.FindByNameAsync(sUser.UserName);
                return new OperationResult(OperationResultType.Success, "注册成功", sysUser);
            }
            catch
            {
                return new OperationResult(OperationResultType.NoChanged, "注册失败", 0);
            }
        }

        /// <summary>
        /// 三方登录
        /// </summary>
        /// <param name="thirdKey">三方key</param>
        /// <param name="thirdProvider">微博?QQ?微信?</param>
        /// <param name="loginDevice">登录设备 IOS?Android?</param>
        /// <param name="clientVersion">客户端版本</param>
        /// <param name="registKey">极光推送Key</param>
        /// <returns></returns>
        public async Task<OperationResult> LoginByThird(string thirdKey, ThirdProvider thirdProvider, LoginDevice loginDevice, string clientVersion, string registKey = "")
        {
            var thirdUser = SysUserLoginRepo.Entities.SingleOrDefault(m => m.ProviderKey == thirdKey && m.ThridProvider == thirdProvider);
            if (thirdUser == null)
            {
                var userName = "xbxd" + DateTime.Now.ToString("yyyyMMddhhffff");
                SysUser sUser = new SysUser()
                {
                    UserName = userName,
                    NickName = userName,
                    UserType = UserType.App用户,
                };
                UserInfoRepo.UnitOfWork.TransactionEnabled = true;
                await UserManager.CreateAsync(sUser);
                var userInfo = new UserInfo()
                {
                    SysUser = sUser,
                    IsDeleted = false,
                    Sex = Sex.不限,
                };
                await UserInfoRepo.InsertAsync(userInfo);
                thirdUser = new SysUserLogin()
                {
                    ProviderKey = thirdKey,
                    ThridProvider = thirdProvider,
                    User = sUser,
                };
                await SysUserLoginRepo.InsertAsync(thirdUser);
                await UserInfoRepo.UnitOfWork.SaveChangesAsync();
            }

            var theUser = UserInfoRepo.Entities.SingleOrDefault(m => m.SysUser.Id == thirdUser.User.Id);
            if (theUser.RegistKey != registKey)
            {
                theUser.RegistKey = registKey;
                await UserInfoRepo.UpdateAsync(theUser);
            }

            //变更登录信息
            await ResetToken(theUser, loginDevice, clientVersion);

            var loginInfo = new UserTokenDto()
            {
                Id = theUser.Id,
                NickName = theUser.SysUser.NickName,
                HeadPic = theUser.HeadPic,
                Sex = theUser.Sex,
                Token = theUser.Token
            };
            return CdkxResult.Success(loginInfo);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="registKey">极光registKey</param>
        /// <param name="loginDevice">登录设备</param>
        /// <param name="clientVersion">客户端版本</param>
        /// <param name="type">登录方式</param>
        /// <returns></returns>
        public async Task<OperationResult> Login(string userName, string password, string registKey, LoginDevice loginDevice, string clientVersion, int type)
        {
            userName.CheckNotNullOrEmpty("userName");
            password.CheckNotNullOrEmpty("password");

            SysUser sUser = await UserManager.FindByNameAsync(userName);
            if (sUser == null)
            {
                return new OperationResult(OperationResultType.ValidError, "用户不存在", 0);
            }
            if (sUser.IsLocked) return new OperationResult(OperationResultType.ValidError, "您的账号已被冻结,请联系客服", 0);
            if (type == 2)
            {
                //验证码
                var severCode = GetValidateCode(userName, CodeType.临时密码);
                if (severCode == null || severCode.Code != password)
                {
                    return new OperationResult(OperationResultType.ValidError, "临时密码错误", 0);
                }
            }
            else
            {
                if (!await UserManager.CheckPasswordAsync(sUser, password))
                {
                    return new OperationResult(OperationResultType.ValidError, "用户名或密码错误", null);
                }
            }
            if (sUser.UserType != UserType.App用户)
            {
                return new OperationResult(OperationResultType.QueryNull, "用户不存在", null);
            }
            //更新最后一次登录的RegistKey
            var theUser = await UserInfos.SingleOrDefaultAsync(p => p.SysUser.UserName == userName);
            if (theUser.RegistKey != registKey)
            {
                theUser.RegistKey = registKey;
                await UserInfoRepo.UpdateAsync(theUser);
            }

            //变更登录信息
            await ResetToken(theUser, loginDevice, clientVersion);

            var loginInfo = new UserTokenDto()
            {
                Id = theUser.Id,
                NickName = theUser.SysUser.NickName,
                HeadPic = theUser.HeadPic,
                Sex = theUser.Sex,
                PhoneNo = theUser.SysUser.PhoneNumber,
                Token = theUser.Token
            };
            //return new OperationResult(OperationResultType.Success, "登录成功", sUser);
            return new OperationResult(OperationResultType.Success, "登录成功", loginInfo);
        }

        /// <summary>
        /// 更改用户名
        /// </summary>
        /// <param name="userName">原用户名</param>
        /// <param name="newUserName">新用户名</param>
        /// <param name="password">登录密码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>业务操作结果</returns>
        public async Task<OperationResult> ChangeUserName(string userName, string newUserName, string password, string validateCode)
        {
            userName.CheckNotNullOrEmpty("userName");
            newUserName.CheckNotNullOrEmpty("newUserName");
            validateCode.CheckNotNullOrEmpty("validateCode");
            //验证码
            var severCode = GetValidateCode(newUserName, CodeType.更换手机);
            if (severCode == null || severCode.Code != validateCode)
            {
                return await Task.FromResult(new OperationResult(OperationResultType.ValidError, "验证码错误"));
            }

            var sUser = await UserManager.FindByNameAsync(userName);

            if (sUser == null)
            {
                return new OperationResult(OperationResultType.NoChanged, "用户不存在");
            }

            if (!await UserManager.CheckPasswordAsync(sUser, password))
            {
                return new OperationResult(OperationResultType.ValidError, "登录密码错误", null);
            }

            if (SysUserRepo.CheckExists(p => p.UserName == newUserName, sUser.Id))
            {
                return new OperationResult(OperationResultType.NoChanged, "该用户名已存在");
            }

            sUser.UserName = userName;
            if (severCode.ValidateType == ValidateType.手机)
            {
                sUser.PhoneNumber = newUserName;
                sUser.UserName = newUserName;
                sUser.PhoneNumberConfirmed = true;
            }
            else
            {
                sUser.Email = newUserName;
                sUser.EmailConfirmed = true;
            }
            await SysUserRepo.UpdateAsync(sUser);
            return new OperationResult(OperationResultType.Success, "更改成功");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPsw">原密码</param>
        /// <param name="newPsw">新密码</param>
        /// <returns></returns>
        public async Task<OperationResult> ChangePassword(string userName, string oldPsw, string newPsw)
        {
            userName.CheckNotNullOrEmpty("userName");
            oldPsw.CheckNotNullOrEmpty("oldPsw");
            newPsw.CheckNotNullOrEmpty("newPsw");

            var sUser = await UserManager.FindByNameAsync(userName);

            if (sUser == null)
            {
                return new OperationResult(OperationResultType.NoChanged, "用户不存在");
            }

            if (!await UserManager.CheckPasswordAsync(sUser, oldPsw))
            {
                return new OperationResult(OperationResultType.ValidError, "原密码错误", null);
            }
            UserManager.RemovePassword(sUser.Id);
            UserManager.AddPassword(sUser.Id, newPsw);
            return new OperationResult(OperationResultType.Success, "密码修改成功");
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="newPsw">新密码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns></returns>
        public async Task<OperationResult> ResetPassword(string userName, string newPsw, string validateCode)
        {
            userName.CheckNotNullOrEmpty("userName");
            newPsw.CheckNotNullOrEmpty("newPsw");
            validateCode.CheckNotNullOrEmpty("validateCode");

            //验证码
            var severCode = GetValidateCode(userName, CodeType.找回密码);
            if (severCode == null || severCode.Code != validateCode)
            {
                return await Task.FromResult(new OperationResult(OperationResultType.ValidError, "验证码错误"));
            }

            var sUser = await UserManager.FindByNameAsync(userName);
            if (sUser == null)
            {
                return new OperationResult(OperationResultType.NoChanged, "用户不存在");
            }

            if (sUser.UserType != UserType.App用户)
            {
                return new OperationResult(OperationResultType.NoChanged, "用户不存在");
            }

            sUser.PasswordHash = UserManager.PasswordHasher.HashPassword(newPsw);

            var result = SysUserRepo.Update(sUser);
            return result == 0 ? new OperationResult(OperationResultType.ValidError, "服务器繁忙") : new OperationResult(OperationResultType.Success, "密码重置成功");
            //UserInfoRepo.Update(sUser);
            //UserManager.RemovePassword(sUser.Id);
            //UserManager.AddPassword(sUser.Id, newPsw);
        }

        /// <summary>
        /// 重置用户Token有效期
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="loginDevice">登录设备</param>
        /// <param name="clientVersion">客户端版本</param>
        /// <returns></returns>
        public async Task<OperationResult> ResetToken(UserInfo user, LoginDevice loginDevice, string clientVersion)
        {
            Operator oper = new Operator()
            {
                UserId = user.Id.ToString(),
                UserName = user.SysUser.UserName,
                LoginDevice = loginDevice,
                PhoneNo = user.SysUser.PhoneNumber,
                ClientVersion = clientVersion,
                ValidatePeriod = DateTime.Now.AddDays(30),//默认30天有效期
                UserDatas = new Dictionary<string, object>()
            };
            string strAuth = oper.ToJsonString();
            user.Token = DesHelper.Encrypt(strAuth, OSharp.Core.Constants.BodeAuthDesKey);

            await UserInfoRepo.UpdateAsync(user);
            return new OperationResult(OperationResultType.Success, "重置成功", user.Token);
        }

        #region 私有方法

        /// <summary>
        /// 从数据库读取验证码
        /// </summary>
        /// <param name="codeKey">验证码Key</param>
        /// <param name="codeType">验证码类型</param>
        /// <returns></returns>
        private ValidateCode GetValidateCode(string codeKey, CodeType codeType)
        {
            var dt = DateTime.Now.AddHours(-1);
            return ValidateCodes.Where(p => p.CodeKey == codeKey && p.CodeType == codeType && p.CreatedTime >= dt)
                    .OrderByDescending(p => p.CreatedTime).FirstOrDefault();
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="codeKey">验证码Key</param>
        /// <param name="validateType">验证方式</param>
        /// <param name="codeType">验证码类型</param>
        /// <param name="sendAction">发送委托</param>
        /// <returns>业务操作结果</returns>
        private async Task<OperationResult> SendValidateCode(string codeKey, ValidateType validateType, CodeType codeType, Action<string> sendAction)
        {
            codeKey.CheckNotNull("codeKey");
            sendAction.CheckNotNull("sendAction");

            string validateCode = new ValidateCoder().GetCode(6, ValidateCodeType.Number);
            var codeEntity = new ValidateCode()
            {
                CodeKey = codeKey,
                Code = validateCode,
                ValidateType = validateType,
                CodeType = codeType
            };
            var result = await ValidateCodeRepo.InsertAsync(codeEntity);

            sendAction(validateCode);
            return new OperationResult(OperationResultType.Success, "验证码发送成功", validateCode);
        }
        #endregion

    }
}
