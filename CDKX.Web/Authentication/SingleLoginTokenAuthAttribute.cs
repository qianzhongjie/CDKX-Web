using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

using OSharp.Utility.Extensions;
using OSharp.Web.Http.Internal;
using OSharp.Utility.Secutiry;
using OSharp.Core.Context;
using OSharp.Core.Data;
using OSharp.Web.Http.Messages;
using OSharp.Utility.Data;
using CDKX.Services.Core.Models.User;

namespace CDKX.Web.Authentication
{
    /// <summary>
    /// 单点登录实现
    /// </summary>
    public class SingleLoginTokenAuthAttribute: AuthorizeAttribute
    {
        public bool AllowAnonymous = false;

        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            try
            {
                var scope = httpContext.Request.GetDependencyScope();
                var userService = scope.GetService(typeof(IRepository<UserInfo, int>)) as IRepository<UserInfo, int>;
                string token = httpContext.Request.Headers.GetValues(HttpHeaderNames.OSharpAuthenticationToken).FirstOrDefault();

                var strAuth = DesHelper.Decrypt(token, OSharp.Core.Constants.BodeAuthDesKey);
                Operator user = strAuth.FromJsonString<Operator>() ?? new Operator();

                int userId = int.Parse(user.UserId);
                var onlineToken = userService.GetByKey(userId).Token;
                var onlineStrAuth = DesHelper.Decrypt(onlineToken, OSharp.Core.Constants.BodeAuthDesKey);

                if (onlineToken != token)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return AllowAnonymous;
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            actionContext.Response = request.CreateResponse(HttpStatusCode.Forbidden);

            ApiResult result = new ApiResult(OperationResultType.ValidError, "帐号在其它设备登录.");
            actionContext.Response.Content = new StringContent(result.ToJsonString(), Encoding.UTF8, "application/json");
        }
    }
}