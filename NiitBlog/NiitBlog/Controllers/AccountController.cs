using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using NiitBlog.Models;
using NiitBlog.Common;
using System.Text;
using NiitBlog.Validator;
using FluentValidation.Results;

namespace NiitBlog.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IRepositoryContainer _repository;

        public AccountController()
        {
            this._repository = new RepositoryContainer();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User", new { username = HttpContext.User.Identity.Name });
            }
            var ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer == null ? "/Home/Index" :
                (System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri == System.Web.HttpContext.Current.Request.Url.AbsoluteUri ? "/Home/Index" : System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri);
            if (System.Web.HttpContext.Current.Request.UrlReferrer != null)
            {
                List<string> url = new List<string>();
                url.Add("/Account/Register");
                url.Add("/Account/GetPassword");
                url.Add("/Account/ForgetPassword");
                url.Add("/Account/Activation");
                url.Add("/Manage");
                url.Add("/Post");
                url.Add("/PostEdit");
                foreach (string uri in url)
                {
                    if (System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri.Contains(uri))
                    {
                        ReturnUrl = "/Home/Index";
                    }
                }
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAjax(string ReturnUrl)
        {
            Users users = new Users { UserName = Request["username"], Password = Request["password"] };
            var res = _repository._UserRepositories.ValidateUser(users);
            if (res == 0)
            {
                return Json(new { Result = "Error", Message = "用户不存在" });
            }
            else if (res == 1)
            {
                return Json(new { Result = "Error", Message = "密码错误" });
            }
            else if (res == 2)
            {
                return Json(new { Result = "Error", Message = "帐号未激活" });
            }
            else
            {
                bool PersistentCookie = false;
                if (Request["rememberpassword"] == "on")
                {
                    PersistentCookie = true;
                }
                FormAuthService.SignIn(users.UserName, PersistentCookie, new string[] { "user" });
                return Json(new { Result = "OK", Message = ReturnUrl });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormAuthService.SignOut();
            var ReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer == null ? "/Home/Index" :
                (System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri == System.Web.HttpContext.Current.Request.Url.AbsoluteUri ? "/Home/Index" : System.Web.HttpContext.Current.Request.UrlReferrer.AbsoluteUri);
            return Redirect(ReturnUrl);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterAjax()
        {
            try
            {
                string verifycode = Request["verifycode"];
                var vc = System.Web.HttpContext.Current.Session["code"] == null ? "" : System.Web.HttpContext.Current.Session["code"].ToString();
                if (verifycode.ToUpper() == vc.ToUpper())
                {
                    string mid = Guid.NewGuid().ToString();
                    int userRoleID = _repository._UserRoleRepositories.GetAllUserRole().Where(n => n.Name == "普通用户").First().Id;
                    Users user = new Users { UserName = Request["username"], Password = Request["pwd"], NickName = Request["username"], Email = Request["email"], Status = 0, Mid = mid, UserRoleID = userRoleID, RegTime = DateTime.Now };
                    if (_repository._UserRepositories.UserNameNotExist(user.UserName) == false)
                    {
                        return Json(new { Result = "Error", Message = "用户名已存在" });
                    }
                    if (_repository._UserRepositories.EmailNotExist(user.Email) == false)
                    {
                        return Json(new { Result = "Error", Message = "电子邮件地址已存在" });
                    }
                    UserValidation userValidation = new UserValidation();
                    ValidationResult validationResult = userValidation.Validate(user);
                    string Msg = "";
                    if (!validationResult.IsValid)
                    {
                        foreach (var failure in validationResult.Errors)
                        {
                            Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                        }
                        return Json(new { Result = "Error", Message = Msg });
                    }
                    user.Password = Encrypt.MD5Encrypt(Request["pwd"]);
                    _repository._UserRepositories.AddUser(user);
                    StringBuilder sb = new StringBuilder();
                    string local = Request.Url.Authority;
                    sb.Append(string.Format(@"<span>恭喜您，已成功注册为NIIT博客的会员。</span><br/>
                        <span>以下是您的部分注册信息：</span><br/>
                        <br/>
                        <span>用户名：</span><span>{0}</span><br/>
                        <span>密码：</span><span>{1}</span><br/>
                        <span>E-mail地址：</span><span>{2}</span><br/>
                        <br/>
                        <span>为了您个人资料的安全与权利，请务必妥善保存好您的会员Email与密码资料，以防您的账号被他人恶意盗取。
                        </span><br/>
                        <br/>
                        <span>请点击此处激活：<a href='http://{3}/Account/Activation?n={4}&mid={5}'>{6}/Account/Activation</a></span><br/>
                        <br/>
                        <br/>
                        <br/>
                        <span>如有任何问题，请与我们联系：</span><br/>", user.UserName, Request["pwd"], user.Email, local, user.UserName, user.Mid, local)
                                                                );
                    sb.Append("<span><a href=\"mailto:niit@niitblog.com\">niit@niitblog.com</a></span><br/><br/>");

                    sb.Append(@"<span>再次感谢您对NIIT博客的支持与关注，祝你每一天都有收获！</span><br/>
                        <br/>
                        <span>============================================================================================</span><br/>
                        <br/>
                        <span>附：以上邮件由系统自动生成，请不要直接回复。</span>"
                        );

                    Task.Factory.StartNew(() =>
                    {
                        Common.SendEmail.SendMails(user.Email, "NIIT博客注册帐号激活", sb.ToString(),
                            () =>
                            {
                                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Server.MapPath("~/data_log/"));
                                if (!dir.Exists)
                                    dir.Create();
                                string _savefile = "/data_log/SendEmailError_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                                string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "/data_log/SendEmailError_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, true, System.Text.Encoding.UTF8,1024);

                                sw.WriteLine(sb);
                                sw.Close();
                                sw.Dispose();
                            });
                    });

                    return Json(new { Result = "OK" });
                }
                else
                {
                    return Json(new { Result = "Error", Message = "验证码错误" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EmailExistOrNot(string email)
        {
            return Json(_repository._UserRepositories.EmailNotExist(email));
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserNameExistOrNot(string username)
        {
            return Json(_repository._UserRepositories.UserNameNotExist(username));
        }

        [AllowAnonymous]
        public ActionResult Activation()
        {
            try
            {
                string username = Request.QueryString["n"];
                string mid = Request.QueryString["mid"];
                var n = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
                if (n.Mid == mid)
                {
                    n.Status = 1;
                    n.Mid = Guid.NewGuid().ToString();
                    _repository._UserRepositories.UpdateUserStatusAndMid(n);
                    FormAuthService.SignIn(n.UserName, false, new string[] { "user" });
                    return View();
                }
                else
                {
                    return Redirect("/404.htm");
                }
            }
            catch
            {
                return Redirect("/404.htm");
            }

        }

        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult HandleForgetPassword()
        {
            try
            {
                string verifycode = Request["verifycode"];
                string email = Request["email"];
                var vc = System.Web.HttpContext.Current.Session["code"] == null ? "" : System.Web.HttpContext.Current.Session["code"].ToString();
                if (verifycode.ToUpper() == vc.ToUpper())
                {
                    Users user = new Users { Email = email };
                    UserValidation userValidation = new UserValidation();
                    ValidationResult validationResult = userValidation.Validate(user);
                    string Msg = "";
                    if (!validationResult.IsValid)
                    {
                        foreach (var failure in validationResult.Errors)
                        {
                            Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                        }
                        return Json(new { Result = "Error", Message = Msg });
                    }
                    var u = _repository._UserRepositories.GetUserByEmail(user);
                    if (u != null)
                    {
                        DateTime datetime = DateTime.Now;
                        CookiesHelp.SetCookie("mid", u.Mid, 24);
                        StringBuilder sb = new StringBuilder();
                        string local = Request.Url.Authority;
                        sb.Append(string.Format(@"<div>此邮件由系统自动发出，请勿直接回复</div><hr/><br/><div><span>亲爱的{0}:</span>
                       <div>您已成功发送密码重置请求，请点击此处<a href='http://{1}/Account/GetPassword?u={2}'>{3}/Account/ForgetPassword</a>重置您的帐号密码。</div>
                       <br/>
                       <hr/>
                       <div>此连接24小时内有效，请及时重置您的密码。</div>
                       <div>请妥善保存此邮件，以防您的帐号被他人恶意盗取。</div>
                       <div>NIIT博客之学好IT不挨踢  {4}</div>", u.UserName, local, u.UserName, local, datetime.ToString()));
                        Task.Factory.StartNew(() =>
                        {
                            Common.SendEmail.SendMails(email, "NIIT博客之学好IT不挨踢注册帐号密码重置", sb.ToString(), () =>
                            {
                                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Server.MapPath("~/data_log/"));
                                if (!dir.Exists)
                                    dir.Create();
                                string _savefile = "~/data_log/HandleForgetPasswordError_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                                System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Web.HttpContext.Current.Server.MapPath(_savefile), true, System.Text.Encoding.UTF8);
                                sw.WriteLine(sb);
                                sw.Close();
                                sw.Dispose();
                            });
                        });
                        return Json(new { Result = "OK" });
                    }
                    else
                    {
                        return Json(new { Result = "Error", Message = "邮箱不存在" }); ;
                    }
                }
                else
                {
                    return Json(new { Result = "Error", Message = "验证码错误" }); ;
                }

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [ActionName("Profile")]
        public ActionResult Profile1(string user)
        {
            string avatarFlashParam;
            string EncodeLocalhost;
            string Localhost;
            string uid;

            int port = Request.Url.Port;
            string ApplicationPath = Request.ApplicationPath != "/" ? Request.ApplicationPath : string.Empty;
            uid = User.Identity.Name;
            Localhost = string.Format("{0}://{1}{2}{3}",
                                 Request.Url.Scheme,
                                 Request.Url.Host,
                                 (port == 80 || port == 0) ? "" : ":" + port,
                                 ApplicationPath);
            EncodeLocalhost = HttpUtility.UrlEncode(Localhost);
            avatarFlashParam = string.Format("{0}/Content/Avatar/common/camera.swf?nt=1&inajax=1&appid=1&input={1}&ucapi={2}/AjaxAvatar.ashx", Localhost, uid, EncodeLocalhost);

            ViewBag.avatarFlashParam = avatarFlashParam;
            ViewBag.Localhost = Localhost;
            ViewBag.uid = uid;

            Users s = _repository._UserRepositories.GetUserByUserName(new Users { UserName = uid });
            if (s == null)
                return Redirect("/404.html");
            ViewBag.ID = s.UID;

            return View(s);
        }

        [HttpPost]
        public ActionResult EditorProfile(string UID)
        {
            try
            {
                string gender = Request["Gender"] == "===请选择===" ? "" : Request["Gender"];
                Users user = new Users { UID = Convert.ToInt32(UID), NickName = Request["NickName"], Gender = gender, SelfIntro = HttpUtility.HtmlEncode(Request["SelfIntro"]), Description = HttpUtility.HtmlEncode(Request["Description"]) };
                UserValidation userValidation = new UserValidation();
                ValidationResult validationResult = userValidation.Validate(user);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                _repository._UserRepositories.UpdateProfile(user);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ChangePassWord(string UID)
        {
            try
            {
                Users user = new Users { UID = Convert.ToInt32(UID), Password = Request["newpassword"] };
                UserValidation userValidation = new UserValidation();
                ValidationResult validationResult = userValidation.Validate(user);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                user.Password = Common.Encrypt.MD5Encrypt(Request["newpassword"]);
                var s = _repository._UserRepositories.GetUserByUID(user);
                if (s.Password == Common.Encrypt.MD5Encrypt(Request["oldpassword"]))
                {
                    _repository._UserRepositories.ChangePassWord(user);
                    return Json(new { Result = "OK" });
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "原密码错误" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [AllowAnonymous]
        public ActionResult GetPassword()
        {
            string mid = CookiesHelp.GetCookieValue("mid");
            if (mid != null)
            {
                string username = Request.QueryString["u"];
                var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
                if (user != null)
                {
                    if (user.Mid == mid)
                    {
                        ViewBag.UID = user.UID;
                        return View();
                    }
                    else
                    {
                        return Redirect("/404.htm");
                    }
                }
                else
                {
                    return Redirect("/404.htm");
                }
            }
            else
            {
                return Redirect("/404.htm");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult GetPassWord(string UID)
        {
            try
            {
                string verifycode = Request["verifycode"];
                var vc = System.Web.HttpContext.Current.Session["code"] == null ? "" : System.Web.HttpContext.Current.Session["code"].ToString();
                if (verifycode.ToUpper() == vc.ToUpper())
                {
                    Users user = new Users { UID = Convert.ToInt32(UID), Password = Request["newpassword"] };
                    UserValidation userValidation = new UserValidation();
                    ValidationResult validationResult = userValidation.Validate(user);
                    string Msg = "";
                    if (!validationResult.IsValid)
                    {
                        foreach (var failure in validationResult.Errors)
                        {
                            Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                        }
                        return Json(new { Result = "Error", Message = Msg });
                    }
                    user.Password = Common.Encrypt.MD5Encrypt(Request["newpassword"]);
                    _repository._UserRepositories.ChangePassWord(user);
                    CookiesHelp.DeleteCookiesObj("mid");
                    return Json(new { Result = "OK" });
                }
                else
                {
                    return Json(new { Result = "Error", Message = "验证码错误" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

    }
}