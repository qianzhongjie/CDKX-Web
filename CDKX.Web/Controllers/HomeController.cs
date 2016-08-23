using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using OSharp.Core.Data;
using OSharp.Utility.Develop.T4;
using OSharp.Utility.Extensions;
using OSharp.Utility.Logging;

namespace CDKX.Web.Controllers
{
    [Description("系统主页")]
    public class HomeController : Controller
    {
        // GET: Home

        [Description("主页")]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}