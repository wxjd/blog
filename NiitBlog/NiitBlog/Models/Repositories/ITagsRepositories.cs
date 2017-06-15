using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface ITagsRepositories
    {
        void DeleteArticleTag(string tname, int aid,int uid);
        int UserTagsCount(int uid);
        IQueryable<HotTagsViewModel> GetUserTag(int uid);
        List<string> ArticleTags(int aid);
        List<Tags> GetTags(int uid, int PageIndex, int ListItemNum);
        void DeleteTag(int uid, int tid);
        void UpdateTag(Tags tag);
    }
}