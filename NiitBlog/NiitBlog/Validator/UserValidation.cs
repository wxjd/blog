using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class UserValidation : AbstractValidator<Users> 
    {
        public UserValidation()
        {
            RuleFor(u => u.UserName).Matches(@"^[a-zA-Z][\w]{4,11}$").WithMessage("用户名不合法");
            RuleFor(u => u.Password).Matches(@"^[\w]{6,15}$").WithMessage("密码不合法");
            RuleFor(u => u.Email).EmailAddress().WithMessage("电子邮件地址错误");
            RuleFor(u => u.SelfIntro).Length(0, 150).WithMessage("自我介绍长度在0到150之间");
            RuleFor(u => u.Description).Length(0, 150).WithMessage("个人描述长度在0到150之间");
            RuleFor(u => u.HeadPic).Length(0, 100).WithMessage("头像url地址长度在0到100之间");
            RuleFor(u => u.NickName).Length(3, 50).WithMessage("昵称长度在3到50之间");
            RuleFor(u => u.Gender).Length(0, 4).Matches(@"^男|女|保密$").WithMessage("请选择性别");
        }
    }
}