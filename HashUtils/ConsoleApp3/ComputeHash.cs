using System.Security.Cryptography;
using System.Text;
using Waher.Security.SHA3;
using SHA3_256 = Waher.Security.SHA3.SHA3_256;
using SHA3_384 = Waher.Security.SHA3.SHA3_384;
using SHA3_512 = Waher.Security.SHA3.SHA3_512;

namespace ConsoleApp3
{
    internal class ComputeHash
    {
        public static string getMD5(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                return convertHashCodeBytes(MD5.HashData(read_stream));
            }
        }

        public static string getSHA1(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                return convertHashCodeBytes(SHA1.HashData(read_stream));
            }
        }

        public static string getSHA2_256(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                return convertHashCodeBytes(SHA256.HashData(read_stream));
            }
        }

        public static string getSHA2_384(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                return convertHashCodeBytes(SHA384.HashData(read_stream));
            }
        }

        public static string getSHA2_512(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                return convertHashCodeBytes(SHA512.HashData(read_stream));
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
                SHAKE128 shake128 = new SHAKE128(Utilities.SHAKE128_SIZE);
                return convertHashCodeBytes(shake128.ComputeVariable(read_stream));
            }
        }

        public static string getSHAKE256(FileInfo fi)
        {
            using (var read_stream = fi.OpenRead())
            {
                SHAKE256 shake256 = new SHAKE256(Utilities.SHAKE256_SIZE);
                return convertHashCodeBytes(shake256.ComputeVariable(read_stream));
            }
        }

        public static string getBLAKE2(FileInfo fi, string hash_type, string blake_path)
        {
            return Utilities.getBLAKEHash_CMD(blake_path, hash_type, fi.FullName).Split(" ")[0];
        }

        public static string getBLAKE3(FileInfo fi, string blake_path)
        {
            return Utilities.getBLAKEHash_CMD(blake_path, null, fi.FullName).Split(" ")[0];
        }

        public static string getHashByName(string hash_method, string path, SettingStruct.SettingConfig setting)
        {
            switch (hash_method)
            {
                case Utilities.MD5_NAME:
                    return getMD5(new FileInfo(path));
                case Utilities.SHA1_NAME:
                    return getSHA1(new FileInfo(path));
                case Utilities.SHA2_256_NAME:
                    return getSHA2_256(new FileInfo(path));
                case Utilities.SHA2_384_NAME:
                    return getSHA2_384(new FileInfo(path));
                case Utilities.SHA2_512_NAME:
                    return getSHA2_512(new FileInfo(path));
                case Utilities.SHA3_224_NAME:
                    return getSHA3_224(new FileInfo(path));
                case Utilities.SHA3_256_NAME:
                    return getSHA3_256(new FileInfo(path));
                case Utilities.SHA3_384_NAME:
                    return getSHA3_384(new FileInfo(path));
                case Utilities.SHA3_512_NAME:
                    return getSHA3_512(new FileInfo(path));
                case Utilities.SHAKE128_NAME:
                    return getSHAKE128(new FileInfo(path));
                case Utilities.SHAKE256_NAME:
                    return getSHAKE256(new FileInfo(path));
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
