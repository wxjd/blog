using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class ComplaintValidation:AbstractValidator<Complaint>
    {
        public ComplaintValidation() 
        {
            RuleFor(u => u.title).NotNull().Length(1, 150).WithMessage("标题不为空且最长为150个字符");
            RuleFor(u => u.text).NotNull().Length(1, 4000).WithMessage("内容不为空且最长为4000个字符");
            RuleFor(u => u.email).NotNull().EmailAddress().WithMessage("邮件地址错误");
        }
    }
}