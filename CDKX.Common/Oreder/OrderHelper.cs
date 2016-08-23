using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CDKX.Common.Oreder
{
    /// <summary>
    /// 订单助手
    /// </summary>
    public class OrderHelper
    {
        /// <summary>
        /// 防止创建类的实例
        /// </summary>
        private OrderHelper() { }

        private static readonly object Locker = new object();
        private static int _sn = 0;

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns></returns>
        public static string GenerateId(string code)
        {
            lock (Locker)  //lock 关键字可确保当一个线程位于代码的临界区时，另一个线程不会进入该临界区。
            {
                if (_sn == int.MaxValue)
                {
                    _sn = 0;
                }
                else
                {
                    _sn++;
                }
                Thread.Sleep(100);
                return code + DateTime.Now.ToString("yyyyMMddHHmmss") + _sn.ToString().PadLeft(10, '0');
            }
        }
    }
}
