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

        public static void copyFile(string file_path, string dst_folder)
        {
            runCMD($"cp \"{file_path}\" \"{dst_folder}\"");
        }

        public static string runCMD(string cmd)
        {
            string result = null;
            using (Process p = new Process())
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    p.StartInfo.FileName = cmd.Split(" ")[0];
                    p.StartInfo.Arguments = cmd.Replace(cmd.Split(" ")[0] + " ", "");
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    p.WaitForExit();
                    result = p.StandardOutput.ReadToEnd();
                    p.Close();
                }
            }
            return result;
        }
    }
}
