using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CDKX.Services.Core.Models.Identity;
using CDKX.Services.Core.Models.User;
using Microsoft.AspNet.Identity;
using OSharp.Core.Data;
using OSharp.Utility.Extensions;
using OSharp.Utility.Logging;

namespace CDKX.Web.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        // GET: Admin/Base
        public IRepository<UserInfo, int> UserInfoRepo { protected get; set; }
        public int Uid { get; set; }
        public string LoginName { get; set; }

        public string HeadPic { get; set; }
        public UserType UserTypes { get; set; }
        private ILogger _logger;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            SysUser user = null;
            if (Request.IsAuthenticated)
            {
                try
                {
                    user = User.Identity.GetUserName().FromJsonString<SysUser>();
                    var singleOrDefault = UserInfoRepo.GetByPredicate(x => x.SysUser.Id == user.Id).SingleOrDefault();
                    if (singleOrDefault != null)
                    {
                        Uid = singleOrDefault.Id;
                        HeadPic = singleOrDefault.HeadPic;
                        LoginName = singleOrDefault.SysUser.NickName;
                        UserTypes = singleOrDefault.SysUser.UserType;
                        ViewBag.UserId = Uid;
                        ViewBag.NickName = LoginName;
                    }
                }
                catch
                {
                }
            }
            ViewBag.HeadPic = HeadPic;
            ViewBag.Uid = Uid;
        }
    }
}