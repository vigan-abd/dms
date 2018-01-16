using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Helpers.Web
{
    public static class DirectoryHelper
    {
        public static string StorageDirectory = "Storage";
        public static string GetWebHostUrl()
        {
            return HttpContext.Current.Request.Url.Scheme + "://" + (HttpContext.Current.Request.Url.Port == 80 ?
                HttpContext.Current.Request.Url.Host :
            HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port
            );
        }

        public static string GetRealPath(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        public static string GetRootStorage()
        {
            return HostingEnvironment.MapPath("~/" + StorageDirectory);
        }

        public static string Basename(string path)
        {
            path = path.Replace("\\", "/");
            path = path.TrimEnd('/');
            path = path.Substring(path.LastIndexOf("/") + 1);
            return path;
        }

        public static string Extension(string file)
        {
            return file.Substring(file.LastIndexOf('.') + 1);
        }

        public static double GBtoByte(double num)
        {
            return num * 1000000000;
        }

        public static double DirectorySize(string relativepath)
        {
            double size = 0;
            DirectoryInfo dr = new DirectoryInfo(GetRootStorage() + relativepath);
            var files = dr.EnumerateFiles();
            foreach (var item in files)
            {
                size += item.Length;
            }
            return size;
        }

        public static void RenameFile(string file, string newfile, string pathtofile)
        {
            if (File.Exists(pathtofile + "/" +newfile))
            {
                throw new Exception("File already exists");
            }

            File.Move(pathtofile + "/" + file, pathtofile + "/" + newfile);
        }

        public static void DeleteFile(string dir, string file)
        {
            if (File.Exists(dir + "/" + file))
                File.Delete(dir + "/" + file);
        }
    }
}
