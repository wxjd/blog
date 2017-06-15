using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class AlbumRepositories : IAlbumRepositories
    {
        private readonly BlogEntities db = null;

        public AlbumRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public IQueryable<Albums> GetAlbumsByUid(int uid)
        {
            return db.Albums.Where(n => n.UID == uid);
        }

        public Albums GetAlbum(int albumid)
        {
            return db.Albums.Where(n => n.AlbumID == albumid).FirstOrDefault();
        }


        public Albums AddAlbum(Albums album)
        {
            var a=  db.Albums.Add(album);
            db.SaveChanges();
            return a;
        }


        public void DeleteAlbum(int uid, int albumid)
        {
            var album = db.Albums.Where(n => n.UID == uid && n.AlbumID == albumid).FirstOrDefault();
            if (album != null)
            {
                db.Albums.Remove(album);
                db.SaveChanges();
            }
        }

        public void UpdateAlbum(Albums album)
        {
            var alb = db.Albums.Where(n => n.AlbumID == album.AlbumID).FirstOrDefault();
            if (alb != null)
            {
                alb.AlbumName = album.AlbumName;
                alb.Description = album.Description;
                alb.CoverPath = album.CoverPath;
                db.SaveChanges();
            }
        }

        public List<Albums> GetAlbums(int uid, int PageIndex, int ListItemNum, string sorting)
        {
            IEnumerable<Albums> query = db.Albums.Where(n => n.UID == uid).ToList();
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
                query.OrderBy(u => u.AlbumID);
            }
            var ret = ListItemNum > 0 ?
                query
                .Skip((PageIndex) * ListItemNum)
                .Take(ListItemNum).ToList() : query.ToList(); //No paging
            return ret;
        }
    }
}