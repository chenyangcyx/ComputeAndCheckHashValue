using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ComputeHash
{
    class Utilities
    {
        public const string MD5_NAME = "md5";
        public const string SHA1_NAME = "sha1";
        public const string SHA256_NAME = "sha256";
        public const string SHA384_NAME = "sha384";
        public const string SHA512_NAME = "sha512";
        public const string BLAKE2b_NAME = "blake2b";
        public const string BLAKE2s_NAME = "blake2s";
        public const string BLAKE2bp_NAME = "blake2bp";
        public const string BLAKE2sp_NAME = "blake2sp";
        public const string BLAKE3_NAME = "blake3";
        public static UTF8Encoding utf8_encoding = new UTF8Encoding(false);

        public static SettingStruct.Rootobject getSetting(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("文件：" + path + "不存在，退出！");
                Environment.Exit(2333);
            }
            return JsonSerializer.Deserialize<SettingStruct.Rootobject>(File.ReadAllText(path));
        }

        public static Dictionary<string, string> getHashDictionary(string[] all_line)
        {
            Dictionary<string, string> hash_dic = new Dictionary<string, string>();
            foreach (string line in all_line)
                hash_dic.Add(line.Split(" ")[1], line.Split(" ")[0]);
            return hash_dic;
        }

        private static string runCMD_Linux(string cmd)
        {
            string result = null;
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "bash";
                p.StartInfo.Arguments = $"-c \"{cmd}\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                result = p.StandardOutput.ReadToEnd();
                p.Close();
            }
            return result;
        }

        private static string runCMD_Windows(string exe_name, string args)
        {
            string result = null;
            using (Process p = new Process())
            {
                p.StartInfo.FileName = exe_name;
                p.StartInfo.Arguments = args;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                result = p.StandardOutput.ReadToEnd();
                p.Close();
            }
            return result;
        }

        public static void chmodBLAKEprogram(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                runCMD_Linux($"chmod 777 \"{path}\"");
            }
        }

        public static void deleteFolder_CMD(string path)
        {
            try
            {
                new DirectoryInfo(path).Delete(true);
            }
            catch (Exception) { }
        }

        public static void createFolder_CMD(string path)
        {
            new DirectoryInfo(path).Create();
        }

        public static string getBLAKEHash_CMD(string blake_path, string args)
        {
            string result = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return runCMD_Linux(blake_path + " " + args);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return runCMD_Windows(blake_path, args);
            }
            return result;
        }
    }
}
