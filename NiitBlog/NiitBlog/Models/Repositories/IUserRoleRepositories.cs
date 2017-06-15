using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiitBlog.Models
{
    public interface IUserRoleRepositories
    {
        List<UserRoles> GetAllUserRole();
    }
}