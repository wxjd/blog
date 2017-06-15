<%@ WebHandler Language="C#" Class="Ajax" %>

using System;
using System.Web;
using System.IO;
using System.Web.Mvc;
using NiitBlog.Models;

public class Ajax : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string uid = context.Request.QueryString["input"];
        if (!string.IsNullOrEmpty(context.Request["Filename"]) && !string.IsNullOrEmpty(context.Request["Upload"]))
        {
            ResponseText(UploadTempAvatar(uid));
        }
        if (!string.IsNullOrEmpty(context.Request["avatar1"]) && !string.IsNullOrEmpty(context.Request["avatar2"]) && !string.IsNullOrEmpty(context.Request["avatar3"]))
        {
            CreateDir(uid);
            if (!(SaveAvatar("avatar1", uid) && SaveAvatar("avatar2", uid) && SaveAvatar("avatar3", uid)))
            {
                File.Delete(GetMapPath("Content\\Avatar\\upload\\avatars\\" + uid + ".jpg"));
                ResponseText("<?xml version=\"1.0\" ?><root><face success=\"0\"/></root>");
                return;
            }
            File.Delete(GetMapPath("Content\\Avatar\\upload\\avatars\\" + uid + ".jpg"));
            ResponseText("<?xml version=\"1.0\" ?><root><face success=\"1\"/></root>");
            return;
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    private void CreateDir(string uid)
    {
        string avatarDir = string.Format("Content/Avatar/upload/avatars/{0}",
             uid);
        if (!Directory.Exists(GetMapPath(avatarDir)))
            Directory.CreateDirectory(GetMapPath(avatarDir));
    }

    private void ResponseText(string text)
    {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.Write(text);
        HttpContext.Current.Response.End();
    }

    private string UploadTempAvatar(string uid)
    {
        string filename = uid + ".jpg";
        string uploadUrl = GetRootUrl("Content/Avatar/") + "upload/avatars";
        string uploadDir = GetMapPath("Content\\Avatar\\upload\\avatars");
        if (!Directory.Exists(uploadDir + "temp\\"))
            Directory.CreateDirectory(uploadDir + "temp\\");

        filename = "temp/" + filename;
        if (HttpContext.Current.Request.Files.Count > 0)
        {
            HttpContext.Current.Request.Files[0].SaveAs(uploadDir + filename);
        }

        return uploadUrl + filename;
    }

    private byte[] FlashDataDecode(string s)
    {
        byte[] r = new byte[s.Length / 2];
        int l = s.Length;
        for (int i = 0; i < l; i = i + 2)
        {
            int k1 = ((int)s[i]) - 48;
            k1 -= k1 > 9 ? 7 : 0;
            int k2 = ((int)s[i + 1]) - 48;
            k2 -= k2 > 9 ? 7 : 0;
            r[i / 2] = (byte)(k1 << 4 | k2);
        }
        return r;
    }

    private bool SaveAvatar(string avatar, string uid)
    {
        
        byte[] b = FlashDataDecode(HttpContext.Current.Request[avatar]);
        if (b.Length == 0)
            return false;
        string size = "";
        if (avatar == "avatar1")
            size = "large";
        else if (avatar == "avatar2")
        {
            size = "medium";
            string headpic = string.Format("/Content/Avatar/upload/avatars/{0}/medium.jpg",uid);
            IRepositoryContainer repository = new RepositoryContainer();
            var user= repository._UserRepositories.GetUserByUserName(new Users { UserName = uid });
            user.HeadPic = headpic;
            repository._UserRepositories.UpdateHeadPic(user);
        }
        else
            size = "small";
        string avatarFileName = string.Format("Content/Avatar/upload/avatars/{0}/{1}.jpg",
            uid, size);

        FileStream fs = new FileStream(GetMapPath(avatarFileName), FileMode.Create);
        fs.Write(b, 0, b.Length);
        fs.Close();
        return true;
    }

    public static string GetRootUrl(string forumPath)
    {
        string ApplicationPath = HttpContext.Current.Request.ApplicationPath != "/" ? HttpContext.Current.Request.ApplicationPath : string.Empty;
        int port = HttpContext.Current.Request.Url.Port;
        return string.Format("{0}://{1}{2}{3}/{4}",
                             HttpContext.Current.Request.Url.Scheme,
                             HttpContext.Current.Request.Url.Host,
                             (port == 80 || port == 0) ? "" : ":" + port,
                             ApplicationPath,
                             forumPath);
    }

    public static string GetMapPath(string strPath)
    {
        if (HttpContext.Current != null)
        {
            return HttpContext.Current.Server.MapPath(strPath);
        }
        else //非web程序引用
        {
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }
    }
}