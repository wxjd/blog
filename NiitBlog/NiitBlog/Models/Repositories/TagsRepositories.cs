using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class TagsRepositories : ITagsRepositories
    {
         private readonly BlogEntities db = null;

         public TagsRepositories(BlogEntities BlogEntities)
         {
             this.db = BlogEntities;
         }


        public int UserTagsCount(int uid)
        {
           return  db.Tags.Where(n => n.UID == uid).Count();
        }

        public IQueryable<HotTagsViewModel> GetUserTag(int uid)
        {
            return from b in db.ArticleTagMapping
                   join a in db.Tags on b.TID equals a.TID
                   where a.UID == uid
                   group a by new { UID = a.UID, TID = a.TID, TName = a.TName } into d
                   orderby d.Count() descending 
                   select new HotTagsViewModel { UID = d.Key.UID, TID = d.Key.TID, TName = d.Key.TName, ArticeCount = d.Count() };
                              
        }

        public List<Tags> GetTags(int uid, int PageIndex, int ListItemNum)
        {
            IEnumerable<Tags> query = db.Tags.Where(n => n.UID == uid).ToList();
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }

        public void DeleteTag(int uid, int tid)
        {
            var tag = db.Tags.Where(n => n.UID == uid && n.TID == tid).FirstOrDefault();
            if (tag != null)
            {
                db.Tags.Remove(tag);
                db.SaveChanges();
            }
        }

        public void UpdateTag(Tags tag)
        {
            var t = db.Tags.Where(n => n.TID == tag.TID).FirstOrDefault();
            if (t != null)
            {
                t.TName = tag.TName;
                db.SaveChanges();
            }
        }
        //============================================================TagMapping
        public List<string> ArticleTags(int aid)
        {
            return db.ArticleTagMapping.Where(n => n.AID == aid).Select(n => n.Tags.TName).ToList();
        }

        public void DeleteArticleTag(string tname, int aid,int uid)
        {
            var tag= db.Tags.Where(n => n.TName == tname&& n.UID==uid).FirstOrDefault();
            if (tag != null)
            {
                var articletag = db.ArticleTagMapping.Where(n => n.AID == aid && n.TID == tag.TID).FirstOrDefault();
                if (articletag != null)
                {
                    db.ArticleTagMapping.Remove(articletag);
                    db.SaveChanges();
                }
            }
        }
    }
}