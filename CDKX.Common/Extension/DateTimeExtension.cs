using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDKX.Common.Extension
{
    public static class DateTimeExtension
    {

        public enum Accurate
        {
            天 = 0,
            小时 = 1,
            分钟 = 2,
            秒 = 3,
        }

        public static string DateDiff(this DateTime DateTime1, DateTime DateTime2, Accurate accurate)
        {
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days.ToString() + "天";
            switch (accurate)
            {
                case Accurate.天:
                    return dateDiff;
                case Accurate.小时:
                    return dateDiff + ts.Hours.ToString() + "小时";
                case Accurate.分钟:
                    return dateDiff + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟";
                case Accurate.秒:
                    return dateDiff + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
                default:
                    return dateDiff + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            }
        }
    }
}
