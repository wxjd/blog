using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc 
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString OutPut(this HtmlHelper helper, string str)
        {
            return new MvcHtmlString(str);
        }
    }
}