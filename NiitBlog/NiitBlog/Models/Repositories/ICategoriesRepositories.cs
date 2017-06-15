using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface ICategoriesRepositories
    {
        int GetCategoryCountByUID(int uid);
        List<Categories> GetCategories(int uid, int PageIndex, int ListItemNum);
        IQueryable<Categories> GetCategoriesByUID(int uid);
        int AddCategories(Categories categories);
        bool CategoryExist(string cname,int uid);
        void DeleteCategory(int uid, int cid);
        void UpdateCategory(Categories category);
        IQueryable<ArticleCategoriesViewModel> GetArticleCategories(int uid);
    }
}