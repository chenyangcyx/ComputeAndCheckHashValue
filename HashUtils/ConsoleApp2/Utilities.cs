using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ConsoleApp2
{
    internal class Utilities
    {
        /** hash方法名称 START */
        public const string MD5_NAME = "MD5";
        public const string SHA1_NAME = "SHA1";
        public const string SHA2_256_NAME = "SHA2-256";
        public const string SHA2_384_NAME = "SHA2-384";
        public const string SHA2_512_NAME = "SHA2-512";
        public const string SHA3_224_NAME = "SHA3-224";
        public const string SHA3_256_NAME = "SHA3-256";
        public const string SHA3_384_NAME = "SHA3-384";
        public const string SHA3_512_NAME = "SHA3-512";
        public const string SHAKE128_NAME = "SHAKE128";
        public const string SHAKE256_NAME = "SHAKE256";
        public const string BLAKE2b_NAME = "BLAKE2b";
        public const string BLAKE2s_NAME = "BLAKE2s";
        public const string BLAKE2bp_NAME = "BLAKE2bp";
        public const string BLAKE2sp_NAME = "BLAKE2sp";
        public const string BLAKE3_NAME = "BLAKE3";
        /** hash方法名称 END */

        /** hash方法参数 START */
        public const int SHAKE128_SIZE = 128 * 2;
        public const int SHAKE256_SIZE = 256 * 2;
        /** hash方法参数 END */

        /** 程序内嵌文件名称 START */
        public const string EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_LINUX = "b2sum-amd64-linux";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_WINDOWS = "b2sum-amd64-windows.exe";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_LINUX = "b3sum_linux_x64_bin";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_WINDOWS = "b3sum_windows_x64_bin.exe";
        public const string EMBEDDED_RESOURCE_NAME_SETTING_DEMO_JSON = "setting_demo.json";
        public const string EMBEDDED_RESOURCE_NAME_SETTING_TARGET_JSON = "setting_demo_{}.json";
        /** 程序内嵌文件名称 END */

        /** 程序运行常量 START */
        public static UTF8Encoding utf8_encoding = new UTF8Encoding(false);
        public const string HASH_FILE_NAME = "hash.txt";
        public const string HASH_FILE_SPLIT_LINE_CONTENT = "----------------------------------------";
        public const string HASH_FILE_FILE_NAME_START = "[name] ";
        /** 程序运行常量 START */

        /** 程序运行必要参数（运行前必须设置） START */
        public static string PROGRAM_RUNNING_PARAM_TEMP_FOLDER = null;
        /** 程序运行必要参数（运行前必须设置） END */

        public static SettingStruct.SettingConfig getSetting(string path)
        {
            return JsonSerializer.Deserialize(File.ReadAllText(path), SettingStructJsonContext.Default.SettingConfig);
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

        public static void setAllFolderInfo(Dictionary<string, List<FileInfo>> local_all_file_dic_list, List<string> check_folder_list)
        {
            foreach (var path in check_folder_list)
            {
                if (!Directory.Exists(path))
                {
                    continue;
                }
                local_all_file_dic_list.Add(path, new List<FileInfo>(new DirectoryInfo(path).GetFiles()));
            }
        }

        public static List<string> getAllFileInFolder(string path, Dictionary<string, List<FileInfo>> local_all_file_dic_list, SettingStruct.SettingConfig setting)
        {
            List<string> result = new List<string>();

            if (local_all_file_dic_list.ContainsKey(path))
            {
                foreach (var file in local_all_file_dic_list[path])
                    result.Add(file.FullName);
            }
            return result;
        }

        public static Dictionary<string, Dictionary<string, string>> getHashResultDict(List<string> all_line_list)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            string name = null;
            Dictionary<string, string> hash_result = new Dictionary<string, string>();
            foreach (var line in all_line_list)
            {
                // 属性标签
                if (line.StartsWith("["))
                {
                    // name属性
                    if (line.StartsWith(HASH_FILE_FILE_NAME_START))
                    {
                        name = line.Substring(HASH_FILE_FILE_NAME_START.Length);
                        continue;
                    }
                    // hash方法
                    string[] line_split = line.Split(" ");
                    string hash_method_name = line_split[0].Replace("[", "").Replace("]", "");
                    switch (hash_method_name)
                    {
                        case MD5_NAME:
                            hash_result.Add(MD5_NAME, line_split[1]);
                            break;
                        case SHA1_NAME:
                            hash_result.Add(SHA1_NAME, line_split[1]);
                            break;
                        case SHA2_256_NAME:
                            hash_result.Add(SHA2_256_NAME, line_split[1]);
                            break;
                        case SHA2_384_NAME:
                            hash_result.Add(SHA2_384_NAME, line_split[1]);
                            break;
                        case SHA2_512_NAME:
                            hash_result.Add(SHA2_512_NAME, line_split[1]);
                            break;
                        case SHA3_224_NAME:
                            hash_result.Add(SHA3_224_NAME, line_split[1]);
                            break;
                        case SHA3_256_NAME:
                            hash_result.Add(SHA3_256_NAME, line_split[1]);
                            break;
                        case SHA3_384_NAME:
                            hash_result.Add(SHA3_384_NAME, line_split[1]);
                            break;
                        case SHA3_512_NAME:
                            hash_result.Add(SHA3_512_NAME, line_split[1]);
                            break;
                        case SHAKE128_NAME:
                            hash_result.Add(SHAKE128_NAME, line_split[1]);
                            break;
                        case SHAKE256_NAME:
                            hash_result.Add(SHAKE256_NAME, line_split[1]);
                            break;
                        case BLAKE2b_NAME:
                            hash_result.Add(BLAKE2b_NAME, line_split[1]);
                            break;
                        case BLAKE2s_NAME:
                            hash_result.Add(BLAKE2s_NAME, line_split[1]);
                            break;
                        case BLAKE2bp_NAME:
                            hash_result.Add(BLAKE2bp_NAME, line_split[1]);
                            break;
                        case BLAKE2sp_NAME:
                            hash_result.Add(BLAKE2sp_NAME, line_split[1]);
                            break;
                        case BLAKE3_NAME:
                            hash_result.Add(BLAKE3_NAME, line_split[1]);
                            break;
                        default:
                            break;
                    }
                }
                // 间隔标签
                else if (line.Equals(HASH_FILE_SPLIT_LINE_CONTENT))
                {
                    result.Add(name, new Dictionary<string, string>(hash_result));
                    name = null;
                    hash_result.Clear();
                }
            }
            // 最后仍有一次记录未被写入
            result.Add(name, hash_result);

            return result;
        }

        public static Stream getMainfestResourceStream(string fileName)
        {
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string item in resourceNames)
            {
                if (item.EndsWith(fileName))
                {
                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(item);
                }
            }
            return null;
        }

        public static FileInfo copyEmbeddedResourceToTempFolder(Stream stream, List<string> appendFolderName, string targetFileName)
        {
            List<string> toCombinePath =
            [
                AppDomain.CurrentDomain.BaseDirectory,
                .. appendFolderName,
                targetFileName,
            ];
            // 要复制到的目录
            string path = Path.Combine(toCombinePath.ToArray());
            FileInfo fileInfo = new FileInfo(path);
            // 创建目录
            DirectoryInfo directoryInfo = fileInfo.Directory;
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            // 复制文件
            using (Stream output = File.Create(path))
            {
                stream.CopyTo(output);
            }
            // 判断是否复制成功
            if (fileInfo.Exists)
            {
                return fileInfo;
            }
            return null;
        }
    }
}
