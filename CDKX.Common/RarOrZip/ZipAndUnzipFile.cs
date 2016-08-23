using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace CDKX.Common.RarOrZip
{
    public static class ZipAndUnzipFile
    {

        private static string Hoststring = ConfigurationManager.AppSettings["ServerHost"];
        private static string afterPath = "~/UploadFile/Zip/";
        /// <summary>
        /// //使用BZIP压缩文件的方法
        /// </summary>
        /// <param name="sourcefilename"></param>
        /// <param name="zipfilename"></param>
        /// <returns></returns>
        public static bool BZipFile(string sourcefilename, string zipfilename)
        {
            bool blResult; //表示压缩是否成功的返回结果
            //为源文件创建文件流实例，作为压缩方法的输入流参数
            FileStream srcFile = File.OpenRead(sourcefilename);
            //为压缩文件创建文件流实例，作为压缩方法的输出流参数
            FileStream zipFile = File.Open(zipfilename, FileMode.Create);
            try
            {
                //以4096字节作为一个块的方式压缩文件
                BZip2.Compress(srcFile, zipFile, 4096);
                blResult = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                blResult = false;
            }
            srcFile.Close(); //关闭源文件流
            zipFile.Close(); //关闭压缩文件流
            return blResult;
        }

        /// <summary>
        /// //使用BZIP解压文件的方法
        /// </summary>
        /// <param name="zipfilename">文件路径</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static List<string> UnBzipFile(string zipfilename, string fileName)
        {
            List<string> fileNames = new List<string>();
            zipfilename = System.Web.HttpContext.Current.Server.MapPath(zipfilename);
            var un = zipfilename.Split('\\');
            var unPath = zipfilename.Replace(un[un.Length - 1], "");
            if (!Directory.Exists(unPath + fileName))
                Directory.CreateDirectory(unPath + fileName);
            if (UnZip(zipfilename, unPath + fileName, ""))
            {
                try
                {


                    DirectoryInfo theFolder = new DirectoryInfo(unPath + fileName);
                    DirectoryInfo[] dirInfo = theFolder.GetDirectories();


                    //遍历文件夹
                    foreach (DirectoryInfo nextFolder in dirInfo)
                    {
                        FileInfo[] fileInfo = nextFolder.GetFiles();
                        fileNames.AddRange(fileInfo.Select(x => x.FullName.Replace(System.Web.HttpContext.Current.Server.MapPath("~/"), Hoststring)));

                        DirectoryInfo[] chrid = nextFolder.GetDirectories();
                        //遍历文zi件夹
                        foreach (DirectoryInfo chridinfo in chrid)
                        {
                            FileInfo[] fileInfoY = chridinfo.GetFiles();
                            fileNames.AddRange(fileInfoY.Select(y => y.FullName.Replace(System.Web.HttpContext.Current.Server.MapPath("~/"), Hoststring)));
                        }
                    }
                }
                catch (Exception e)
                {
                }
            }
            //处理完成 删除原文件
            //File.Delete(zipfilename);
            return fileNames;
        }

        /// <summary>
        ///   //使用GZIP压缩文件的方法
        /// </summary>
        /// <param name="sourcefilename"></param>
        /// <param name="zipfilename"></param>
        /// <returns></returns>
        public static bool GZipFile(string sourcefilename, string zipfilename)
        {
            bool blResult; //表示压缩是否成功的返回结果
            //为源文件创建读取文件的流实例
            FileStream srcFile = File.OpenRead(sourcefilename);
            //为压缩文件创建写入文件的流实例，
            GZipOutputStream zipFile = new GZipOutputStream(File.Open(zipfilename, FileMode.Create));
            try
            {
                byte[] fileData = new byte[srcFile.Length]; //创建缓冲数据
                srcFile.Read(fileData, 0, (int)srcFile.Length); //读取源文件
                zipFile.Write(fileData, 0, fileData.Length); //写入压缩文件
                blResult = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                blResult = false;
            }
            srcFile.Close(); //关闭源文件
            zipFile.Close(); //关闭压缩文件
            return blResult;
        }

        /// <summary>
        /// //使用GZIP解压文件的方法
        /// </summary>
        /// <param name="zipfilename"></param>
        /// <param name="unzipfilename"></param>
        /// <returns></returns>
        public static bool UnGzipFile(string zipfilename, string unzipfilename)
        {
            bool blResult; //表示解压是否成功的返回结果
            //创建压缩文件的输入流实例
            GZipInputStream zipFile = new GZipInputStream(File.OpenRead(zipfilename));
            //创建目标文件的流
            FileStream destFile = File.Open(unzipfilename, FileMode.Create);
            try
            {
                int buffersize = 2048; //缓冲区的尺寸，一般是2048的倍数
                byte[] fileData = new byte[buffersize]; //创建缓冲数据
                while (buffersize > 0) //一直读取到文件末尾
                {
                    buffersize = zipFile.Read(fileData, 0, buffersize); //读取压缩文件数据
                    destFile.Write(fileData, 0, buffersize); //写入目标文件
                }
                blResult = true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                blResult = false;
            }
            destFile.Close(); //关闭目标文件
            zipFile.Close(); //关闭压缩文件
            return blResult;
        }


        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>   
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!File.Exists(fileToUnZip))
                return false;

            //if (!Directory.Exists(zipedFolder))
            //    Directory.CreateDirectory(zipedFolder);
            // File.Create(zipedFolder);
            try
            {
                zipStream = new ZipInputStream(File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi   

                        if (fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        fs = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = fs.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }

    }
}
