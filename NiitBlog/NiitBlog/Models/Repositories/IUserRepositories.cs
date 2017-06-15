using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiitBlog.Models
{
    public interface IUserRepositories
    {
        Users GetUserByUID(Users users);
        Users GetUserByUserName(Users users);
        Users GetUserByEmail(Users users);
        int ValidateUser(Users users);
        void AddUser(Users users);
        void ChangePassWord(Users users);
        void UpdateHeadPic(Users users);
        void UpdateProfile(Users users);
        void UpdateUserStatusAndMid(Users users);
        bool UserNameNotExist(string username);
        bool EmailNotExist(string email);
    }
}
