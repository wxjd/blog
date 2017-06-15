using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public class UserRoleRepositories:IUserRoleRepositories
    {
        private readonly BlogEntities db = null;

        public UserRoleRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }

        public List<UserRoles> GetAllUserRole()
        {
            return db.UserRoles.ToList();
        }
    }
}