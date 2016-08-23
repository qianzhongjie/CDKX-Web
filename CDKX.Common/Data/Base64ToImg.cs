using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CDKX.Common.Data
{
    public class Base64ToImg
    {

        private static readonly string ServerHost = ConfigurationManager.AppSettings["ServerHost"];
        //图片 转为    base64编码的文本
        private void ImgToBase64String(string Imagefilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                //this.pictureBox1.Image = bmp;
                FileStream fs = new FileStream(Imagefilename + ".txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                sw.Write(strbaser64);
                sw.Close();
                fs.Close();
                // MessageBox.Show("转换成功!");
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ImgToBase64String 转换失败\nException:" + ex.Message);
            }
        }

        //base64编码的文本 转为    图片
        public static string Base64StringToImage(string inputStr, string name)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(inputStr);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                string path = HttpContext.Current.Server.MapPath("~/UploadFile/ModelPic/");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                bmp.Save(path + name + ".jpg", ImageFormat.Jpeg);
                ms.Close();
                bmp.Dispose();
                return path.Replace(HttpContext.Current.Server.MapPath("~"), ServerHost) + name + ".jpg";
            }
            catch (Exception ex)
            {
                return "fall";
            }
        }

        public static string TestBase64(string strPath)
        {
            byte[] bpath = Convert.FromBase64String(strPath);
            strPath = System.Text.ASCIIEncoding.Default.GetString(bpath);
            return strPath;
        }

    }
}
