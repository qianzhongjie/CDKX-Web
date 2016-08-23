using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDKX.Web.Areas.Admin.ViewModels
{
    public class HashNode
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public IEnumerable<HashNode> Childrens { get; set; }
    }
}