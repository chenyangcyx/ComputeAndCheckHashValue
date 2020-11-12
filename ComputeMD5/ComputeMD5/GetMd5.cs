using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ComputeMD5
{
    class GetMd5
    {
        public string GetMd5HashValue(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine(path + "不存在！");
                return null;
            }
            using (MD5 md5 = MD5.Create())
            {
                FileStream fileStream = new FileInfo(path).OpenRead();
                fileStream.Position = 0;
                byte[] hashValue = md5.ComputeHash(fileStream);
                fileStream.Close();
                return ConvertHashCodeBytes(hashValue);
            }
        }

        public string GetMd5HashValue(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (MD5 md5 = MD5.Create())
            {
                FileStream fileStream = fi.OpenRead();
                fileStream.Position = 0;
                byte[] hashValue = md5.ComputeHash(fileStream);
                fileStream.Close();
                return ConvertHashCodeBytes(hashValue);
            }
        }
        public string ConvertHashCodeBytes(byte[] hashvalue)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashvalue.Length; i++)
                builder.Append(hashvalue[i].ToString("x2"));
            return builder.ToString();
        }
    }
}
