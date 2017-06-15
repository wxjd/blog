using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class LatestArticleViewModel
    {

        public int AID { get; set; }
        public string Title { get; set; }
        public string Summery { get; set; }
        public DateTime CreateTime { get; set; }
        public int? CommentNum { get; set; }
        public int? ViewNum { get; set; }
        public string UserName { get; set; }
        public string HeadPic { get; set; }
    }
}