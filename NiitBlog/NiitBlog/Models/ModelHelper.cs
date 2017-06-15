using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace NiitBlog.Models
{
    public class ModelHelper
    {
        public static object GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
    }
}