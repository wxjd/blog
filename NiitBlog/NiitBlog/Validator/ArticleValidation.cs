using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class ArticleValidation : AbstractValidator<Articles>
    {
        public ArticleValidation()
        {
            RuleFor(u => u.Title).NotNull().Length(1,100).WithMessage("标题不为空且最长为100个字符");
            RuleFor(u => u.Summery).NotNull().Length(1, 500).WithMessage("摘要不为空且最长为500个字符");
        }
    }
}