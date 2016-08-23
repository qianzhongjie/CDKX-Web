using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OSharp.Utility.Data;
using OSharp.Web.Http.Messages;
using Com.Alipay;
using OSharp.Web.Mvc.Pay.WxPay;
using OSharp.Utility.Logging;

namespace CDKX.Web.Areas.Web.Controllers
{
    [Description("web支付")]
    public class PayController : WebController
    {
        private static readonly string ServerHost = ConfigurationManager.AppSettings["ServerHost"];

        public ActionResult PayOrder()
        {
            return View();
        }

        // GET: Web/Pay
        [Description("订单发起支付")]
        [HttpGet]
        public ActionResult Topay(string orderNo)
        {

            //var order = PresellOrderContract.PresellOrderInfos.SingleOrDefault(x => x.OrderNo == orderNo);
            //if (order == null) return Json(new ApiResult(OperationResultType.Error, "订单号有误！"), JsonRequestBehavior.AllowGet);
            ////////////////////////////////////////////请求参数////////////////////////////////////////////
            //支付类型
            string payment_type = "1";
            //必填，不能修改
            //服务器异步通知页面路径
            string notify_url = "http://xibuxiedu.dev.bodecn.com/Web/Pay/AlipayNotifyUrl";
            //需http://格式的完整路径，不能加?id=123这类自定义参数
            //页面跳转同步通知页面路径
            string return_url = "http://xibuxiedu.dev.bodecn.com/Web/Pay/AlipayReturnUrl";
            //需http://格式的完整路径，不能加?id=123这类自定义参数，不能写成http://localhost/
            //商户订单号
            string out_trade_no = orderNo;
            //商户网站订单系统中唯一订单号，必填
            //订单名称
            string subject = "";//order.OrderLines.First().PresellGoodsInfo.Name;
            //必填
            //付款金额
            string total_fee = "";// order.TotalPrice.ToString();
            //必填/
            //订单描述
            string body = "xx";//order.OrderLines.First().PresellGoodsInfo.Name + " " + order.OrderLines.Count() + "件商品"; ;
            //商品展示地址
            string show_url = ""; //ServerHost + "Web/PresellGoods/PresellGoodsInfo?goodsId=" + order.OrderLines.First().PresellGoodsInfo.Id;
            //需以http://开头的完整路径，例如：http://www.商户网址.com/myorder.html
            //防钓鱼时间戳
            string anti_phishing_key = "";
            //若要使用请调用类文件submit中的query_timestamp函数
            //客户端的IP地址
            string exter_invoke_ip = "";
            //非局域网的外网IP地址，如：221.0.0.1
            ////////////////////////////////////////////////////////////////////////////////////////////////
            //把请求参数打包成数组
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Com.Alipay.Config.Partner);
            sParaTemp.Add("seller_email", Com.Alipay.Config.Seller_email);
            sParaTemp.Add("_input_charset", "utf-8");
            sParaTemp.Add("service", "create_direct_pay_by_user");
            sParaTemp.Add("payment_type", payment_type);
            sParaTemp.Add("notify_url", notify_url);
            sParaTemp.Add("return_url", return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("body", body);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("anti_phishing_key", anti_phishing_key);
            sParaTemp.Add("exter_invoke_ip", exter_invoke_ip);

            //建立请求
            string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
            Response.Write(sHtmlText);
            return Content("");
        }


        [HttpPost]
        [Description("支付宝异步回调地址")]
        public async Task<ActionResult> AlipayNotifyUrl()
        {
            var loger = LogManager.GetLogger("Alipay");

            SortedDictionary<string, string> sPara = GetRequestPost();

            Com.Alipay.Notify aliNotify = new Com.Alipay.Notify();
            bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

            if (sPara.Count > 0 && verifyResult)
            {
                //商户订单号
                string outTradeNo = Request.Form["out_trade_no"];
                //支付宝交易号
                string tradeNo = Request.Form["trade_no"];
                //交易状态
                string tradeStatus = Request.Form["trade_status"];

                //打日志
                loger.Error("orderNo:{0};tradeStatus:{1};", outTradeNo, tradeStatus);
                if (tradeStatus == "TRADE_FINISHED" || tradeStatus == "TRADE_SUCCESS")
                {
                    //await OrderContract.PayOrder(outTradeNo, tradeNo, PayType.支付宝);
                    return Content("success");
                    //注意：
                    //该种交易状态只在两种情况下出现
                    //1、开通了普通即时到账，买家付款成功后。
                    //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                    //1、开通了高级即时到账，买家付款成功后。
                }
            }
            return Content("");
        }

        [Description("支付宝同步回调地址")]
        public ActionResult AlipayReturnUrl()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Com.Alipay.Notify aliNotify = new Com.Alipay.Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);
                if (verifyResult)//验证成功
                {
                    string out_trade_no = Request.QueryString["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];
                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];
                    if (trade_status == "TRADE_FINISHED" || trade_status == "TRADE_SUCCESS")
                    {
                        // PresellOrderContract.OrederPay(out_trade_no, PayTypes.支付宝);
                        return Content("success");
                        //注意：
                        //该种交易状态只在两种情况下出现
                        //1、开通了普通即时到账，买家付款成功后。
                        //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                        //1、开通了高级即时到账，买家付款成功后。
                    }

                    //打印页面
                    Response.Write("验证成功<br />");

                    //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
                else//验证失败
                {
                    Response.Write("验证失败");
                }
            }
            else
            {
                Response.Write("无返回参数");
            }
            return Content("");
        }

        [HttpPost]
        [Description("微信回调地址")]
        public async Task<ActionResult> WxpayNotifyUrl()
        {
            var loger = LogManager.GetLogger("Wxpay");
            HttpContextBase context = HttpContext;
            OSharp.Web.Mvc.Pay.WxPay.WxPayData notifyData = new OSharp.Web.Mvc.Pay.WxPay.Notify(context).GetNotifyData();
            loger.Error("notifyData:{0}", notifyData);
            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                return ReturnWxContent("FAIL", "支付结果中微信订单号不存在");
            }
            //查询订单，判断订单真实性
            string transactionId = notifyData.GetValue("transaction_id").ToString();
            loger.Error("transactionId:{0}", transactionId);
            if (!QueryWxOrder(transactionId))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                return ReturnWxContent("FAIL", "订单查询失败");
            }
            //查询订单成功
            else
            {
                string orderNo = notifyData.GetValue("out_trade_no").ToString();
                //OperationResult result = await OrderContract.PayOrder(orderNo, transactionId, PayType.微信);
                //return result.Successed ? ReturnWxContent("SUCCESS", "OK") : ReturnWxContent("FAIL", "平台业务处理失败");
                return ReturnWxContent("SUCCESS", "OK");

            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 查询微信订单
        /// </summary>
        /// <param name="transactionId">微信支付订单号</param>
        /// <returns>订单是否存在</returns>
        private bool QueryWxOrder(string transactionId)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transactionId);
            WxPayData res = WxPayApi.OrderQuery(req);
            return res.GetValue("return_code").ToString() == "SUCCESS" &&
                   res.GetValue("result_code").ToString() == "SUCCESS";
        }
        /// <summary>
        /// 返回微信回调处理结果
        /// </summary>
        /// <param name="code">类型</param>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        private ContentResult ReturnWxContent(string code, string msg)
        {
            var loger = LogManager.GetLogger("wxpay");
            WxPayData res = new WxPayData();
            res.SetValue("return_code", code);
            res.SetValue("return_msg", msg);
            loger.Info(this.GetType().ToString(), "logs : " + res.ToXml());
            return Content(res.ToXml());
        }
    }
}