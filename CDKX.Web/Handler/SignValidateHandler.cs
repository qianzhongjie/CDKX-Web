using OSharp.Utility.Secutiry;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CDKX.Web.Handler
{
    /// <summary>
    /// 客户端消息内容签名验证Handler
    /// </summary>
    public class SignValidateHandler : DelegatingHandler
    {
        /// <summary>
        /// 请求处理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var form = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.Form;
            if (form.Keys.Count == 0)
            {
                return base.SendAsync(request, cancellationToken);
            }
            if (!request.Headers.Contains("BodeSign"))
            {
                return CreateForbiddenResponseMessage(request);
            }

            var sign= request.Headers.GetValues("BodeSign").FirstOrDefault();
            var md5 = HashHelper.GetMd5(form.ToString(), Encoding.UTF8);
            if (sign != md5)
            {
                return CreateForbiddenResponseMessage(request);
            }
            return base.SendAsync(request, cancellationToken);
        }

        private static Task<HttpResponseMessage> CreateForbiddenResponseMessage(HttpRequestMessage request)
        {
            return Task<HttpResponseMessage>.Factory.StartNew(() =>
            {
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.Forbidden, "sign validate error");
                return response;
            });
        }
    }
}