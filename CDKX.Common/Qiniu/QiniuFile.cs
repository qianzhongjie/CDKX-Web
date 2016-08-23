using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qiniu.IO;
using Qiniu.IO.Resumable;
using Qiniu.RS;
using Qiniu.Conf;
using System.IO;

namespace OSharp.Web.Mvc.QiniuFile
{
    public static class QiniuFile
    {
        private static string bucket = "";

        private static string afterPath = "~/UploadFile/Zip/";
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="name">自定义资源名称</param>
        /// <param name="src">路径</param>
        /// <returns></returns>
        public static bool PutFile(string name, string src)
        {
            
            var policy = new PutPolicy(bucket, 3600);
            string upToken = policy.Token();
            PutExtra extra = new PutExtra();
            IOClient client = new IOClient();
            var e = client.PutFile(upToken, name, src, extra);
            return e.OK;
        }

        /// <summary>
        /// 断点续传
        /// </summary>
        /// <param name="name">自定义资源名称</param>
        /// <param name="src">路径</param>
        /// <returns></returns>
        public static bool ResumablePutFile(string name, string src)
        {
            Console.WriteLine("\n===> ResumablePutFile {0}:{1} fname:{2}", bucket, src, name);
            PutPolicy policy = new PutPolicy(bucket, 3600);
            string upToken = policy.Token();
            Settings setting = new Settings();
            ResumablePutExtra extra = new ResumablePutExtra();
            ResumablePut client = new ResumablePut(setting, extra);
            var e = client.PutFile(upToken, name, Guid.NewGuid().ToString());
            return e.OK;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys"></param>
        public static bool BatchDelete(string[] keys)
        {
            RSClient client = new RSClient();
            List<EntryPath> entryPaths = new List<EntryPath>();
            foreach (string key in keys)
            {
                Console.WriteLine(@"\n===> Stat {0}:{1}", bucket, key);
                entryPaths.Add(new EntryPath(bucket, key));
            }
            var e = client.BatchDelete(entryPaths.ToArray());
            return e.OK;
        }



        /// <summary>
        /// 遍历文件夹 获取文件名称并上传七牛
        /// </summary>
        /// <param name="src">文件夹路径</param>
        /// <returns></returns>
        public static List<string> GetFileName(string src)
        {
            DirectoryInfo theFolder = new DirectoryInfo(src);
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();

            //遍历当前文件夹
            List<string> fileNames = new List<string>();
            foreach (var p in theFolder.GetFiles())
            {
                if (p.DirectoryName != null)
                {
                    PutFile(p.DirectoryName.Replace(afterPath, ""), p.DirectoryName);
                    fileNames.Add("host" + p.DirectoryName.Replace(afterPath, ""));
                }
            }
            //遍历子文件夹
            foreach (DirectoryInfo nextFolder in dirInfo)
            {
                FileInfo[] fileInfo = nextFolder.GetFiles();
                foreach (var x in fileInfo)
                {
                    if (x.DirectoryName != null)
                    {
                        PutFile(x.DirectoryName.Replace(afterPath, ""), x.DirectoryName);
                        fileNames.Add("host" + x.DirectoryName.Replace(afterPath, ""));
                    }
                }
            }
            return fileNames;
        }
    }
}
