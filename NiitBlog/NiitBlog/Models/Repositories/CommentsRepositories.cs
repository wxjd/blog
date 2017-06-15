using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class CommentsRepositories:ICommentsRepositories
    {
         private readonly BlogEntities db = null;

         public CommentsRepositories(BlogEntities BlogEntities)
         {
             this.db = BlogEntities;
         }

        public CommentsViewModel AddComment(Comments comments)
        {
            Comments commentadded = db.Comments.Add(comments);
            db.SaveChanges();

            var u=db.Users.Where(n=>n.UID==commentadded.UID).First();
            return new CommentsViewModel { Id = commentadded.ID, Author = u.UserName, Comment = commentadded.Content, ParentId = commentadded.ParentId, UserAvatar = u.HeadPic, CanDelete = true, CanReplay = true, Date = commentadded.CreateTime.ToString() };
        }

     
        public int DeleteComment(int id,int uid)
        {
            int i=0;
            var s = db.Comments.Where(n => n.ID == id).FirstOrDefault();
            if (s != null)
            {
                if (s.UID == uid)
                {
                    
                    i= db.proc_delete_comments(id);
                }
            }
            return i;
        }
    }
}