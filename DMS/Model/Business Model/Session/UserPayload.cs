using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Model.Business.Session
{
    public static class UserPayload
    {
        public static string UserPath
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var userPath = HttpContext.Current.Session["UserPath"];
                if (userPath != null)
                    return userPath as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["UserPath"] = value;
            }
        }

        public static string UserEmail
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var mail = HttpContext.Current.Session["UserEmail"];
                if (mail != null)
                    return mail as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["UserEmail"] = value;
            }
        }

        public static int UserID
        {
            get
            {
                if (HttpContext.Current == null)
                    return -1;
                var userID = HttpContext.Current.Session["UserID"];
                if (userID != null)
                    return (int)userID;
                return -1;
            }
            set
            {
                HttpContext.Current.Session["UserID"] = value;
            }
        }

        public static string UserType
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var userType = HttpContext.Current.Session["UserType"];
                if (userType != null)
                    return userType as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["UserType"] = value;
            }
        }

        public static string ForgetStamp
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                var stamp = HttpContext.Current.Session["ForgetStamp"];
                if (stamp != null)
                    return stamp as string;
                return null;
            }
            set
            {
                HttpContext.Current.Session["ForgetStamp"] = value;
            }
        }

        public static SortedSet<int> FileAccessList
        {
            get
            {
                if (HttpContext.Current.Session["FileAccessList"] == null)
                    return null;
                var set = HttpContext.Current.Session["FileAccessList"];
                if (set != null)
                    return set as SortedSet<int>;
                return null;
            }
            set
            {
                HttpContext.Current.Session["FileAccessList"] = value;
            }
        }
    }
}