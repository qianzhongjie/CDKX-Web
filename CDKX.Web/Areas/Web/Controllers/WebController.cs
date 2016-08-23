using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Models.Identity;
using Microsoft.AspNet.Identity;
using OSharp.Core.Caching;
using OSharp.Utility.Data;
using OSharp.Utility.Extensions;
using CDKX.Web.Areas.Api.Models;
using CDKX.Web.Areas.Web.Models;

namespace CDKX.Web.Areas.Web.Controllers
{
    [Description("Web用户信息获取")]
    public class WebController : Controller
    {
        #region 公共参数
        // GET: Web/Web
        public IUserContract UserContract { get; set; }
        public int OperatorId { get; private set; }
        public string NickName { get; private set; }
        public UserType UserTypes { get; set; }
        public string OptimizationCache = "optimization";
        #endregion

        /// <summary>
        /// 处理用户登录状态
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
         
            SysUser user = null;
            if (Request.IsAuthenticated)
            {
                try
                {
                    user = User.Identity.GetUserName().FromJsonString<SysUser>();
                    var singleOrDefault = UserContract.UserInfos.SingleOrDefault(x => x.SysUser.Id == user.Id);
                    if (singleOrDefault != null)
                    {
                        OperatorId = singleOrDefault.Id;
                        UserTypes = singleOrDefault.SysUser.UserType;
                        ViewBag.UserId = OperatorId;
                        ViewBag.UserType = UserTypes;
                        NickName = singleOrDefault.SysUser.NickName ?? singleOrDefault.SysUser.UserName;
                        ViewBag.NickName = NickName;
                    }
                }
                catch
                {
                }
            }
        }
        //处理异常 跳转404页面
        //protected override void OnException(ExceptionContext filterContext)
        //{

        //    //if (!filterContext.ExceptionHandled && filterContext.Exception is ArgumentOutOfRangeException)
        //    if (!filterContext.ExceptionHandled)
        //    {
        //        filterContext.Result = new RedirectResult("~/Content/404/404.htm");
        //        filterContext.ExceptionHandled = true;
        //    }

        //    base.OnException(filterContext);
        //}



        #region 公共方法
 
        #endregion

    }
}