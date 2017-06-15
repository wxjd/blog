using FluentValidation.Results;
using NiitBlog.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using NiitBlog.Validator;
using System.Text.RegularExpressions;




namespace NiitBlog.Controllers
{
    public class AlbumController : Controller
    {
        private IRepositoryContainer _repository;

        public AlbumController()
        {
            this._repository = new RepositoryContainer();
        }

        [AllowAnonymous]
        public ActionResult Index(string username)
        {
            var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
            if (user == null)
                return null;
            try
            {
                user.HeadPic = user.HeadPic.Replace("medium", "large");
            }
            catch
            {
                user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
            }
            List<AlbumsViewModel> albumcolumn1 = new List<AlbumsViewModel>();
            List<AlbumsViewModel> albumcolumn2 = new List<AlbumsViewModel>();
            List<AlbumsViewModel> albumcolumn3 = new List<AlbumsViewModel>();
            var albums=_repository._AlbumRepositories.GetAlbumsByUid(user.UID);
            int i = 0;
            foreach (Albums a in albums)
            {
                int k=i%3 ;
                if (k== 0)
                    albumcolumn1.Add(new AlbumsViewModel { AlbumID = a.AlbumID, CoverPath = a.CoverPath, CreateTime = a.CreateTime, AlbumName = a.AlbumName, Description = a.Description, PhotoNum = a.PhotoNum, column = k, UID = a.UID });
                else if (k == 1)
                {
                    albumcolumn2.Add(new AlbumsViewModel { AlbumID = a.AlbumID, CoverPath = a.CoverPath, CreateTime = a.CreateTime, AlbumName = a.AlbumName, Description = a.Description, PhotoNum = a.PhotoNum, column = k, UID = a.UID });
                }
                else if (k== 2)
                {
                    albumcolumn3.Add(new AlbumsViewModel { AlbumID = a.AlbumID, CoverPath = a.CoverPath, CreateTime = a.CreateTime, AlbumName = a.AlbumName, Description = a.Description, PhotoNum = a.PhotoNum, column = k, UID = a.UID });
                }
                i++;
            }
            ViewData["albums1"] = albumcolumn1;
            ViewData["albums2"] = albumcolumn2;
            ViewData["albums3"] = albumcolumn3;
            ViewData["album"] =new SelectList( _repository._AlbumRepositories.GetAlbumsByUid(user.UID).Select(n => new {AlbumID= n.AlbumID,AlbumName= n.AlbumName }),"AlbumID","AlbumName");
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AlbumUpdate(int uid)
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
                        int albumid =Convert.ToInt32(Request["albumid"]);
                        string albumname=  Request["albumname"];
                        string description = Request["description"];
                        HttpPostedFileBase file = Request.Files["imgfile"];
                        if (file != null)
                        {
                            if (file.InputStream == null || file.InputStream.Length > 1024000)
                            {
                                return Json(new { Result = "ERROR", Message = "上传单张图片最大为1M" });
                            }
                            else
                            {
                                string FileType = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                                if (FileType == "gif" || FileType == "GIF" || FileType == "jpg" || FileType == "JPG" || FileType == "png" || FileType == "PNG")
                                {
                                    string savePath = string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, albumid);
                                    string saveUrl = "http://" + Request.Url.Authority + savePath;
                                    string dirPath = Server.MapPath(savePath);
                                    string fileName = file.FileName;
                                    string fileExt = Path.GetExtension(fileName).ToLower();
                                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                                    string filePath = dirPath + newFileName;
                                    file.SaveAs(filePath);
                                    string fileUrl = saveUrl + newFileName;
                                    Albums album = _repository._AlbumRepositories.GetAlbum(albumid);
                                    string coverpath = album.CoverPath;
                                    album.CoverPath = fileUrl;
                                    album.AlbumName = albumname;
                                    album.Description = description;
                                    AlbumValidation albumValidation = new AlbumValidation();
                                    ValidationResult validationResult = albumValidation.Validate(album);
                                    string Msg = "";
                                    if (!validationResult.IsValid)
                                    {
                                        foreach (var failure in validationResult.Errors)
                                        {
                                            Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                                        }
                                        return Json(new { Result = "Error", Message = Msg });
                                    }
                                    Common.DirFile.DeleteFile(savePath + coverpath.Substring(coverpath.LastIndexOf('/') + 1));
                                    _repository._AlbumRepositories.UpdateAlbum(album);
                                    return Json(new { Result = "OK", Message = new { AlbumID = album.AlbumID, CoverPath = album.CoverPath, AlbumName = album.AlbumName, Description = album.Description} });
                                }
                                else
                                {
                                    return Json(new { Result = "ERROR", Message = "上传图片格式错误" });
                                }
                            }
                        }
                        else 
                        {

                            Albums album = _repository._AlbumRepositories.GetAlbum(albumid);
                            album.AlbumName = albumname;
                            album.Description = description;
                            AlbumValidation albumValidation = new AlbumValidation();
                            ValidationResult validationResult = albumValidation.Validate(album);
                            string Msg = "";
                            if (!validationResult.IsValid)
                            {
                                foreach (var failure in validationResult.Errors)
                                {
                                    Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                                }
                                return Json(new { Result = "Error", Message = Msg });
                            }
                            _repository._AlbumRepositories.UpdateAlbum(album);
                            return Json(new { Result = "OK", Message = new { AlbumID = album.AlbumID, CoverPath = album.CoverPath, AlbumName = album.AlbumName, Description = album.Description } });
                        }
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
        public ActionResult AddPhoto(int uid)
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
                        string name = Request["photoname"];
                        string description = Request["photodescription"];
                        int albumid =Convert.ToInt32(Request["album"]);
                        HttpPostedFileBase file = Request.Files["path"];
                        if (file != null)
                        {
                            if (file.InputStream == null || file.InputStream.Length > 2048000)
                            {
                                return Json(new { Result = "ERROR", Message = "上传单张图片最大为2M" });
                            }
                            else
                            {
                                string FileType = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                                if (FileType == "gif" || FileType == "GIF" || FileType == "jpg" || FileType == "JPG" || FileType == "png" || FileType == "PNG")
                                {
                                    Photos photo = new Photos { PhotoName = name, Description = description, Path = " ", Thumbnail = " ",Thumbnailw=" ", CreateTime = DateTime.Now,AlbumID = albumid, UID = uid };
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
                                    var a = _repository._PhotoRepositories.AddPhoto(photo);
                                    string savePath = string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, a.AlbumID);
                                    string saveUrl = "http://" + Request.Url.Authority + savePath;
                                    string dirPath = Server.MapPath(savePath);
                                    if (!Directory.Exists(dirPath))
                                        Directory.CreateDirectory(dirPath);
                                    string fileName = file.FileName;
                                    string fileExt = Path.GetExtension(fileName).ToLower();
                                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                                    string newThumbnailFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo)+"_thumbnail" + fileExt;
                                    string newThumbnailwFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "_thumbnailw" + fileExt;
                                    string filePath = dirPath + newFileName;
                                    file.SaveAs(filePath);
                                    string fileUrl = saveUrl + newFileName;
                                    a.Path = fileUrl;
                                    Common.ImageHelp.LocalImage2Thumbs(filePath, dirPath + newThumbnailFileName, 300, 300, "CUT");
                                    a.Thumbnail=saveUrl+newThumbnailFileName;
                                    int w = 0;
                                    int h = 0;
                                    Common.ImageHelp.LocalImage2Thumbs2(filePath, dirPath + newThumbnailwFileName, 200, 300, "W",ref w,ref h);
                                    a.Thumbnailw_width = w;
                                    a.Thumbnailw_height = h;
                                    a.Thumbnailw = saveUrl + newThumbnailwFileName ;
                                    _repository._PhotoRepositories.UpdatePhoto(a);
                                    return Json(new { Result = "OK" });
                                }
                                else
                                {
                                    return Json(new { Result = "ERROR", Message = "上传图片格式错误" });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { Result = "ERROR", Message = "请选择上传图片" });
                        }
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
        public ActionResult AddAlbum(int uid)
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
                        string albumname = Request["albumname"];
                        string description = Request["description"];
                        HttpPostedFileBase file = Request.Files["coverpath"];
                        if (file != null)
                        {
                            if (file.InputStream == null || file.InputStream.Length > 1024000)
                            {
                                return Json(new { Result = "ERROR", Message = "上传单张图片最大为1M" });
                            }
                            else
                            {
                                string FileType = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);
                                if (FileType == "gif" || FileType == "GIF" || FileType == "jpg" || FileType == "JPG" || FileType == "png" || FileType == "PNG")
                                {
                                    Albums album = new Albums();
                                    album.AlbumName = albumname;
                                    album.Description = description;
                                    album.CreateTime = DateTime.Now;
                                    album.PhotoNum = 0;
                                    album.UID = uid;
                                    album.CoverPath = "";
                                    AlbumValidation albumValidation = new AlbumValidation();
                                    ValidationResult validationResult = albumValidation.Validate(album);
                                    string Msg = "";
                                    if (!validationResult.IsValid)
                                    {
                                        foreach (var failure in validationResult.Errors)
                                        {
                                            Msg += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage;
                                        }
                                        return Json(new { Result = "Error", Message = Msg });
                                    }
                                    var a= _repository._AlbumRepositories.AddAlbum(album);
                                    string savePath = string.Format("/Content/Upload/{0}/{1}/", HttpContext.User.Identity.Name, a.AlbumID);
                                    string saveUrl = "http://" + Request.Url.Authority + savePath;
                                    string dirPath = Server.MapPath(savePath);
                                    if (!Directory.Exists(dirPath))
                                        Directory.CreateDirectory(dirPath);
                                    string fileName = file.FileName;
                                    string fileExt = Path.GetExtension(fileName).ToLower();
                                    string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
                                    string filePath = dirPath + newFileName;
                                    file.SaveAs(filePath);
                                    string fileUrl = saveUrl + newFileName;
                                    a.CoverPath = fileUrl;
                                    _repository._AlbumRepositories.UpdateAlbum(a);
                                    return Json(new { Result = "OK", Message = new { AlbumID = a.AlbumID, CoverPath = a.CoverPath, AlbumName = a.AlbumName, Description = a.Description, CreateTime = a.CreateTime, PhotoNum = a.PhotoNum} });
                                }
                                else
                                {
                                    return Json(new { Result = "ERROR", Message = "上传图片格式错误" });
                                }
                            }
                        }
                        else
                        {
                            return Json(new { Result = "ERROR", Message = "请选择相册封面" });
                        }
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

        [AllowAnonymous]
        public ActionResult Photo(int albumid, string username)
        {
            var user = _repository._UserRepositories.GetUserByUserName(new Users { UserName = username });
            if (user == null)
                return null;
            try
            {
                user.HeadPic = user.HeadPic.Replace("medium", "large");
            }
            catch
            {
                user.HeadPic = "http://" + Request.Url.Authority + "/Content/Avatar/upload/avatars/noavatar_medium.gif";
            }
            ViewData["photos"] = _repository._PhotoRepositories.GetPhotosByAlbumId(albumid);
            ViewBag.AlbumId = albumid;
            ViewBag.AlbumName = _repository._AlbumRepositories.GetAlbum(albumid).AlbumName;
            return View(user);
        }

        public ActionResult GetComments(int pid, string commentusername)
        {
            int uid;
            var u = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername });
            if (u != null)
                uid = _repository._UserRepositories.GetUserByUserName(new Users { UserName = commentusername }).UID;
            else
                uid = 0;
            List<CommentsViewModel> lcom = _repository._PhotoRepositories.GetCommentsByPid(pid, uid);
            foreach (CommentsViewModel com in lcom)
            {
                com.Date = com.TempDate.ToString();
                com.UserAvatar = com.UserAvatar + "?i=" + Guid.NewGuid().ToString();
            }
            return Json(lcom, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize]
        public ActionResult PostComment(int pid, string commentusername, FormCollection collection)
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
                    Comments com = new Comments { PID = pid, UID = uid, CreateTime = DateTime.Now, Content = HttpUtility.HtmlEncode(collection["comment"]) };
                    var parentId = Convert.ToInt32(collection["parentId"]);
                    if (parentId == 0)
                        com.ParentId = null;
                    else
                        com.ParentId = parentId;
                    var commentview = _repository._CommentsRepositories.AddComment(com);
                    _repository._ArticleRepositories.CommentNumPlusOne(pid);
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
        public ActionResult DeleteComment(int pid, string commentusername)
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
                    _repository._ArticleRepositories.CommentNumSubtract(pid, rowcount);
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
