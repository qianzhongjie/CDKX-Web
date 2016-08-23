using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDKX.Common.Result
{
    public class ResultJsonp : JsonResult
    {
        public ResultJsonp()
        {

            JsonRequestBehavior = JsonRequestBehavior.AllowGet;

        }



        public string Callback { get; set; }


        /// <summary>
        /// 对操作结果进行处理
        /// </summary>
        /// <param name="context">客户端请求</param>
        public override void ExecuteResult(ControllerContext context)
        {

            var httpContext = context.HttpContext;

            var callBack = Callback;

            string ss = "(";
            string sss = ");";
            //如果callback为空 则不加（）
            if (string.IsNullOrWhiteSpace(callBack))
            {
                callBack = httpContext.Request["callback"]; //获得客户端提交的回调函数名称
                if (string.IsNullOrWhiteSpace(callBack))
                {
                    ss = "";
                    sss = "";
                }
            }



            // 返回客户端定义的回调函数

            httpContext.Response.Write(callBack + ss);

            httpContext.Response.Write(Data);          //Data 是服务器返回的数据        

            httpContext.Response.Write(sss);            //将函数输出给客户端，由客户端执行

        }


    }

}