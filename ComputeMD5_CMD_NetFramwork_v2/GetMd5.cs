using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ComputeMD5_CMD_NetFramwork_v2
{
    class GetMd5
    {
        public static string GetMd5HashValue(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine(path + "不存在！");
                return null;
            }
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(new FileInfo(path).OpenRead());
                return ConvertHashCodeBytes(hashValue);
            }
        }

        public static string GetMd5HashValue(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(fi.OpenRead());
                return ConvertHashCodeBytes(hashValue);
            }
        }
        public static string ConvertHashCodeBytes(byte[] hashvalue)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashvalue.Length; i++)
                builder.Append(hashvalue[i].ToString("x2"));
            return builder.ToString();
        }
    }
}
