using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface IAlbumRepositories
    {
        IQueryable<Albums> GetAlbumsByUid(int uid);
        Albums GetAlbum(int albumid);
        Albums AddAlbum(Albums album);
        void DeleteAlbum(int uid, int albumid);
        void UpdateAlbum(Albums album);
        List<Albums> GetAlbums(int uid, int PageIndex, int ListItemNum, string sorting);
    }
}