// -----------------------------------------------------------------------
//  <copyright file="PageHtml.cs">
//      Copyright (c) 2015
//  </copyright>
//  <last-editor>qianzhongjie</last-editor>
//  <last-date>2015-05-20 4:30</last-date>
// -----------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
namespace System.Web.Mvc
{
    /// <summary>
    /// 分页控件 返回dom对象
    /// </summary>
    public static partial class PageHtml
    {
        //public static string  
        public static HtmlString ShowPageNavigate(this HtmlHelper htmlHelper, int currentPage, int pageSize, int totalCount, string scriptfun)
        {
            var redirectTo = htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url.AbsolutePath;
            pageSize = pageSize == 0 ? 3 : pageSize;
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1);//总页数
            var output = new StringBuilder();
            output.Append("<nav class=\"page-center\" style=\"text-align:center;\"><ul class=\"pagination\">");
            if (totalPages > 1)
            {
                {//处理首页连接
                    output.AppendFormat("<li><a href=\"javascript:void(0);\" onclick='" + scriptfun + "(1);'>&laquo;</a></li>");
                }
                if (currentPage >= 2)
                {
                    //处理上一页的连接
                    output.AppendFormat("<li><a href=\"javascript:void(0);\" onclick='" + scriptfun + "({0});'>上一页</a></li>", currentPage - 1);
                }
                else
                {
                    output.Append("<li class='disabled'><a href=\"javascript:void(0);\">上一页</a></li>");
                }
                int currint = 5;
                for (int i = 0; i <= 10; i++)
                {
                    if ((currentPage + i - currint) >= 1 && (currentPage + i - currint) <= totalPages)
                    {
                        if (currint == i)
                        {
                            //当前页处理
                            output.AppendFormat("<li><a href=\"javascript:void(0);\"  class=\"Btnactive\" onclick='" + scriptfun + "({0});'>{1}</a></li>", currentPage, (currentPage + i - currint));
                        }
                        else
                        {
                            //一般页处理
                            output.AppendFormat("<li><a href=\"javascript:void(0);\" onclick='" + scriptfun + "({0});'>{1}</a></li>", currentPage + i - currint, currentPage + i - currint);
                        }
                    }
                }
                if (currentPage < totalPages)
                {
                    //处理下一页的连接
                    output.AppendFormat("<li><a href=\"javascript:void(0);\" onclick='" + scriptfun + "({0});'>下一页</a></li>", currentPage + 1);
                }
                else
                {
                    output.AppendFormat("<li class=\"disabled\"><a href=\"javascript:void(0);\">下一页</a></li>");
                }
                if (currentPage != totalPages)
                {
                    output.AppendFormat("<li><a href='javascript:void(0)' onclick='" + scriptfun + "({0});'>&raquo;</a></li>", totalPages);
                }
                else
                {
                    output.AppendFormat("<li class=\"disabled\"><a href='javascript:void(0)'>&raquo;</a></li>");
                }
                output.Append("</ul></nav>");
            }
            return new HtmlString(output.ToString());
            //return output.ToString(); 
        }
    }
}