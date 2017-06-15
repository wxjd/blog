using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class PhotoRepositories:IPhotoRepositories
    {
        private readonly BlogEntities db = null;

        public PhotoRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public int GetPhotoCountByUID(int uid)
        {
            return db.Photos.Where(n => n.UID == uid).Count();
        }

        public IQueryable<Photos> GetPhotosByAlbumId(int albumId)
        {
            return db.Photos.Where(n => n.AlbumID == albumId);
        }

        public Photos Getphoto(int pid)
        {
            return db.Photos.Where(n => n.PID == pid).FirstOrDefault();
        }

        public Photos AddPhoto(Photos photo)
        {
            var p = db.Photos.Add(photo);
            db.SaveChanges();
            return p;
        }

        public void DeletePhoto(int uid, int pid)
        {
            var photo = db.Photos.Where(n => n.UID == uid && n.PID == pid).FirstOrDefault();
            if (photo != null)
            {
                db.Photos.Remove(photo);
                db.SaveChanges();
            }
        }

        public void UpdatePhoto(Photos photo)
        {
            var pho = db.Photos.Where(n => n.PID == photo.PID).FirstOrDefault();
            if (pho != null)
            {
                pho.PhotoName = photo.PhotoName;
                pho.Path = photo.Path;
                pho.Thumbnail = photo.Thumbnail;
                pho.Thumbnailw = photo.Thumbnailw;
                pho.Thumbnailw_width = photo.Thumbnailw_width;
                pho.Thumbnailw_height = photo.Thumbnailw_height;
                pho.AlbumID = photo.AlbumID;
                pho.Description = photo.Description;
                db.SaveChanges();
            }
        }

        public List<Photos> GetPhotos(int uid, int PageIndex, int ListItemNum, string sorting)
        {
            IEnumerable<Photos> query = db.Photos.Where(n => n.UID == uid).ToList();
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


        public List<CommentsViewModel> GetCommentsByPid(int pid, int uid)
        {
            var query = from a in db.Photos
                        join b in db.Comments on a.PID equals b.PID
                        join c in db.Users on b.UID equals c.UID
                        where a.PID == pid
                        select new CommentsViewModel { Id = b.ID, Author = c.UserName, Comment = b.Content, UserAvatar = c.HeadPic, CanDelete = uid == b.UID ? true : false, CanReplay = true, TempDate = b.CreateTime, ParentId = b.ParentId };
            var ret = query.ToList();
            return ret;
        }

        public IQueryable<Photos> GetPhotoList(int page, int ListItemNum)
        {
            return db.Photos.OrderByDescending(n => n.CreateTime).Skip((page - 1) * ListItemNum).Take(ListItemNum);
        }
    }

}