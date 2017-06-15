using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class PhotoValidation:AbstractValidator<Photos>
    {
        public PhotoValidation()
        {
            RuleFor(u => u.PhotoName).NotNull().Length(1, 20).WithMessage("照片名不为空且最多为20个字符");
            RuleFor(u => u.Description).Length(0, 150).WithMessage("照片描述最多为150个字符");
        }
    }
}