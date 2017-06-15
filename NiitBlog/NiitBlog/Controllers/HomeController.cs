using NiitBlog.Models;
using NiitBlog.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiitBlog.Controllers
{
    public class HomeController : Controller
    {
        private IRepositoryContainer _repository;

        public HomeController()
        {
            this._repository = new RepositoryContainer(); 
        }
        public ActionResult Index()
        {
            ViewData["LatestArticle"] = _repository._ArticleRepositories.GetLatestArticle(6)
                  .Select(n => new LatestArticleViewModel
                  {
                      AID = n.AID,
                      Title = n.Title,
                      Summery = n.Summery,
                      CreateTime = n.CreateTime,
                      CommentNum = n.CommentNum,
                      ViewNum = n.ViewNum,
                      UserName = n.Users.UserName,
                      HeadPic = n.Users.HeadPic,
                  }).ToList();
            return View();
          
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult GetPhotoList(int page, int ListItemNum)
        {
            var ret = _repository._PhotoRepositories.GetPhotoList(page, ListItemNum)
                .Select(n => new
                {
                    PID = n.PID,
                    Path = n.Path,
                    Thumbnailw = n.Thumbnailw,
                    width = n.Thumbnailw_width,
                    height = n.Thumbnailw_height,
                    PhotoName = n.PhotoName,
                    Description = n.Description,
                    UserName = n.Users.UserName,
                    AlbumID = n.AlbumID,
                }).ToList();
            return Json(ret);
        }
    }
}