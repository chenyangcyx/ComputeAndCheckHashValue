using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace CheckHash
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

        private static string runCMD_Linux(string cmd)
        {
            StringBuilder sb = new StringBuilder();
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "bash";
                p.StartInfo.Arguments = $"-c \"{cmd}\"";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                using (var sr = p.StandardOutput)
                {
                    while (!sr.EndOfStream)
                    {
                        string _t = sr.ReadLine();
                        sb.Append(_t + '\n');
                    }
                }
                p.WaitForExit();
                p.Close();
            }
            return sb.ToString();
        }

        private static string runCMD_Windows(string exe_name, string args)
        {
            StringBuilder sb = new StringBuilder();
            using (Process p = new Process())
            {
                p.StartInfo.FileName = exe_name;
                p.StartInfo.Arguments = args;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                using (var sr = p.StandardOutput)
                {
                    while (!sr.EndOfStream)
                    {
                        string _t = sr.ReadLine();
                        sb.Append(_t + '\n');
                    }
                }
                p.WaitForExit();
                p.Close();
            }
            return sb.ToString();
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

        public static string getBLAKEHash_CMD(string blake_path, string type, string file_path)
        {
            string result = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (type == null)
                    return runCMD_Linux($"\'{blake_path}\' \'{file_path}\'");
                else
                    return runCMD_Linux($"\'{blake_path}\' -a {type} \'{file_path}\'");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (type == null)
                    return runCMD_Windows(blake_path, $"\"{file_path}\"");
                else
                    return runCMD_Windows(blake_path, $"-a {type} \"{file_path}\"");
            }
            return result;
        }

        public static void copyFile(string old_file_path, string new_file_path, bool use_rclone,string rclone_config_file)
        {
            if (!use_rclone)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    runCMD_Linux($"cp -f \'{old_file_path}\' \'{new_file_path}\'");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runCMD_Windows("copy",$"/y \"{old_file_path}\" \"{new_file_path}\"");
                }
            }
            else
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    runCMD_Linux($"rclone --config \'{rclone_config_file}\' copy \'{old_file_path}\' \'{new_file_path}\'");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    runCMD_Windows("rclone",$"--config \"{rclone_config_file}\" copy \"{old_file_path}\" \"{new_file_path}\"");
                }
            }
        }

        public static void setAllFolderInfo(Dictionary<string, List<RcloneFileList.FileInfo>> rclone_all_file_dic_list, Dictionary<string, List<FileInfo>> local_all_file_dic_list, SettingStruct.Rootobject setting)
        {
            // rclone模式下
            if (setting.use_rclone == 1)
            {
                foreach(var path in setting.check_folder)
                {
                    string lsjson_result = null;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        lsjson_result = runCMD_Linux($"rclone --config \'{setting.rclone_config_file}\' lsjson \'{path}\'");
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        lsjson_result = runCMD_Windows("rclone", $"--config \"{setting.rclone_config_file}\" lsjson \"{path}\"");
                    rclone_all_file_dic_list.Add(path, JsonSerializer.Deserialize<List<RcloneFileList.FileInfo>>(lsjson_result));
                }
            }
            // 本地模式下
            else
            {
                foreach(var path in setting.check_folder)
                {
                    local_all_file_dic_list.Add(path, new List<FileInfo>(new DirectoryInfo(path).GetFiles()));
                }
            }
        }

        public static List<string> getAllFileInFolder(string path,Dictionary<string, List<RcloneFileList.FileInfo>> rclone_all_file_dic_list, Dictionary<string, List<FileInfo>> local_all_file_dic_list, SettingStruct.Rootobject setting)
        {
            List<string> result = new List<string>();
            if (setting.use_rclone != 1)
            {
                if (local_all_file_dic_list.ContainsKey(path))
                {
                    foreach (var file in local_all_file_dic_list[path])
                        result.Add(file.FullName);
                }
            }
            else
            {
                if (rclone_all_file_dic_list.ContainsKey(path))
                {
                    foreach (var file in rclone_all_file_dic_list[path])
                        result.Add(path + file.Name);
                }
            }
            return result;
        }
    }
}
