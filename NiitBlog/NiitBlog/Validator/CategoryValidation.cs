using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class CategoryValidation:AbstractValidator<Categories>
    {
        public CategoryValidation() 
        {
            RuleFor(u => u.CName).NotNull().Length(1, 30).WithMessage("分类名不为空且最长为30个字符");
        }
    }
}