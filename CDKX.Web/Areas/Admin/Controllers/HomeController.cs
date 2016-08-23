using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using OSharp.Core.Security;
using OSharp.Utility.Data;
using OSharp.Web.Mvc.Security;
using OSharp.Web.Mvc.UI;
using OSharp.Utility.Extensions;
using CDKX.Services.Core.Contracts;
using CDKX.Services.Core.Models.Identity;
using CDKX.Web.Areas.Admin.ViewModels;
using CDKX.Web.Authentication;

namespace CDKX.Web.Areas.Admin.Controllers
{
    [Description("管理主页")]
    //[BodeMenuGroupKey("xxxx")]
    public class HomeController : Controller
    {
        public IUserContract UserContract { get; set; }
        public ISecurityContract SecurityContract { get; set; }
        public IIdentityContract IdentityContract { get; set; }

        [WebIsLoginTokenAuth]
        [Description("后台首页")]
        public ActionResult Index()
        {
            var user = IdentityContract.Users.SingleOrDefault(p => p.UserName == User.Identity.Name);
            if (user == null || user.UserType == UserType.App用户)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }
            ViewBag.User = user;
            return View();
        }

        [AjaxOnly]
        [HttpPost]
        [Description("获取操作目录")]
        [WebIsLoginTokenAuth]
        public ActionResult GetMenus()
        {
            var icons = new[]
            {
                "fa-th", "fa-desktop", "fa-table", "fa-bar-chart-o", "fa-pencil-square-o", "fa-picture-o", "fa-calendar",
                "fa-credit-card","fa-laptop", "fa-hdd-o", "fa-tasks"," fa-list-alt"
            };

            var userRoleIds = IdentityContract.UserRoleMaps
                .Where(p => p.User.UserName == User.Identity.Name).Select(p => p.Role.Id).Distinct().ToList();

            var functions = SecurityContract.FunctionRoleMaps
                .Where(p => userRoleIds.Contains(p.Role.Id))
                .Where(p => !p.Function.IsLocked && !p.Function.IsCustom && !p.Function.IsAjax && p.Function.PlatformToken == PlatformToken.Mvc && p.Function.Controller != "Home")
                .Select(p => p.Function).Distinct().OrderBy(p => p.OrderNo).ToList();

            int i = 0;
            var menus = functions.Where(p => p.IsController).Select(p =>
            {
                return new TreeNode()
                {
                    Id = p.Id,
                    Text = p.Name,
                    IconCls = icons[i++ % icons.Count()],
                    Url = Url.Action(p.Action, p.Controller, new { area = p.Area }),
                    Children =
                        functions.Where(
                            m => m.MenuGroupKey == p.MenuGroupKey && !m.IsController && m.IsMenu)
                            .Select(m =>
                            {
                                string url = Url.Action(m.Action, m.Controller, new { area = m.Area });
                                if (url == "/") url = "";
                                return new TreeNode()
                                {
                                    Id = m.Id,
                                    Text = m.Name,
                                    IconCls = "",
                                    Url = url,
                                };
                            }).ToList()
                };
            }).ToList();
            return Json(menus);
        }

        [WebIsLoginTokenAuth]
        [Description("后台欢迎页")]
        public ActionResult Welcome()
        {
            var user = IdentityContract.Users.SingleOrDefault(p => p.UserName == User.Identity.Name);
            if (user == null || user.UserType == UserType.App用户)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }
            ViewBag.NickName = user.NickName;
            return View();
        }

        [WebIsLoginTokenAuth]
        [Description("退出登录")]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Description("修改密码")]
        [WebIsLoginTokenAuth]
        public async Task<ActionResult> UpdatePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(User.Identity.Name))
            {
                return Json(new OperationResult(OperationResultType.ValidError, "身份信息过期，请重新登录").ToAjaxResult());
            }
            var result = await IdentityContract.ResetPassword(User.Identity.Name, oldPassword, newPassword);
            return Json(result.ToAjaxResult());
        }
        
        [Description("登录页")]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Description("登录提交")]
        public async Task<ActionResult> LoginPostDo(string userName, string password)
        {
            var result = await IdentityContract.Login(userName, password);
            if (result.ResultType == OperationResultType.Success)
            {
                FormsAuthentication.SetAuthCookie(userName, false);
            }
            return Json(result.ToAjaxResult());
        }
    }
}
