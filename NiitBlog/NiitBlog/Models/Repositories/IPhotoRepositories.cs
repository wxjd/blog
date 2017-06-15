using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface IPhotoRepositories
    {
        int GetPhotoCountByUID(int uid);
        IQueryable<Photos> GetPhotosByAlbumId(int albumId);
        Photos Getphoto(int pid);
        Photos AddPhoto(Photos photo);
        void DeletePhoto(int uid, int pid);
        void UpdatePhoto(Photos photo);
        List<Photos> GetPhotos(int uid, int PageIndex, int ListItemNum, string sorting);
        List<CommentsViewModel> GetCommentsByPid(int pid, int uid);
        IQueryable<Photos> GetPhotoList(int page, int ListItemNum);
    }
}