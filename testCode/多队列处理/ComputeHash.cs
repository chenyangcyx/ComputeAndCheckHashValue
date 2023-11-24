using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Waher.Security.SHA3;

namespace CheckHash3
{
    internal class ComputeHash
    {
        public static string getMD5(FileInfo fi)
        {
            using (MD5 md5 = MD5.Create())
            {
                /*if (!File.Exists(fi.FullName))
                {
                    Console.WriteLine(fi.FullName + "不存在！");
                    return null;
                }*/
                using (var read_stream = fi.OpenRead())
                {
                    byte[] hashValue = md5.ComputeHash(read_stream);
                    return convertHashCodeBytes(hashValue);
                }
            }
        }

        public static string getMD5(MergedMemoryStream stream)
        {
            stream.resetStream();
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashValue = md5.ComputeHash(stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        public static string getSHA1(FileInfo fi)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                /*if (!File.Exists(fi.FullName))
                {
                    Console.WriteLine(fi.FullName + "不存在！");
                    return null;
                }*/
                using (var read_stream = fi.OpenRead())
                {
                    byte[] hashValue = sha1.ComputeHash(read_stream);
                    return convertHashCodeBytes(hashValue);
                }
            }
        }

        public static string getSHA1(MergedMemoryStream stream)
        {
            stream.resetStream();
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashValue = sha1.ComputeHash(stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        public static string getSHA2_256(FileInfo fi)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                /*if (!File.Exists(fi.FullName))
                {
                    Console.WriteLine(fi.FullName + "不存在！");
                    return null;
                }*/
                using (var read_stream = fi.OpenRead())
                {
                    byte[] hashValue = sha256.ComputeHash(read_stream);
                    return convertHashCodeBytes(hashValue);
                }
            }
        }

        public static string getSHA2_256(MergedMemoryStream stream)
        {
            stream.resetStream();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue = sha256.ComputeHash(stream);
                return convertHashCodeBytes(hashValue);
            }
        }

        public static string getSHA2_384(FileInfo fi)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                /*if (!File.Exists(fi.FullName))
                {
                    Console.WriteLine(fi.FullName + "不存在！");
                    return null;
                }*/
                using (var read_stream = fi.OpenRead())
                {
                    byte[] hashValue = sha384.ComputeHash(read_stream);
                    return convertHashCodeBytes(hashValue);
                }
            }
        }

        public static string getSHA2_512(FileInfo fi)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                /*if (!File.Exists(fi.FullName))
                {
                    Console.WriteLine(fi.FullName + "不存在！");
                    return null;
                }*/
                using (var read_stream = fi.OpenRead())
                {
                    byte[] hashValue = sha512.ComputeHash(read_stream);
                    return convertHashCodeBytes(hashValue);
                }
            }
        }

        public static string getSHA3_224(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHA3_224 sha = new SHA3_224();
                return convertHashCodeBytes(sha.ComputeVariable(read_stream));
            }
        }

        public static string getSHA3_256(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHA3_256 sha = new SHA3_256();
                return convertHashCodeBytes(sha.ComputeVariable(read_stream));
            }
        }

        public static string getSHA3_384(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHA3_384 sha = new SHA3_384();
                return convertHashCodeBytes(sha.ComputeVariable(read_stream));
            }
        }

        public static string getSHA3_512(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHA3_512 sha = new SHA3_512();
                return convertHashCodeBytes(sha.ComputeVariable(read_stream));
            }
        }

        public static string getSHAKE128(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHAKE128 shake128 = new SHAKE128(128*2);
                return convertHashCodeBytes(shake128.ComputeVariable(read_stream));
            }
        }

        public static string getSHAKE256(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHAKE256 shake256 = new SHAKE256(256*2);
                return convertHashCodeBytes(shake256.ComputeVariable(read_stream));
            }
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
