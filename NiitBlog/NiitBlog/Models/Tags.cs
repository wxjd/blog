//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NiitBlog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tags
    {
        public Tags()
        {
            this.ArticleTagMapping = new HashSet<ArticleTagMapping>();
        }
    
        public int TID { get; set; }
        public string TName { get; set; }
        public int UID { get; set; }
    
        public virtual ICollection<ArticleTagMapping> ArticleTagMapping { get; set; }
        public virtual Users Users { get; set; }
    }
}
