using DbHelper;
using FluentValidation.Results;
using NiitBlog.Models;
using NiitBlog.Validator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NiitBlog.Controllers
{
    public class ArticleController : Controller
    {
        
        private IRepositoryContainer _repository;

        public ArticleController()
        {
            this._repository = new RepositoryContainer();
        }

        public ActionResult Detail(int id, string username)
        {
                int aid = id;
                ViewBag.aid = aid;
                int uid = _repository._ArticleRepositories.GetUIDbyAID(aid);
                if (uid != -1)
                {
                    var user = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
                    if (user.UserName != username)
                        return Content("您所要访问的用户不存在！");

                    

                    DBHelper.RunNoQuery("inser into visit values(@vTime,@ip,@http)", CommandType.Text);
                 
                   
                  




                    ViewBag.Title=_repository._ArticleRepositories.GetArticleByAID(aid).Title;
                    //判断独立ip 才plusone

                    _repository._ArticleRepositories.ViewNumPlusOne(aid);
                    ViewBag.UID = uid;
                    ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUID(uid);
                    try
                    {
                        user.HeadPic = user.HeadPic.Replace("medium", "large");
                    }
                    catch
                    {
                        user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
                    }
                    return View(user);
                }
                else
                {
                    return null;
                }
        }


        [HttpPost]
        public ActionResult Detailajax(int aid)
        {
            var article = _repository._ArticleRepositories.GetArticleByAID(aid);
            Dictionary<int,string> art = _repository._ArticleRepositories.GetPreAndNextArticleIdAndTitle(article);
            var ret = new { AID = article.AID, Content = article.Content, CreateTime = article.CreateTime.ToShortDateString(), Title = article.Title, CommentNum = article.CommentNum, ViewNum = article.ViewNum, Category = new { CID = article.CID, CName = article.Categories.CName }, pre = new { id = art.ElementAt(0).Key, title = art.ElementAt(0).Value }, next = new { id = art.ElementAt(1).Key, title = art.ElementAt(1).Value } };
            return Json(ret);
        }


        [Authorize]
        public ActionResult Post(string username)
        {
            if (username != HttpContext.User.Identity.Name)
            {
                return null;
            }
            else
            {
                var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
                if (user != null)
                {
                    ViewData["Category"] = new SelectList(_repository._CategoriesRepositories.GetCategoriesByUID(user.UID), "CID", "CName");
                    try
                    {
                        user.HeadPic = user.HeadPic.Replace("medium", "large");
                    }
                    catch
                    {
                        user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
                    }
                    return View(user);
                }
                else
                    return null;
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Post(int uid,FormCollection frm)
        {
            try
            {
                string content = frm["content"];
                if (string.IsNullOrEmpty(content))
                    return Json(new { Result = "Error", Message = "请输入文章内容" });
                string tags = frm["tags"];
                string title = frm["title"];
                int cid = Convert.ToInt32(frm["Category"]);
                string summery = frm["Summery"];
                Articles art = new Articles { Title = title, Content = content,Summery=summery, CreateTime = DateTime.Now, ViewNum = 0, CommentNum = 0, CID = cid, UID = uid };
                if (!string.IsNullOrEmpty(tags))
                {
                    var tag = tags.Split(',');
                    if (tag.Where(n => n.Length >= 30).Count() > 0)
                    {
                        return Json(new { Result = "Error", Message = "标签名最多为30个字符" });
                    }
                }
                ArticleValidation articleValidation = new ArticleValidation();
                ValidationResult validationResult = articleValidation.Validate(art);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                _repository._ArticleRepositories.AddArticle(art, tags);
                return Json(new { Result = "OK", Message = art.AID });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(int uid)
        {
            try
            {
                Categories cat = new Categories { CName = Request["cname"], UID = uid };
                CategoryValidation albumValidation = new CategoryValidation();
                ValidationResult validationResult = albumValidation.Validate(cat);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                if (_repository._CategoriesRepositories.CategoryExist(cat.CName, uid) == false)
                {
                    return Json(new { Result = "Error", Message = "类别已存在" });
                }
                int cid= _repository._CategoriesRepositories.AddCategories(cat);
                return Json(new { Result = "OK", Message=cid });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CategoryExist(string cname,int uid)
        {
            return Json(_repository._CategoriesRepositories.CategoryExist(cname, uid));
        }

        [Authorize]
        public ActionResult Postedit(string username,int id)
        {
            if (username != HttpContext.User.Identity.Name)
            {
                return  null;
            }
            else
            {
                var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
                if (user != null)
                {
                    try
                    {
                        user.HeadPic = user.HeadPic.Replace("medium", "large");
                    }
                    catch
                    {
                        user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
                    }
                    var article = _repository._ArticleRepositories.GetArticleByAID(id);
                    if (article == null)
                        return null;
                    ViewBag.articleAid = article.AID;
                    ViewBag.articleTitle = article.Title;
                    ViewBag.articleContent = article.Content;
                    ViewBag.articleSummery = article.Summery;
                    var tag = _repository._TagsRepositories.ArticleTags(id);
                    StringBuilder sb = new StringBuilder();
                    int len = tag.Count;
                    for (int i = 0; i < len; i++)
                    {
                        if (i == len - 1)
                        {
                            sb.Append(tag[i]);
                        }
                        else
                        {
                            sb.Append(tag[i] + ",");
                        }
                    }
                    ViewBag.articleTags = sb.ToString();
                    ViewData["Category"] = new SelectList(_repository._CategoriesRepositories.GetCategoriesByUID(user.UID), "CID", "CName", article.CID);
                    var s=Guid.NewGuid().ToString();
                    Common.CookiesHelp.SetCookie("token", s);
                    Common.CookiesHelp.SetCookie("_token", s);
                    return View(user);
                }
                else
                    return null;
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Postedit(int aid, int uid, FormCollection frm)
        {
            try
            {
                string content = frm["content"];
                if (string.IsNullOrEmpty(content))
                    return Json(new { Result = "Error", Message = "请输入文章内容" });
                string tags = frm["tags"];
                string title = frm["title"];
                int cid = Convert.ToInt32(frm["Category"]);
                string summery = frm["Summery"];
                Articles art = new Articles { AID = aid, Title = title, Content = content, Summery = summery, CID = cid, UID = uid };
                if (!string.IsNullOrEmpty(tags))
                {
                    var tag = tags.Split(',');
                    if (tag.Where(n => n.Length >= 30).Count() > 0)
                    {
                        return Json(new { Result = "Error", Message = "标签名最多为30个字符" });
                    }
                }
                ArticleValidation articleValidation = new ArticleValidation();
                ValidationResult validationResult = articleValidation.Validate(art);
                string Msg = "";
                if (!validationResult.IsValid)
                {
                    foreach (var failure in validationResult.Errors)
                    {
                        Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                    }
                    return Json(new { Result = "Error", Message = Msg });
                }
                _repository._ArticleRepositories.UpdateArticle(art, tags);
                return Json(new { Result = "OK", Message = aid });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult DeleteArticleTag(string tname,int aid,int uid)
        {
            try
            {
                if (Common.CookiesHelp.GetCookieValue("token") == Common.CookiesHelp.GetCookieValue("_token"))
                {
                    _repository._TagsRepositories.DeleteArticleTag(tname, aid,uid);
                    return Json(new { Result = "OK" });
                }
                else
                {
                    Common.RecordHack.SaveVisitLog(2, 0);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetPageList(int uid,int ListItemNum, int PageIndex, string sorting)
        {
            try
            {
                var articles = _repository._ArticleRepositories.GetArticles(uid, PageIndex, ListItemNum, sorting);
                var res= articles.Select(n => new {
                    AID=n.AID,
                    Title=n.Title,
                    Summery =n.Summery,
                    CreateTime=n.CreateTime,
                    UID=n.UID,
                    Tag=_repository._TagsRepositories.ArticleTags(n.AID)
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetPageListByTid(int uid, int ListItemNum, int PageIndex, string sorting,int tid)
        {
            try
            {
                var articles = _repository._ArticleRepositories.GetArticles(uid, PageIndex, ListItemNum, sorting,tid);
                var res = articles.Select(n => new
                {
                    AID = n.AID,
                    Title = n.Title,
                    Summery = n.Summery,
                    CreateTime = n.CreateTime,
                    UID = n.UID,
                    Tag = _repository._TagsRepositories.ArticleTags(n.AID)
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult GetPageListByCid(int uid, int ListItemNum, int PageIndex, string sorting, int cid)
        {
            try
            {
                var articles = _repository._ArticleRepositories.GetArticles1(uid, PageIndex, ListItemNum, sorting, cid);
                var res = articles.Select(n => new
                {
                    AID = n.AID,
                    Title = n.Title,
                    Summery = n.Summery,
                    CreateTime = n.CreateTime,
                    UID = n.UID,
                    Tag = _repository._TagsRepositories.ArticleTags(n.AID)
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetPageListByYearMonth(int uid, int ListItemNum, int PageIndex, string sorting, int y,int m)
        {
            try
            {
                var articles = _repository._ArticleRepositories.GetArticles(uid, PageIndex, ListItemNum, sorting,y,m);
                var res = articles.Select(n => new
                {
                    AID = n.AID,
                    Title = n.Title,
                    Summery = n.Summery,
                    CreateTime = n.CreateTime,
                    UID = n.UID,
                    Tag = _repository._TagsRepositories.ArticleTags(n.AID)
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ReadTop(int uid)
        {
            var s = _repository._ArticleRepositories.ReadTop(uid, 5);
            var ret = s.Select(n => new { AID = n.AID, Title = n.Title, ViewNum = n.ViewNum });
            return Json(ret);
        }


        [HttpPost]
        public ActionResult Archive(int uid)
        {
            return Json(_repository._ArticleRepositories.GetArticleArchives(uid));
        }

        [HttpPost]
        public ActionResult Category(int uid)
        {
            return Json(_repository._CategoriesRepositories.GetArticleCategories(uid));
        }

        public ActionResult GetComments(int aid, string commentusername)
        {
            int uid;
            var u = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername });
            if (u != null)
                uid = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername }).UID;
            else
                uid = 0;
            List<CommentsViewModel> lcom=_repository._ArticleRepositories.GetCommentsByAid(aid,uid);
            foreach (CommentsViewModel com in lcom)
            {
                com.Date = com.TempDate.ToString();
                com.UserAvatar = com.UserAvatar + "?i=" + Guid.NewGuid().ToString();
            }
            return Json(lcom,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        public ActionResult PostComment(int aid, string commentusername, FormCollection collection)
        {
            if (commentusername != HttpContext.User.Identity.Name)
            {
                return null;
            }
            else
            {
                var u = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername });
                if (u != null)
                {
                    int uid = u.UID;
                    Comments com = new Comments { AID = aid, UID = uid, CreateTime = DateTime.Now, Content = HttpUtility.HtmlEncode(collection["comment"]) };
                    var pid = Convert.ToInt32(collection["parentId"]);
                    if (pid == 0)
                        com.ParentId = null;
                    else
                        com.ParentId = pid;
                    var commentview = _repository._CommentsRepositories.AddComment(com);
                    _repository._ArticleRepositories.CommentNumPlusOne(aid);
                    commentview.UserAvatar = commentview.UserAvatar + "?i=" + Guid.NewGuid().ToString();
                    return Json(commentview);
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteComment(int aid, string commentusername)
        {
            if (commentusername != HttpContext.User.Identity.Name)
            {
                return null;
            }
            else
            {
                var u = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername });
                if (u != null)
                {
                    int id = Convert.ToInt32(Request["commentId"]);
                    int rowcount = _repository._CommentsRepositories.DeleteComment(id, u.UID);
                    _repository._ArticleRepositories.CommentNumSubtract(aid, rowcount);
                    return Json(id);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
