
using OSharp.Data.Entity.Migrations;
using System.Collections.Generic;

namespace CDKX.Services.Implement.DbSeed
{
    public class CreateCitySeedAction : ISeedAction
    {
        /// <summary>
     /// 定义种子数据初始化过程
     /// </summary>
     /// <param name="context">数据上下文</param>
        public void Action(System.Data.Entity.DbContext context)
        {
            
        }

        /// <summary>
        /// 获取 操作排序，数值越小越先执行
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
