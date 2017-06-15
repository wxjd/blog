using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class AlbumsViewModel
    {
        public int AlbumID { get; set; }
        public string AlbumName { get; set; }
        public string Description { get; set; }
        public string CoverPath { get; set; }
        public Nullable<int> PhotoNum { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public int UID { get; set; }
        public int column { get; set; }
    }
}