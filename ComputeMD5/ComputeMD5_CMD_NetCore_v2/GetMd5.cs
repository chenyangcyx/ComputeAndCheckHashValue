﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ComputeMD5_CMD_NetCore_v2
{
    class GetMd5
    {
        private static MD5 md5 = MD5.Create();
        public static string GetMd5HashValue(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            byte[] hashValue = md5.ComputeHash(fi.OpenRead());
            return ConvertHashCodeBytes(hashValue);
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
