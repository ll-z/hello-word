using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DemonForWorldZhu.Common
{
   public class StringRegulation
   {
        /// <summary>
        /// 判断是否为链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool ValidateUrl(string url)
        {
            Regex validipregex = new Regex(@"^((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?");
            return (url != "" && validipregex.IsMatch(url.Trim())) ? true : false;
        }

        /// <summary>
        /// 判断是否为邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidateEmail(string email)
        {
            Regex validipregex = new Regex(@"^(\w+\.) * \w+@(\w+\.)+[A-Za-z]+");
            return (email != "" && validipregex.IsMatch(email.Trim())) ? true : false;
        }

    }
}
