using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiitBlog.Models.Repositories
{
    public interface IArticleRepositories
    {

        int GetArticlesCountByUIDCID(int uid, int cid);
        List<Articles> GetArticles1(int uid, int PageIndex, int ListItemNum, string sorting, int cid);
        int GetArticlesCountByUIDandCID(int uid, int cid);
        Dictionary<int, string> GetPreAndNextArticleIdAndTitle(Articles article);
        /// <summary>
        /// 根据文章编号取文章实体
        /// </summary>
        /// <param name="aid">文章编号</param>
        /// <returns>文章实体</returns>
        Articles GetArticleByAID(int aid);
        /// <summary>
        /// 存在返回编号 不存在返回-1 
        /// </summary>
        /// <param name="aid">文章编号</param>
        /// <returns>用户编号</returns>
        int GetUIDbyAID(int aid);
        int GetArticlesCountByUID(int uid);
        List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting);
        List<Articles> ReadTop(int uid, int topnum);
        /// <summary>
        /// 浏览数加1
        /// </summary>
        /// <param name="aid">文章编号</param>
        void ViewNumPlusOne(int aid);
        /// <summary>
        /// 获取文章归档
        /// </summary>
        /// <param name="uid">用户编号</param>
        /// <returns></returns>
        List<ArticleArchivesViewModel> GetArticleArchives(int uid);
        /// <summary>
        /// 根据用户编号的tag编号获取文章总数
        /// </summary>
        /// <param name="uid">用户编号</param>
        /// <param name="tid">tag编号</param>
        /// <returns>文章数</returns>
        int GetArticlesCountByUIDTID(int uid, int tid);
        List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting, int tid);
        /// <summary>
        /// 获取N年M月某用户的文章总数
        /// </summary>
        /// <param name="uid">用户编号</param>
        /// <param name="y">年</param>
        /// <param name="m">月</param>
        /// <returns>文章数</returns>
        int GetArticlesCountByUIDYM(int uid, int y, int m);
        List<Articles> GetArticles(int uid, int PageIndex, int ListItemNum, string sorting, int y, int m);


        List<CommentsViewModel> GetCommentsByAid(int aid, int uid);
        void CommentNumPlusOne(int aid);
        void CommentNumSubtract(int aid, int num);
        void AddArticle(Articles article, string tags);
        void UpdateArticle(Articles article, string tags);
        void DeleteArticle(int aid, int uid);
        IQueryable<Articles> GetLatestArticle(int ListItemNum);
    }
}
