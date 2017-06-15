using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class HotTagsViewModel
    {
        public int UID { get; set; }
        public int TID { get; set; }
        public string TName { get; set; }
        public int ArticeCount { get; set; }
    }
}