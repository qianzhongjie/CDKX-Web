using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDKX.Web.Areas.Api.Models
{
    public class BaseHotKey
    {

        public string Key { get; set; }
        public string Value { get; set; }
        public int Count { get; set; }
    }
}