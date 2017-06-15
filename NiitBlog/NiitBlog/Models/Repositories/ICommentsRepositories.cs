using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface ICommentsRepositories
    {
        CommentsViewModel AddComment(Comments comments);
        /// <summary>
        /// 删除用户评论
        /// </summary>
        /// <param name="id">评论编号</param>
        /// <param name="uid">用户编号</param>
        /// <returns>影响行数</returns>
        int DeleteComment(int id, int uid);
    }
}