using NiitBlog.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiitBlog.Models
{
    public interface IRepositoryContainer
    {
        IUserRepositories _UserRepositories { get; }
        IUserRoleRepositories _UserRoleRepositories { get; }
        ICompaintRepositories _CompaintRepositories { get; }
        ITagsRepositories _TagsRepositories { get; }
        IArticleRepositories _ArticleRepositories { get; }
        ICommentsRepositories _CommentsRepositories { get; }
        ICategoriesRepositories _CategoriesRepositories { get; }
        IAlbumRepositories _AlbumRepositories { get; }
        IPhotoRepositories _PhotoRepositories { get; }
    }
}
