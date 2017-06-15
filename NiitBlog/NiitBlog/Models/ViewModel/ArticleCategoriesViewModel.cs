using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class ArticleCategoriesViewModel
    {
        public int UID { get; set; }
        public int CID { get; set; }
        public string CName { get; set; }
        public int Count { get; set; }
    }
}