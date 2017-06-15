using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class CategoriesRepositories : ICategoriesRepositories
    {
        private readonly BlogEntities db = null;

        public CategoriesRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public IQueryable<Categories> GetCategoriesByUID(int uid)
        {
            return db.Categories.Where(n => n.UID == uid);
        }

        public int GetCategoryCountByUID(int uid)
        {
            return db.Categories.Where(n => n.UID == uid).Count();
        }

        public List<Categories> GetCategories(int uid, int PageIndex, int ListItemNum)
        {
            IEnumerable<Categories> query = db.Categories.Where(n => n.UID == uid).ToList();
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }

        public IQueryable<ArticleCategoriesViewModel> GetArticleCategories(int uid)
        {
            return from b in db.Articles
                   join a in db.Categories on b.CID equals a.CID
                   where a.UID == uid
                   group a by new { UID = a.UID, CID = a.CID, CName = a.CName } into d
                   orderby d.Count() descending
                   select new ArticleCategoriesViewModel { UID = d.Key.UID, CID = d.Key.CID, CName = d.Key.CName, Count = d.Count() };

        }

        public void DeleteCategory(int uid, int cid)
        {
            var category=db.Categories.Where(n=>n.UID ==uid&&n.CID==cid).FirstOrDefault();
            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
        }

        public int AddCategories(Categories categories)
        {
            var c= db.Categories.Add(categories);
            db.SaveChanges();
            return c.CID;
        }

        public bool CategoryExist(string cname,int uid)
        {
            return db.Categories.Where(n => n.CName == cname&& n.UID==uid).FirstOrDefault() == null ? true : false;
        }

        public void UpdateCategory(Categories category)
        {
            var cat = db.Categories.Where(n => n.CID == category.CID).FirstOrDefault();
            if (cat != null) 
            {
                cat.CName = category.CName;
                db.SaveChanges();
            }
        }
    }
}