using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDKX.Web.Areas.Web.Models
{
    public class Comment
    {
        public int UserId { get; set; }
        public string NickName { get; set; }
        public string HeadPic { get; set; }
        public decimal AverageGrade { get; set; }
        public DateTime? CreatedTime { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 评论图片
        /// </summary>
        public List<Pictures> Pictures { get; set; }
        /// <summary>
        /// 商品颜色
        /// </summary>
        public string ColorName { get; set; }
        /// <summary>
        /// 商品尺码
        /// </summary>
        public string SizeValue { get; set; }

        /// <summary>
        /// 平台回复
        /// </summary>
        public string Reply { get; set; }
    }

    public class Pictures
    {
        public string Path { get; set; }
        public int OrderNo { get; set; }
    }
}