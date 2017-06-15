using FluentValidation.Results;
using NiitBlog.Models;
using NiitBlog.Validator;
using System;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace NiitBlog.Controllers
{
    public class UserController : Controller
    {

        private IRepositoryContainer _repository;

        public UserController()
        {
            this._repository = new RepositoryContainer();
        }
  
        public ActionResult Index(string UserName)
        {
            var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = UserName });
            if (user != null)
            {
                ViewBag.Title = UserName;
                ViewBag.TID = Request.QueryString["tid"];
                ViewBag.Year = Request.QueryString["y"];
                ViewBag.Month = Request.QueryString["m"];
                ViewBag.CID = Request.QueryString["cid"];
                int uid = user.UID;
                ViewBag.UID = uid;
                if (Request.QueryString["tid"] == null && (Request.QueryString["y"] == null || Request.QueryString["m"] == null) && Request.QueryString["cid"]==null)
                {
                    ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUID(uid);
                }
                else if (Request.QueryString["tid"] == null && Request.QueryString["cid"] == null)
                {
                    int y = Convert.ToInt32(Request.QueryString["y"]);
                    int m = Convert.ToInt32(Request.QueryString["m"]);
                    ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUIDYM(uid, y, m);
                }
                else if (Request.QueryString["cid"] == null)
                {
                    int tid = Convert.ToInt32(Request.QueryString["tid"]);
                    ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUIDTID(uid, tid);
                }
                else 
                {
                    int cid = Convert.ToInt32(Request.QueryString["cid"]);
                    ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUIDCID(uid, cid);
                }
                try
                {
                    user.HeadPic = user.HeadPic.Replace("medium", "large");
                }
                catch
                {
                    user.HeadPic="http://"+Request.Url.Authority+"/Content/Avatar/upload/avatars/noavatar_medium.gif";
                }
                return View(user);
            }
            else
            {
                return Content("您所要访问的用户不存在！");
            }
        }

        [Authorize]
        public ActionResult Manage(string UserName)
        {
            if (UserName != HttpContext.User.Identity.Name)
            {
                return null;
            }
            else
            {
                var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = UserName });
                ViewBag.Title = UserName;
                ViewBag.UID = user.UID;
                try
                {
                    user.HeadPic = user.HeadPic.Replace("medium", "large");
                }
                catch
                {
                    user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
                }
                ViewBag.RecordCount = _repository._ArticleRepositories.GetArticlesCountByUID(user.UID);
                ViewBag.CategoryRecordCount = _repository._CategoriesRepositories.GetCategoryCountByUID(user.UID);
                ViewBag.TagRecordCount = _repository._TagsRepositories.UserTagsCount(user.UID);
                ViewBag.AlbumRecordCount = _repository._AlbumRepositories.GetAlbumsByUid(user.UID).Count();
                ViewBag.PhotoRecordCount = _repository._PhotoRepositories.GetPhotoCountByUID(user.UID);
                ViewData["album"] = new SelectList(_repository._AlbumRepositories.GetAlbumsByUid(user.UID).Select(n => new { AlbumID = n.AlbumID, AlbumName = n.AlbumName }), "AlbumID", "AlbumName");
                return View(user);
            }
        }

        [HttpPost]
        public ActionResult Tags(int uid)
        {
            int tagcount = 10;
            var tag = _repository._TagsRepositories.GetUserTag(uid).Take(tagcount); //热门标签
            tagcount = tag.Count();
            var json = Json(new { results = tag, count = tagcount });
            return json;
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetPageList(int uid, int ListItemNum, int PageIndex, string sorting)
        {
            try
            {
                var articles = _repository._ArticleRepositories.GetArticles(uid, PageIndex, ListItemNum, sorting);
                var res = articles.Select(n => new
                {
                    AID = n.AID,
                    Title = n.Title,
                    ViewNum=n.ViewNum,
                    CommentNum = n.CommentNum,
                    CreateTime = n.CreateTime,
                    UID = n.UID
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteArticle(int aid,int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        _repository._ArticleRepositories.DeleteArticle(aid, uid);
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else 
            {
                return null;
            }

        }


        [Authorize]
        [HttpPost]
        public ActionResult GetCategoryPageList(int uid, int ListItemNum, int PageIndex)
        {
            try
            {
                var category = _repository._CategoriesRepositories.GetCategories(uid, PageIndex, ListItemNum);
                var res = category.Select(n => new
                {
                    CID = n.CID,
                    CName = n.CName,
                    UID = n.UID
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateCategory(int cid, string cname, int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        Categories category = new Categories { CID = cid, CName = cname, UID = uid };
                        CategoryValidation albumValidation = new CategoryValidation();
                        ValidationResult validationResult = albumValidation.Validate(category);
                        string Msg = "";
                        if (!validationResult.IsValid)
                        {
                            foreach (var failure in validationResult.Errors)
                            {
                                Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                            }
                            return Json(new { Result = "Error", Message = Msg });
                        }
                        _repository._CategoriesRepositories.UpdateCategory(category);
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult GetArticlesCountByUIDandCID(int uid, int cid)
        {
            return Json(_repository._ArticleRepositories.GetArticlesCountByUIDandCID(uid, cid));
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteCategory(int cid, int uid)
        {
              var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
              if (u != null)
              {
                  if (u.UserName != HttpContext.User.Identity.Name)
                  {
                      return null;
                  }
                  else
                  {
                      try
                      {
                          _repository._CategoriesRepositories.DeleteCategory(uid, cid);
                          return Json(new { Result = "OK" });
                      }
                      catch (Exception ex)
                      {
                          return Json(new { Result = "Error", Message = ex.Message });
                      }
                  }
              }
              else 
              { 
                  return null; 
              }
        }

        [Authorize]
        [HttpPost]
        public ActionResult GetTagPageList(int uid, int ListItemNum, int PageIndex)
        {
            try
            {
                var tag = _repository._TagsRepositories.GetTags(uid, PageIndex, ListItemNum);
                var res = tag.Select(n => new
                {
                    TID = n.TID,
                    TName = n.TName,
                    UID = n.UID
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateTag(int tid, string tname, int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        NiitBlog.Models.Tags tag = new NiitBlog.Models.Tags { TID = tid, TName = tname, UID = uid };
                        TagValidation tagValidation = new TagValidation();
                        ValidationResult validationResult = tagValidation.Validate(tag);
                        string Msg = "";
                        if (!validationResult.IsValid)
                        {
                            foreach (var failure in validationResult.Errors)
                            {
                                Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                            }
                            return Json(new { Result = "Error", Message = Msg });
                        }
                        _repository._TagsRepositories.UpdateTag(tag);
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteTag(int tid, int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        _repository._TagsRepositories.DeleteTag(uid, tid);
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult GetAlbumPageList(int uid, int ListItemNum, int PageIndex, string sorting)
        {
            try
            {
                var album = _repository._AlbumRepositories.GetAlbums(uid, PageIndex, ListItemNum, sorting);
                var res = album.Select(n => new
                {
                    AlbumID = n.AlbumID,
                    AlbumName = n.AlbumName,
                    UID = n.UID,
                    Description=n.Description,
                    PhotoNum=n.PhotoNum
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteAlbum(int albumid, int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        var album = _repository._AlbumRepositories.GetAlbum(albumid);
                        _repository._AlbumRepositories.DeleteAlbum(uid, albumid);
                        Common.DirFile.DeleteDir(string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, album.AlbumID));
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult GetPhotoPageList(int uid, int ListItemNum, int PageIndex, string sorting)
        {
            try
            {
                var tag = _repository._PhotoRepositories.GetPhotos(uid, PageIndex, ListItemNum,sorting);
                var res = tag.Select(n => new
                {
                    PID = n.PID,
                    PhotoName = n.PhotoName,
                    UID = n.UID,
                    Path = n.Path,
                    Thumbnail = n.Thumbnail,
                    Description = n.Description,
                    CreateTime = n.CreateTime,
                    AlbumID = n.AlbumID,
                    AlbumName = n.Albums.AlbumName
                });
                return Json(new { Result = "OK", Message = res });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdatePhoto(int uid,FormCollection frm)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        int pid =Convert.ToInt32(frm["pid"]);
                        string name = Request["photoname"];
                        string description = Request["photodescription"];
                        int albumid = Convert.ToInt32(Request["album"]);
                        var photo = _repository._PhotoRepositories.Getphoto(pid);
                        photo.PhotoName = name;
                        photo.Description = description;
                        photo.AlbumID = albumid;
                        PhotoValidation photoValidation = new PhotoValidation();
                        ValidationResult validationResult = photoValidation.Validate(photo);
                        string Msg = "";
                        if (!validationResult.IsValid)
                        {
                            foreach (var failure in validationResult.Errors)
                            {
                                Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                            }
                            return Json(new { Result = "Error", Message = Msg });
                        }
                        _repository._PhotoRepositories.UpdatePhoto(photo);
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeletePhoto(int pid, int uid)
        {
            var u = _repository._UserRepositories.GetUserByUID(new Users { UID = uid });
            if (u != null)
            {
                if (u.UserName != HttpContext.User.Identity.Name)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        var photo = _repository._PhotoRepositories.Getphoto(pid);
                        _repository._PhotoRepositories.DeletePhoto(uid, pid);
                        Common.DirFile.DeleteFile(string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, photo.AlbumID) + photo.Path.Substring(photo.Path.LastIndexOf('/') + 1));
                        Common.DirFile.DeleteFile(string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, photo.AlbumID) + photo.Thumbnail.Substring(photo.Thumbnail.LastIndexOf('/') + 1));
                        Common.DirFile.DeleteFile(string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, photo.AlbumID) + photo.Thumbnailw.Substring(photo.Thumbnailw.LastIndexOf('/') + 1));
                        return Json(new { Result = "OK" });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Result = "Error", Message = ex.Message });
                    }
                }
            }
            else
            {
                return null;
            }
        }

    }
}
