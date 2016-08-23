using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OSharp.Utility.Extensions;
using OSharp.Web.Http.Messages;

namespace CDKX.Web.Authentication
{
    public class WebIsLoginTokenAuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Request.IsAuthenticated;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //var url = "/Admin/Home/Login?re=login&url=" + filterContext.HttpContext.Request.Url.ToString().Replace("&", "@");
            filterContext.HttpContext.Response.Redirect("/Admin/Home/Login");
            //if (filterContext.HttpContext.Request.RequestType == "POST")
            //{
            //    ApiResult result = new ApiResult(OSharp.Utility.Data.OperationResultType.NoSingIn, "请先登录", filterContext.HttpContext.Request.UrlReferrer.ToString().Replace("&", "@"));
            //    filterContext.Result = new ContentResult
            //    {
            //        Content = result.ToJsonString(),
            //        ContentEncoding = Encoding.UTF8,
            //        ContentType = "application/json",
            //    };
            //}
            //else
            //{
            //    filterContext.HttpContext.Response.Redirect(url);
            //}
        }
    }
}