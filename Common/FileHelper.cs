using CloudSalon.Model.Enum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalon.Common
{
    public class FileHelper
    {
        public static string SaveImageWithThumbnail(string folder, string base64Str, int width, int height)
        {
            byte[] bytes = Convert.FromBase64String(base64Str);
            MemoryStream m = new MemoryStream(bytes);
            Image image = Image.FromStream(m);
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            FileStream file = new FileStream(folder + fileName, FileMode.Create);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
            file.Dispose();
            file = null;

            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            image.GetThumbnailImage(width, height, callb, IntPtr.Zero).Save(folder + "s" + fileName);

            return fileName;
        }

        public static string SaveImage(string folder, string base64Str)
        {
            byte[] bytes = Convert.FromBase64String(base64Str);
            string fileName = Guid.NewGuid().ToString() + ".jpg";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            FileStream file = new FileStream(folder + fileName, FileMode.Create);
            file.Write(bytes, 0, bytes.Length);
            file.Close();
            file.Dispose();
            file = null;

            return fileName;
        }

        public static bool ThumbnailCallback()
        {
            return false;
        }


        public static void DeleteImage(string fileName, bool isDeleteThumbnail)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);

                if (isDeleteThumbnail)
                {
                    string ShrunkenImg = Path.GetFileName(fileName);
                    ShrunkenImg = fileName.Replace(ShrunkenImg, "s" + ShrunkenImg);

                    if (File.Exists(ShrunkenImg))
                        File.Delete(ShrunkenImg);
                }
            }
            catch (IOException ioe)
            {
                //IOException表示要删除的文件正在被其他handle引用
                //如果是这个异常，不处理此异常，程序继续
                //如果是PathTooLongException，DirectoryNotFoundException，因为是IOException的子异常，也会被catch，仍需向上抛，让框架捕获，以便日志
                if (ioe.GetType() != typeof(IOException))
                    throw ioe;
            }
        }
    }
}
