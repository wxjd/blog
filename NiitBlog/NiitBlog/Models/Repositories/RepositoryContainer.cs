using NiitBlog.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class RepositoryContainer : IRepositoryContainer, IDisposable
    {
        private BlogEntities db = null;

        public RepositoryContainer()
        {
            this.db = new BlogEntities();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                db = null;
            }
        }

        ~RepositoryContainer()
        {
            Dispose(false);
        }


        public IUserRepositories _UserRepositories
        {
            get { return new UserRepositories(db); }
        }

        public IUserRoleRepositories _UserRoleRepositories
        {
            get { return new UserRoleRepositories(db); }
        }


        public ICompaintRepositories _CompaintRepositories
        {
            get { return new CompaintRepositories(db); }
        }


        public ITagsRepositories _TagsRepositories
        {
            get { return new TagsRepositories(db); }
        }


        public IArticleRepositories _ArticleRepositories
        {
            get { return new ArticleRepositories(db); }
        }


        public ICommentsRepositories _CommentsRepositories
        {
            get { return new CommentsRepositories(db); }
        }

        public ICategoriesRepositories _CategoriesRepositories
        {
            get { return new CategoriesRepositories(db); }
        }


        public IAlbumRepositories _AlbumRepositories
        {
            get { return new AlbumRepositories(db); }
        }


        public IPhotoRepositories _PhotoRepositories
        {
            get { return new PhotoRepositories(db); }
        }
    }
}