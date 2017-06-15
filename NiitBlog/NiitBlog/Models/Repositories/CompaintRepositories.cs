using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class CompaintRepositories : ICompaintRepositories
    {
         private readonly BlogEntities db = null;

        public CompaintRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public void AddCompaint(Complaint complaint)
        {
            db.Complaint.Add(complaint);
            db.SaveChanges();
        }
    }
}