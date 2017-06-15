using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NiitBlog.Common;

namespace NiitBlog.Models
{
    public class UserRepositories : IUserRepositories
    {

        private readonly BlogEntities db = null;

        public UserRepositories(BlogEntities BlogEntities)
        {
            this.db = BlogEntities;
        }


        public Users GetUserByUID(Users users)
        {
            return db.Users.Where(n => n.UID == users.UID).FirstOrDefault();
        }

        public Users GetUserByUserName(Users users)
        {
            return db.Users.Where(n => n.UserName == users.UserName).FirstOrDefault();
        }

        public Users GetUserByEmail(Users users)
        {
            return db.Users.Where(n => n.Email == users.Email).FirstOrDefault();
        }

        public void AddUser(Users users)
        {
            db.Users.Add(users);
            db.SaveChanges();
        }

        public int ValidateUser(Users users)
        {
            var user = db.Users.Where(u => u.UserName == users.UserName).FirstOrDefault();
            if (user != null)
            {
                if (user.Password == Encrypt.MD5Encrypt(users.Password))
                {

                    if (user.Status == 1)
                    {
                        return 3; //验证成功
                    }
                    else
                    {
                        return 2; //用户未激活
                    }
                }
                else
                {
                    return 1; //密码错误
                }
            }
            else
            {
                return 0; //用户不存在
            }
        }


        public void ChangePassWord(Users users)
        {
            var foundUser = db.Users.FirstOrDefault(s => s.UID == users.UID);
            if (foundUser == null)
            {
                return;
            }

            foundUser.Password = users.Password;

            db.SaveChanges();
        }

        public void UpdateHeadPic(Users users)
        {
            var foundUser = db.Users.FirstOrDefault(s => s.UID == users.UID);
            if (foundUser == null)
            {
                return;
            }

            foundUser.HeadPic = users.HeadPic;

            db.SaveChanges();
        }

        public void UpdateProfile(Users users)
        {
            var foundUser = db.Users.FirstOrDefault(s => s.UID == users.UID);
            if (foundUser == null)
            {
                return;
            }

            foundUser.NickName = users.NickName;
            foundUser.SelfIntro = users.SelfIntro;
            foundUser.Description = users.Description;
            foundUser.Gender = users.Gender;

            db.SaveChanges();
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="users"></param>
        public void UpdateUserStatusAndMid(Users users)
        {
            var foundUser = db.Users.FirstOrDefault(s => s.UID == users.UID);
            if (foundUser == null)
            {
                return;
            }

            foundUser.Status = users.Status;
            foundUser.Mid = users.Mid;
            foundUser.ActiveTime = DateTime.Now;

            db.SaveChanges();
        }


        public bool UserNameNotExist(string username)
        {
            return db.Users.Where(n => n.UserName == username).FirstOrDefault() == null ? true : false;
        }

        public bool EmailNotExist(string email)
        {
            return db.Users.Where(n => n.Email == email).FirstOrDefault() == null ? true : false;
        }
    }
}