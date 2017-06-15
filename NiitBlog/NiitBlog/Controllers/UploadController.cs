using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiitBlog.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult UploadFile(string dir)
        {
            if (String.IsNullOrEmpty(HttpContext.User.Identity.Name))
                return null;
            Hashtable hash = new Hashtable();
            var p = string.Format("/Content/Attached/{0}/", HttpContext.User.Identity.Name);
            string savePath = p;
            string saveUrl = "http://" + Request.Url.Authority + p;
            int maxSize = 1024000;
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");
            string msg = "";
            switch (dir)
            {
                case "image":
                    var path = string.Format("/Content/Upload/{0}/Article/", HttpContext.User.Identity.Name);
                    savePath = path;
                    saveUrl = "http://" + Request.Url.Authority + path;
                    msg = "上传单张图片最大为1M";
                    break;
                case "flash":
                    maxSize = 10240000;
                    msg = "上传flash文件最大为10M";
                    break;
                case "media":
                    maxSize = 10240000;
                    msg = "上传媒体文件最大为10M";
                    break;
                case "file":
                    maxSize = 102400000;
                    msg = "上传文件最大为100M";
                    break;
            }
            HttpPostedFileBase file = Request.Files["imgFile"];
            string dirPath = Server.MapPath(savePath);
            string fileName = file.FileName;
            string fileExt = Path.GetExtension(fileName).ToLower();
            if (file == null)
                return UploadJsonRe(1, "请选择文件", "");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            if (file.InputStream == null || file.InputStream.Length > maxSize)
                return UploadJsonRe(1, msg, "");
            if (string.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dir]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                return UploadJsonRe(1, "上传文件的扩展名师不允许的扩展名", "");
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            string filePath = dirPath + newFileName;
            file.SaveAs(filePath);
            string fileUrl = saveUrl + newFileName;
            return UploadJsonRe(0, "", fileUrl);
        }

        private JsonResult UploadJsonRe(int error, string message, string url)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = error;
            hash["message"] = message;
            hash["url"] = url;
            return Json(hash, "text/html; charset=UTF-8");
        }
    }
}
