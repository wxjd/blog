using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;

namespace NiitBlog.Common
{
    public static class DownloadHelper
    {
        /// <summary>
        /// 把内容中的图片下载到本地
        /// </summary>
        /// <param name="htmlContent"></param>
        /// <returns></returns>
        public static string DownloadImages(string htmlContent, string folderPath, string webUrl)
        {
            string newContent = htmlContent;
            //实例化HtmlAgilityPack.HtmlDocument对象
            HtmlDocument doc = new HtmlDocument();
            //载入HTML
            doc.LoadHtml(htmlContent);
            var imgs = doc.DocumentNode.SelectNodes("//img");
            if (imgs != null && imgs.Count > 0)
            {
                foreach (HtmlNode child in imgs)
                {
                    if (child.Attributes["src"] == null)
                        continue;

                    string imgurl = child.Attributes["src"].Value;

                    if (imgurl.IndexOf(webUrl) > -1 || imgurl.IndexOf("http://") == -1)
                        continue;

                    string newimgurl = DownLoadImg(imgurl, folderPath);
                    if (newimgurl != "")
                    {
                        newContent = newContent.Replace(imgurl, webUrl + newimgurl);
                    }
                }
            }
            return newContent;
        }


        /// <summary>
        /// 将远程图片保存到服务器
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="filePath"></param>
        public static string DownLoadImg(string url, string folderPath, bool isoldname = false)
        {
            try
            {
                string fileName = "";
                fileName = url.Substring(url.LastIndexOf("/") + 1);
                if (fileName.IndexOf("?") > -1)
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("?"));
                }
                fileName = CleanInvalidFileName(fileName);
                if (!isoldname)
                {
                    string extension = fileName.Split('.')[1];
                    // 生成随机文件名
                    Random random = new Random(DateTime.Now.Millisecond);
                    fileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + "." + extension;
                }
                var fileFolder = System.Web.HttpContext.Current.Server.MapPath(folderPath);
                DownloadImage(url, fileFolder + fileName);
                return folderPath + fileName;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 验证文件名
        /// </summary>
        private static readonly char[] InvalidFileNameChars = new[]
                                                                  {
                                                                      '"',
                                                                      '<',
                                                                      '>',
                                                                      '|',
                                                                      '\0',
                                                                      '\u0001',
                                                                      '\u0002',
                                                                      '\u0003',
                                                                      '\u0004',
                                                                      '\u0005',
                                                                      '\u0006',
                                                                      '\a',
                                                                      '\b',
                                                                      '\t',
                                                                      '\n',
                                                                      '\v',
                                                                      '\f',
                                                                      '\r',
                                                                      '\u000e',
                                                                      '\u000f',
                                                                      '\u0010',
                                                                      '\u0011',
                                                                      '\u0012',
                                                                      '\u0013',
                                                                      '\u0014',
                                                                      '\u0015',
                                                                      '\u0016',
                                                                      '\u0017',
                                                                      '\u0018',
                                                                      '\u0019',
                                                                      '\u001a',
                                                                      '\u001b',
                                                                      '\u001c',
                                                                      '\u001d',
                                                                      '\u001e',
                                                                      '\u001f',
                                                                      ':',
                                                                      '*',
                                                                      '?',
                                                                      '\\',
                                                                      '/'
                                                                  };

        public static string CleanInvalidFileName(string fileName)
        {
            fileName = fileName + "";
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));

            if (fileName.Length > 1)
                if (fileName[0] == '.')
                    fileName = "dot" + fileName.TrimStart('.');

            return fileName;
        }

        /// <summary>
        /// 将远程图片保存到服务器
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="filePath"></param>
        public static void DownloadImage(string remotePath, string filePath)
        {
            WebClient w = new WebClient();
            try
            {
                w.DownloadFile(remotePath, filePath);
            }
            finally
            {
                w.Dispose();
            }
        }
    }
}