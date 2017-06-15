using NiitBlog.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class ArticleRepositories : IArticleRepositories
    {
        private readonly BlogEntities db = null;

        public ArticleRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public int GetUIDbyAID(int aid)
        {
            var s = db.Articles.Where(n => n.AID == aid).FirstOrDefault();
            if (s != null)
                return s.UID;
            else
                return -1;
        }

        public Articles GetArticleByAID(int aid)
        {
            return db.Articles.Where(n => n.AID == aid).FirstOrDefault();
        }

        public int GetArticlesCountByUID(int uid)
        {
            return db.Articles.Where(n => n.UID == uid).Count();
        }

        public int GetArticlesCountByUIDandCID(int uid, int cid)
        {
            return db.Articles.Where(n => n.UID == uid && n.CID == cid).Count();
        }

        public Dictionary<int, string> GetPreAndNextArticleIdAndTitle(Articles article)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            if (article != null)
            {
                var query = db.Articles.Where(n => n.UID == article.UID).OrderByDescending(n => n.CreateTime);
                var q1 = query.FirstOrDefault(n => n.CreateTime < article.CreateTime);
                if (q1 != null)
                    dictionary.Add(q1.AID, q1.Title);
                else
                    dictionary.Add(-1, "没有了");
                query = db.Articles.Where(n => n.UID == article.UID).OrderBy(n => n.CreateTime);
                var q2 = query.FirstOrDefault(n => n.CreateTime > article.CreateTime);
                if (q2 != null)
                    dictionary.Add(q2.AID, q2.Title);
                else
                    dictionary.Add(0, "没有了");
            }
            return dictionary;
        }

        public List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting)
        {
            IEnumerable<Articles> query = db.Articles.Where(n => n.UID == uid).ToList();
            if (!string.IsNullOrEmpty(sorting))
            {
                string[] sort = sorting.Split(' ');
                if (sort[1].ToUpper() == "ASC")
                {
                    query = query.OrderBy(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
                else if (sort[1].ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
            }
            else
            {
                query.OrderBy(u => u.AID);
            }
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }


        public int GetArticlesCountByUIDCID(int uid, int cid)
        {
            return (from a in db.Articles
                    where a.CID == cid && a.UID == uid
                    select a).Count();
        }

        public List<Articles> GetArticles1(int uid, int PageIndex, int ListItemNum, string sorting, int cid)
        {
            IEnumerable<Articles> query = (from a in db.Articles
                                           where a.CID == cid && a.UID == uid
                                           select a).ToList();
            if (!string.IsNullOrEmpty(sorting))
            {
                string[] sort = sorting.Split(' ');
                if (sort[1].ToUpper() == "ASC")
                {
                    query = query.OrderBy(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
                else if (sort[1].ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
            }
            else
            {
                query.OrderBy(u => u.AID);
            }
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }

        public int GetArticlesCountByUIDTID(int uid, int tid)
        {
            return (from a in db.Articles
                    join b in db.ArticleTagMapping on a.AID equals b.AID
                    where b.TID == tid && a.UID == uid
                    select a).Count();
        }

        public List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting, int tid)
        {
            IEnumerable<Articles> query = (from a in db.Articles
                                           join b in db.ArticleTagMapping on a.AID equals b.AID
                                           where b.TID == tid && a.UID == uid
                                           select a).ToList();
            if (!string.IsNullOrEmpty(sorting))
            {
                string[] sort = sorting.Split(' ');
                if (sort[1].ToUpper() == "ASC")
                {
                    query = query.OrderBy(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
                else if (sort[1].ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
            }
            else
            {
                query.OrderBy(u => u.AID);
            }
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }

        public int GetArticlesCountByUIDYM(int uid, int y, int m)
        {
            return db.Articles.Where(n => n.UID == uid && n.CreateTime.Year == y && n.CreateTime.Month == m).Count();
        }

        public List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting, int y, int m)
        {
            IEnumerable<Articles> query = db.Articles.Where(n => n.UID == uid && n.CreateTime.Year == y && n.CreateTime.Month == m).ToList();
            if (!string.IsNullOrEmpty(sorting))
            {
                string[] sort = sorting.Split(' ');
                if (sort[1].ToUpper() == "ASC")
                {
                    query = query.OrderBy(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
                else if (sort[1].ToUpper() == "DESC")
                {
                    query = query.OrderByDescending(u => ModelHelper.GetPropertyValue(u, sort[0]));
                }
            }
            else
            {
                query.OrderBy(u => u.AID);
            }
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }

        public List<ArticleArchivesViewModel> GetArticleArchives(int uid)
        {
            var grouped = (from p in db.Articles
                           where p.UID == uid
                           group p by new
                           {
                               month = p.CreateTime.Month,
                               year = p.CreateTime.Year
                           } into d
                           select new ArticleArchivesViewModel { Year = d.Key.year, Month = d.Key.month, Count = d.Count() })
                           .OrderByDescending(g => g.Year)
                           .ThenByDescending(g => g.Month);
            return grouped.ToList();
        }

        public List<Articles> ReadTop(int uid, int topnum)
        {
            return db.Articles.Where(n => n.UID == uid).OrderByDescending(n => n.ViewNum).Take(5).ToList();
        }

        public void ViewNumPlusOne(int aid)
        {
            var s = db.Articles.Where(n => n.AID == aid).FirstOrDefault();
            if (s != null)
            {
                s.ViewNum += 1;
                db.SaveChanges();
            }
        }

        public void CommentNumPlusOne(int aid)
        {
            var s = db.Articles.Where(n => n.AID == aid).FirstOrDefault();
            if (s != null)
            {
                s.CommentNum += 1;
                db.SaveChanges();
            }
        }

        public void CommentNumSubtract(int aid, int num)
        {
            var s = db.Articles.Where(n => n.AID == aid).FirstOrDefault();
            if (s != null)
            {
                s.CommentNum -= num;
                db.SaveChanges();
            }
        }

        public List<CommentsViewModel> GetCommentsByAid(int aid, int uid)
        {
            var query = from a in db.Articles
                        join b in db.Comments on a.AID equals b.AID
                        join c in db.Users on b.UID equals c.UID
                        where a.AID == aid
                        select new CommentsViewModel { Id = b.ID, Author = c.UserName, Comment = b.Content, UserAvatar = c.HeadPic, CanDelete = uid == b.UID ? true : false, CanReplay = true, TempDate = b.CreateTime, ParentId = b.ParentId };
            var ret = query.ToList();
            return ret;
        }


        public void AddArticle(Articles article, string tags)
        {
            db.Articles.Add(article);
            if (tags != "")
            {
                string[] itags = tags.Split(',');
                foreach (string t in itags)
                {
                    var query = db.Tags.Where(n => n.UID == article.UID).Where(n => n.TName.Equals(t, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (query != null)
                    {
                        db.ArticleTagMapping.Add(new ArticleTagMapping { TID = query.TID, Description = "", AID = article.AID });
                        db.SaveChanges();
                    }
                    else
                    {
                        var tag = new Tags { TName = t, UID = article.UID };
                        db.Tags.Add(tag);
                        db.ArticleTagMapping.Add(new ArticleTagMapping { TID = tag.TID, Description = "", AID = article.AID });
                        db.SaveChanges();
                    }
                }
            }
            db.SaveChanges();
        }

        public void UpdateArticle(Articles article, string tags)
        {
            var s = db.Articles.Where(n => n.AID == article.AID).First();
            if (s != null)
            {
                s.Title = article.Title;
                s.Content = article.Content;
                s.CID = article.CID;
                s.Summery = article.Summery;
                if (tags != "")
                {
                    string[] itags = tags.Split(',');
                    foreach (string t in itags)
                    {
                        var query = db.Tags.Where(n => n.UID == article.UID).Where(n => n.TName.Equals(t, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (query != null)
                        {
                            if (db.ArticleTagMapping.Where(n => n.AID == article.AID && n.TID == query.TID).FirstOrDefault() == null)
                            {
                                db.ArticleTagMapping.Add(new ArticleTagMapping { TID = query.TID, Description = "", AID = article.AID });
                            }
                        }
                        else
                        {
                            var tag = new Tags { TName = t, UID = article.UID };
                            db.Tags.Add(tag);
                            db.ArticleTagMapping.Add(new ArticleTagMapping { TID = tag.TID, Description = "", AID = article.AID });
                        }
                    }
                }
            }
            db.SaveChanges();
        }

        public void DeleteArticle(int aid, int uid)
        {
            var article = db.Articles.Where(n => n.AID == aid && n.UID == uid).FirstOrDefault();
            if (article != null)
            {
                db.Articles.Remove(article);
                db.SaveChanges();
            }
        }

        public IQueryable<Articles> GetLatestArticle(int ListItemNum)
        {
            return db.Articles.OrderByDescending(n => n.CreateTime).Take(ListItemNum);
        }
    }
}