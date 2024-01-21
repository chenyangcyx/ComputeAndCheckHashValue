using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace ConsoleApp3
{
    internal class Utilities
    {
        // 程序名称
        public static string PROGRAM_NAME = "CheckHash3";

        /** hash方法名称参数 START */
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
        public const int SHAKE128_SIZE = 128 * 2;
        public const int SHAKE256_SIZE = 256 * 2;
        public static bool CAN_USE_NET_SHAKE_ALGORITHM = false;
        public static bool CAN_USE_NET_SHA3_ALGORITHM = false;
        /** hash方法名称参数 END */

        /** BLAKE程序路径 */
        public static string? BLAKE2_EXE_PATH;
        public static string? BLAKE3_EXE_PATH;
        /** BLAKE程序路径 */

        /** 程序内嵌文件名称 START */
        public const string EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_LINUX = "b2sum-amd64-linux";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_WINDOWS = "b2sum-amd64-windows.exe";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_LINUX = "b3sum_linux_x64_bin";
        public const string EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_WINDOWS = "b3sum_windows_x64_bin.exe";
        public const string EMBEDDED_RESOURCE_NAME_SETTING_DEMO_JSON = "setting_demo.json";
        public const string EMBEDDED_RESOURCE_NAME_SETTING_TARGET_JSON = "setting_demo_{}.json";
        /** 程序内嵌文件名称 END */

        /** 程序运行常量 START */
        public static UTF8Encoding UTF8_ENCODING = new UTF8Encoding(false);
        public const string HASH_FILE_NAME = "hash.txt";
        public const string HASH_FILE_SPLIT_LINE_CONTENT = "----------------------------------------";
        public const string HASH_FILE_CONTENT_NAME_PREFIX = "[name] ";
        public const string PROGRAM_LOG_FILE_NAME = "run_log.txt";
        public const string PROGRAM_LOG_CONTENT_FOLDER_PREFIX = "[folder_path] ";
        public const string PROGRAM_LOG_CONTENT_FILE_PATH_PREFIX = "[file_path] ";
        public const string PROGRAM_LOG_CONTENT_FILE_SIZE_PREFIX = "[size] ";
        public const string PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_START = "============PROGRAM RUNNING LOG START============";
        public const string PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_END = "============PROGRAM RUNNING LOG END============";
        /** 程序运行常量 START */

        /** 程序运行必要参数（运行前必须设置） START */
        public static string? PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME;
        public static string? PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH;
        public static string? PROGRAM_RUNNING_LOG_FILE_PATH;
        /** 程序运行必要参数（运行前必须设置） END */

        public static SettingStruct.SettingConfig getSetting(string path)
        {
            return JsonSerializer.Deserialize<SettingStruct.SettingConfig>(File.ReadAllText(path))!;
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
                        string? _t = sr.ReadLine();
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
                        string? _t = sr.ReadLine();
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

        public static string? getBLAKEHash_CMD(string blake_path, string? type, string file_path)
        {
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
            return null;
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

        // 从hash.txt文件读取hash结果
        // fileName -> (hashName -> hashValue)
        public static Dictionary<string, Dictionary<string, string>> getHashResultDictFromHashFile(List<string> all_line_list)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            string? name = null;
            Dictionary<string, string> hash_result = new Dictionary<string, string>();
            foreach (var line in all_line_list)
            {
                // 属性标签
                if (line.StartsWith("["))
                {
                    // name属性
                    if (line.StartsWith(HASH_FILE_CONTENT_NAME_PREFIX))
                    {
                        name = line.Substring(HASH_FILE_CONTENT_NAME_PREFIX.Length);
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
                    result.Add(name!, new Dictionary<string, string>(hash_result));
                    name = null;
                    hash_result.Clear();
                }
            }
            // 最后仍有一次记录未被写入
            result.Add(name!, hash_result);

            return result;
        }

        // 从run_log.txt文件读取hash结果
        // folderPath -> [filePath -> (hashName -> hashValue)]
        public static Dictionary<string, RunLog> getHashResultDictFromRunLog(string run_log_path, List<string> all_hash_method_name_list)
        {
            List<string> all_line_list = new List<string>();
            if (!File.Exists(run_log_path))
            {
                return new Dictionary<string, RunLog>();
            }
            using (StreamReader stream_reader = new StreamReader(run_log_path!, UTF8_ENCODING))
            {
                while (!stream_reader.EndOfStream)
                {
                    string line = stream_reader.ReadLine()!;
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    all_line_list.Add(line);
                }
            }

            Dictionary<string, RunLog> result = new Dictionary<string, RunLog>();
            string? folderPath = null;
            string? filePath = null;
            long fileSize = -1;
            string? hashName = null;
            string? hashValue = null;
            foreach (string line in all_line_list)
            {
                // RUN LOG开始
                if (line.Equals(PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_START))
                {
                    folderPath = null;
                    filePath = null;
                    fileSize = -1;
                    hashName = null;
                    hashValue = null;

                    continue;
                }
                // RUN LOG 结束
                else if (line.Equals(PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_END))
                {
                    RunLog runLog = new RunLog(folderPath!, filePath!, fileSize, hashName!, hashValue!);
                    result[runLog.getRunLogHash()] = runLog;

                    continue;
                }
                // 每一项目
                else if (line.StartsWith("["))
                {
                    // [folder_path]
                    if (line.StartsWith(PROGRAM_LOG_CONTENT_FOLDER_PREFIX))
                    {
                        folderPath = line.Substring(PROGRAM_LOG_CONTENT_FOLDER_PREFIX.Length);
                        continue;
                    }
                    // [file_path]
                    else if (line.StartsWith(PROGRAM_LOG_CONTENT_FILE_PATH_PREFIX))
                    {
                        filePath = line.Substring(PROGRAM_LOG_CONTENT_FILE_PATH_PREFIX.Length);
                        continue;
                    }
                    // [size]
                    else if (line.StartsWith(PROGRAM_LOG_CONTENT_FILE_SIZE_PREFIX))
                    {
                        fileSize = long.Parse(line.Substring(PROGRAM_LOG_CONTENT_FILE_SIZE_PREFIX.Length));
                        continue;
                    }
                    // 其他情况，hash的key-value值
                    else
                    {
                        string[] line_split = line.Split(" ");
                        string hash_method_name = line_split[0].Replace("[", "").Replace("]", "");
                        foreach (string hash_method_name_item in all_hash_method_name_list)
                        {
                            if (hash_method_name_item.Equals(hash_method_name))
                            {
                                hashName = hash_method_name_item;
                                hashValue = line_split[1];

                                break;
                            }
                        }

                        continue;
                    }
                }
            }
            return result;
        }

        public static Stream getMainfestResourceStream(string fileName)
        {
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string item in resourceNames)
            {
                if (item.EndsWith(fileName))
                {
                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(item)!;
                }
            }
            throw new Exception($"内嵌文件[{fileName}]不存在");
        }

        public static FileInfo copyEmbeddedResourceToTempFolder(Stream stream, List<string> appendFolderName, string targetFileName)
        {
            List<string> toCombinePath =
            [
                AppDomain.CurrentDomain.BaseDirectory,
                .. appendFolderName,
                targetFileName,
            ];
            // 构造目标文件地址
            string path = Path.Combine(toCombinePath.ToArray());

            // 如果文件已经存在，则直接退出
            if (File.Exists(path))
            {
                return new FileInfo(path);
            }

            // 如果文件夹不存在则创建
            FileInfo fileInfo = new FileInfo(path);
            DirectoryInfo directoryInfo = fileInfo.Directory!;
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            using (Stream output = File.Create(path))
            {
                stream.CopyTo(output);
            }
            // 判断是否复制成功
            if (File.Exists(path))
            {
                return fileInfo;
            }
            else
            {
                throw new Exception($"复制内嵌文件失败{targetFileName}");
            }
        }
    }
}
