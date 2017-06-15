using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class CommentsViewModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public int? ParentId { get; set; }
        public string UserAvatar { get; set; }
        public bool CanDelete { get; set; }
        public bool CanReplay { get; set; }
        public string Date { get; set; }
        public DateTime TempDate { get; set; }
    }
}