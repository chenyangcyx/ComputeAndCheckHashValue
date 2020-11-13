using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CheckHash
{
    class ComputeHash
    {
        private static MD5 md5 = MD5.Create();
        public static string getMD5(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (var read_stream = fi.OpenRead())
            {
                byte[] hashValue = md5.ComputeHash(read_stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        private static SHA1 sha1 = SHA1.Create();
        public static string getSHA1(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (var read_stream = fi.OpenRead())
            {
                byte[] hashValue = sha1.ComputeHash(read_stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        private static SHA256 sha256 = SHA256.Create();
        public static string getSHA256(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (var read_stream = fi.OpenRead())
            {
                byte[] hashValue = sha256.ComputeHash(read_stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        private static SHA384 sha384 = SHA384.Create();
        public static string getSHA384(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (var read_stream = fi.OpenRead())
            {
                byte[] hashValue = sha384.ComputeHash(read_stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        private static SHA512 sha512 = SHA512.Create();
        public static string getSHA512(FileInfo fi)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            using (var read_stream = fi.OpenRead())
            {
                byte[] hashValue = sha512.ComputeHash(read_stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        public static string getBLAKE2(FileInfo fi, string hash_type, string blake_path)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            return Utilities.getBLAKEHash_CMD($"\"{blake_path}\"", $"-a {hash_type} \"{fi.FullName}\"").Split(" ")[0];
        }

        public static string getBLAKE3(FileInfo fi, string blake_path)
        {
            if (!File.Exists(fi.FullName))
            {
                Console.WriteLine(fi.FullName + "不存在！");
                return null;
            }
            string cmd = $"\"{blake_path}\" \"{fi.FullName}\"";
            return Utilities.getBLAKEHash_CMD(blake_path, fi.FullName).Split(" ")[0];
        }

        public static string getHashByName(string hash_method, string path, SettingStruct.Rootobject setting)
        {
            switch (hash_method)
            {
                case Utilities.MD5_NAME:
                    return getMD5(new FileInfo(path));
                case Utilities.SHA1_NAME:
                    return getSHA1(new FileInfo(path));
                case Utilities.SHA256_NAME:
                    return getSHA256(new FileInfo(path));
                case Utilities.SHA384_NAME:
                    return getSHA384(new FileInfo(path));
                case Utilities.SHA512_NAME:
                    return getSHA512(new FileInfo(path));
                case Utilities.BLAKE2b_NAME:
                    return getBLAKE2(new FileInfo(path), "blake2b", setting.blake2_exe_path);
                case Utilities.BLAKE2s_NAME:
                    return getBLAKE2(new FileInfo(path), "blake2s", setting.blake2_exe_path);
                case Utilities.BLAKE2bp_NAME:
                    return getBLAKE2(new FileInfo(path), "blake2bp", setting.blake2_exe_path);
                case Utilities.BLAKE2sp_NAME:
                    return getBLAKE2(new FileInfo(path), "blake2sp", setting.blake2_exe_path);
                case Utilities.BLAKE3_NAME:
                    return getBLAKE3(new FileInfo(path), setting.blake3_exe_path);
            }
            return null;
        }

        public static string convertHashCodeBytes(byte[] hashvalue)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashvalue.Length; i++)
                builder.Append(hashvalue[i].ToString("x2"));
            return builder.ToString();
        }
    }
}
