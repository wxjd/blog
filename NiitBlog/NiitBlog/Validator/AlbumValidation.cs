using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using NiitBlog.Models;

namespace NiitBlog.Validator
{
    public class AlbumValidation:AbstractValidator<Albums>
    {
        public AlbumValidation()
        {
            RuleFor(u => u.AlbumName).NotNull().Length(1, 20).WithMessage("相册名不为空且最多为20个字符");
            RuleFor(u => u.Description).Length(0, 150).WithMessage("相册描述最多为150个字符");
        }
    }
}