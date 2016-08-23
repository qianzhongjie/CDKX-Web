using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OSharp.Utility;
using OSharp.Utility.Data;
using OSharp.Web.Mvc;
using OSharp.Web.Mvc.Security;
using OSharp.Web.Mvc.UI;
using OSharp.Utility.Extensions;
using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Core.Models.User;

namespace CDKX.Web.Areas.Admin.Controllers
{
    [Authorize]
    [Description("会员管理")]
    public class UserController : AdminBaseController
    {
        public IUserContract UserContract { get; set; }
        public IIdentityContract IdentityContract { get; set; }

        #region Ajax功能

        [AjaxOnly]
        [Description("获取用户数据")]
        public ActionResult GetUserInfoData()
        {
            int total;
            GridRequest request = new GridRequest(Request);

            var datas =
                GetQueryData<UserInfo, int>(UserContract.UserInfos.Where(p => p.SysUser.UserType == UserType.App用户)
                .Include(p => p.SysUser), out total, request)
                    .Select(m => new
                    {
                        m.Id,
                        m.SysUser.IsLocked,
                        m.SysUser.UserName,
                        m.SysUser.NickName,
                        m.CreatedTime,
                        m.Sex
                    });
            return Json(new GridData<object>(datas, total), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Description("冻结/解冻用户")]
        public async Task<ActionResult> LockUserOrNot(int userId)
        {
            UserInfo userInfo = await UserContract.UserInfos.SingleOrDefaultAsync(p => p.Id == userId);

            var result = await IdentityContract.LockUserOrNot(userInfo.SysUser);
            return Json(result.ToAjaxResult());
        }

        [AjaxOnly]
        [Description("获取用户反馈")]
        public ActionResult GetFeedBackData()
        {
            int total;
            GridRequest request = new GridRequest(Request);

            var datas =
                GetQueryData<FeedBack, int>(UserContract.FeedBacks.Include(p => p.UserInfo.SysUser), out total, request).Select(m => new
                {
                    m.Id,
                    m.UserInfo.SysUser.UserName,
                    m.UserInfo.SysUser.NickName,
                    m.Content,
                    m.CreatedTime
                });
            return Json(new GridData<object>(datas, total), JsonRequestBehavior.AllowGet);
        }


        [AjaxOnly]
        [HttpPost]
        [Description("删除用户反馈")]
        public async Task<ActionResult> DeleteFeedBacks(int[] ids)
        {
            ids.CheckNotNull("ids");
            OperationResult result = await UserContract.DeleteFeedBacks(ids);
            return Json(result.ToAjaxResult());
        }


        #endregion

        #region 视图功能

        [Description("会员列表")]
        public ActionResult UserInfoList()
        {
            ViewBag.Sexs = typeof(Sex).ToDictionary().Select(p => new
            {
                val = p.Key,
                text = p.Value
            }).ToList();
            return View();
        }

        [Description("会员反馈")]
        public ActionResult FeedBackList()
        {
            return View();
        }

        #endregion
    }
}