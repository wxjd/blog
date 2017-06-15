using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VerificationCode;

namespace NiitBlog.Controllers
{
    public class VerificationCodeController : Controller
    {
        //验证码<img alt="验证码" src="/VerificationCode/Index" id="code_img" title="看不清请点击！" />
        // GET: /VerificationCode/

        public ActionResult Index()
        {
            VerifyCode vc = new VerifyCode();
            string num = vc.MakeValidateCode();
            System.Web.HttpContext.Current.Session["code"] = num.ToLower();//写入session
            byte[] bytes = vc.CreateCheckCodeImage(num);
            return File(bytes, @"image/jpeg");
        }
    }
}
