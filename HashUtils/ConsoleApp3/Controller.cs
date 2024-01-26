using System.Reflection;
using System.Security.Cryptography;

namespace ConsoleApp3
{
    internal class Controller
    {
        public static void generateHash(SettingStruct.SettingConfig setting, List<string> check_folder_list, List<string> hash_method_name, List<bool> hash_method_use, bool use_run_log = true)
        {
            DateTime before_all = DateTime.Now;
            Console.WriteLine("\n开始生成，开始时间：" + before_all.ToString("yyyy-MM-dd HH:mm:ss") + "\n");

            /* 预测剩余时间 START */
            long all_file_num = 0;
            long all_file_byte = 0L;
            double handle_file_time_second = 0d;
            long handle_file_num = 0;
            long handle_file_byte = 0L;
            // 统计所有文件个数及大小
            Dictionary<string, List<FileInfo>> local_all_file_dic_list = new Dictionary<string, List<FileInfo>>();
            Utilities.setAllFolderInfo(local_all_file_dic_list, check_folder_list);
            foreach (var info in local_all_file_dic_list.Values)
            {
                foreach (var file in info)
                {
                    all_file_num++;
                    all_file_byte += file.Length;
                }
            }
            /* 预测剩余时间 END */

            /* 如果存在run_log文件，则读取里面的所有内容 */
            Dictionary<string, RunLog> exist_run_log_dic = new Dictionary<string, RunLog>();
            if (use_run_log)
            {
                exist_run_log_dic = Utilities.getHashResultDictFromRunLog(Utilities.PROGRAM_RUNNING_LOG_FILE_PATH!, hash_method_name);
            }
            if (exist_run_log_dic.Count == 0)
            {
                Console.WriteLine($"\n**未检测到程序运行log文件{Utilities.PROGRAM_LOG_FILE_NAME}\n");
            }
            else
            {
                Console.WriteLine($"\n**检测到程序运行log文件{Utilities.PROGRAM_LOG_FILE_NAME}，共有{exist_run_log_dic.Count}条记录，文件地址：{Utilities.PROGRAM_RUNNING_LOG_FILE_PATH}\n");
            }
            /* 如果存在run_log文件，则读取里面的所有内容 */

            /* 临时目录写入stream初始化 */
            StreamWriter? programTempFolderRunningLogFile = openStream(Utilities.PROGRAM_RUNNING_LOG_FILE_PATH!, true);
            outputAndFlushStream(programTempFolderRunningLogFile!, $"\n\n开始运行程序：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n\n");
            /* 临时目录写入stream初始化 */

            List<string> error_check_folder = new List<string>();
            int no = 0;
            for (int path_no = 0; path_no < check_folder_list.Count; path_no++)
            {
                string this_folder_path = check_folder_list[path_no];
                if (!Directory.Exists(this_folder_path))
                {
                    Console.WriteLine((++no) + "." + this_folder_path + " 不存在，跳过！\n");
                    error_check_folder.Add(this_folder_path);
                    continue;
                }
                DateTime folder_before_time = DateTime.Now;
                Console.WriteLine((++no) + "." + this_folder_path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                // 获取所有的文件信息
                List<string> all_file = Utilities.getAllFileInFolder(this_folder_path, local_all_file_dic_list, setting);
                List<FileInfo> all_file_no_hash_file = new List<FileInfo>();
                int file_no = 0;

                // 开始逐个遍历文件
                foreach (var file_item in all_file)
                {
                    DateTime file_before_time = DateTime.Now;
                    FileInfo file = new FileInfo(file_item);
                    // 排除hash.txt文件
                    if (file.Name.Equals(Utilities.HASH_FILE_NAME))
                    {
                        continue;
                    }
                    else
                    {
                        all_file_no_hash_file.Add(file);
                    }
                    Console.WriteLine("    (" + (++file_no) + ")" + file_item);

                    // 获取各个hash方法的结果
                    Dictionary<string, string> hash_result = new Dictionary<string, string>();
                    for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                    {
                        if (hash_method_use[hash_no])
                        {
                            // 判断在run_log.txt文件中是否已经存储了结果
                            if (exist_run_log_dic.ContainsKey(RunLog.getRunLogHash(this_folder_path, file, hash_method_name[hash_no])))
                            {
                                Console.WriteLine($"      -{hash_method_name[hash_no]}: 已经计算过，跳过不再重复计算");
                                continue;
                            }
                            // 计算文件的hash值
                            string hash_compute_value = ComputeHash.getHashByName(hash_method_name[hash_no], file.FullName, setting);
                            // runLog运行临时文件输出
                            RunLog runLog = new RunLog(check_folder_list[path_no], file, hash_method_name[hash_no], hash_compute_value);
                            outputAndFlushStream(programTempFolderRunningLogFile, runLog);
                            // 屏幕输出
                            Console.WriteLine("      -" + hash_method_name[hash_no] + ": " + hash_compute_value);
                            hash_result.Add(hash_method_name[hash_no], hash_compute_value);
                        }
                    }

                    DateTime file_after_time = DateTime.Now;
                    double use_time_second = (file_after_time - file_before_time).TotalSeconds;
                    Console.WriteLine("      -开始时间：" + file_before_time.ToString("yyyy-MM-dd HH:mm:ss") + "，结束时间：" + file_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共同时：" + use_time_second.ToString("0.0000") + " 秒 ≈≈ " + (use_time_second / 60.0).ToString("0.0000") + " 分");

                    /* 预估剩余时间 START */
                    handle_file_num++;
                    handle_file_byte += file.Length;
                    handle_file_time_second += use_time_second;
                    double per_byte_average = handle_file_byte / handle_file_time_second;
                    double remain_second = (all_file_byte - handle_file_byte) / per_byte_average;
                    Console.WriteLine($"      -当前进度：{handle_file_num} / {all_file_num} = {(handle_file_num * 100.0d / all_file_num).ToString("0.00")}%，当前文件：{handle_file_num}，总共文件：{all_file_num}");
                    Console.WriteLine("      -剩余时间：" + (remain_second / 86400.0).ToString("0.0000") + " 天 ≈≈ " + (remain_second / 3600.0).ToString("0.0000") + " 小时 ≈≈ " + (remain_second / 60.0).ToString("0.0000") + " 分 ≈≈ " + remain_second.ToString("0.0000") + " 秒");
                    Console.WriteLine("      -预计结束时间：" + DateTime.Now.AddSeconds(remain_second).ToString("yyyy-MM-dd HH:mm:ss"));
                    /* 预估剩余时间 END */
                }
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss"));
                Console.WriteLine("  总共用时：" + (folder_after_time - folder_before_time).TotalDays.ToString("0.0000") + " 天 ≈≈ " + (folder_after_time - folder_before_time).TotalHours.ToString("0.0000") + " 小时 ≈≈ " + (folder_after_time - folder_before_time).TotalMinutes.ToString("0.0000") + " 分 ≈≈ " + (folder_after_time - folder_before_time).TotalSeconds.ToString("0.0000") + " 秒\n");

                // 读取程序运行RUN_LOG文件中的所有记录
                outputAndFlushStream(programTempFolderRunningLogFile!, null, true);
                Dictionary<string, RunLog> all_run_log_dic = Utilities.getHashResultDictFromRunLog(Utilities.PROGRAM_RUNNING_LOG_FILE_PATH!, hash_method_name);
                programTempFolderRunningLogFile = openStream(Utilities.PROGRAM_RUNNING_LOG_FILE_PATH!, false);

                // 将RUN LOG中的记录全部写入到hash.txt文件中
                using (StreamWriter sw_result = new StreamWriter(Path.Combine(check_folder_list[path_no], Utilities.HASH_FILE_NAME), false, Utilities.UTF8_ENCODING))
                {
                    for (int file_info_no = 0; file_info_no < all_file_no_hash_file.Count; file_info_no++)
                    {
                        FileInfo fileInfo = all_file_no_hash_file[file_info_no];
                        if (file_info_no > 0)
                        {
                            sw_result.WriteLine(Utilities.HASH_FILE_SPLIT_LINE_CONTENT);
                        }
                        sw_result.WriteLine(Utilities.HASH_FILE_CONTENT_NAME_PREFIX + fileInfo.Name);
                        for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                        {
                            if (hash_method_use[hash_no])
                            {
                                string run_log_dic_key = RunLog.getRunLogHash(this_folder_path, fileInfo, hash_method_name[hash_no]);
                                if (!all_run_log_dic.ContainsKey(run_log_dic_key))
                                {
                                    throw new Exception($"找不到对应的hash记录，folderPath={this_folder_path}, filePath={fileInfo.FullName}, fileSize={fileInfo.Length}, hashName={hash_method_name[hash_no]}");
                                }
                                RunLog runLog = all_run_log_dic[run_log_dic_key];
                                // 将结果写入到hash.txt文件
                                sw_result.WriteLine("[" + runLog.hashName + "] " + runLog.hashValue);
                            }
                        }
                    }
                }
            }
            DateTime after_all = DateTime.Now;
            Console.WriteLine("\n结束执行，结束时间：" + after_all.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalDays.ToString("0.0000") + " 天 ≈≈ " + (after_all - before_all).TotalHours.ToString("0.0000") + " 小时 ≈≈ " + (after_all - before_all).TotalMinutes.ToString("0.0000") + " 分 ≈≈ " + (after_all - before_all).TotalSeconds.ToString("0.0000") + " 秒");

            if (error_check_folder.Count > 0)
            {
                Console.WriteLine("\n#### 出错文件夹 ####");
                no = 0;
                foreach (var folder_path in error_check_folder)
                    Console.WriteLine((++no) + ". " + folder_path);
            }
            else
            {
                Console.WriteLine("全部文件夹全部生成正确！");
            }
            Console.Write("\n\n");

            // 关闭log文件的写入
            outputAndFlushStream(programTempFolderRunningLogFile, $"\n\n结束运行程序：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n\n", true);
        }

        public static void checkHash(SettingStruct.SettingConfig setting, List<string> check_folder_list, List<string> hash_method_name, List<bool> verify_method_use)
        {
            DateTime before_all = DateTime.Now;
            Console.WriteLine("\n开始校验，开始时间：" + before_all.ToString("yyyy-MM-dd HH:mm:ss") + "\n");

            /* 预测剩余时间 START */
            long all_file_num = 0;
            long all_file_byte = 0L;
            double handle_file_time_second = 0d;
            long handle_file_num = 0;
            long handle_file_byte = 0L;
            // 统计所有文件个数及大小
            Dictionary<string, List<FileInfo>> local_all_file_dic_list = new Dictionary<string, List<FileInfo>>();
            Utilities.setAllFolderInfo(local_all_file_dic_list, check_folder_list);
            foreach (var info in local_all_file_dic_list.Values)
            {
                foreach (var file in info)
                {
                    all_file_num++;
                    all_file_byte += file.Length;
                }
            }
            /* 预测剩余时间 END */

            // 开始流程
            VerifyErrorDetailHandler error_check_file = new VerifyErrorDetailHandler();
            VerifyErrorDetailHandler error_check_folder = new VerifyErrorDetailHandler();
            int no = 0;
            for (int path_no = 0; path_no < check_folder_list.Count; path_no++)
            {
                string path = check_folder_list[path_no];
                if (!Directory.Exists(path))
                {
                    Console.WriteLine((++no) + "." + path + " 不存在，跳过！\n");
                    error_check_folder.addVerifyErrorDetail(new VerifyErrorDetail(path, "目录路径不存在，跳过"));
                    continue;
                }
                string hash_file_path = Path.Combine(path, Utilities.HASH_FILE_NAME);
                if (!File.Exists(hash_file_path))
                {
                    Console.WriteLine((++no) + "." + hash_file_path + " 不存在，跳过！\n");
                    error_check_folder.addVerifyErrorDetail(new VerifyErrorDetail(path, "hash文件不存在，跳过"));
                    continue;
                }
                // 读入文件夹内的hash.txt文件
                List<string> hash_file_all_line_list = new List<string>();
                using (StreamReader stream_reader = new StreamReader(hash_file_path, Utilities.UTF8_ENCODING))
                {
                    while (!stream_reader.EndOfStream)
                    {
                        string line = stream_reader.ReadLine()!;
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                        hash_file_all_line_list.Add(line);
                    }
                }
                // 获取该文件保存的hash值
                // 对应结构：文件名->(hash方法->hash值)
                Dictionary<string, Dictionary<string, string>> hash_file_dic = Utilities.getHashResultDictFromHashFile(hash_file_all_line_list);
                // 开始校验流程
                DateTime folder_before_time = DateTime.Now;
                Console.WriteLine((++no) + "." + path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                // 获取所有的文件信息
                List<string> all_file = Utilities.getAllFileInFolder(path, local_all_file_dic_list, setting);
                int file_no = 0;
                // 开始逐个遍历文件
                foreach (var file_item in all_file)
                {
                    DateTime file_before_time = DateTime.Now;
                    FileInfo file = new FileInfo(file_item);
                    // 排除hash.txt文件
                    if (file.Name.Equals(Utilities.HASH_FILE_NAME))
                    {
                        continue;
                    }
                    Console.WriteLine("    (" + (++file_no) + ")" + file_item);

                    // 获取已有hash中文件的值dict
                    var _file_hash_dict = hash_file_dic.GetValueOrDefault(file.Name);
                    if (_file_hash_dict == null || _file_hash_dict.Count == 0)
                    {
                        Console.WriteLine($"{file_item} 没有对应的hash记录！");
                        error_check_file.addVerifyErrorDetail(new VerifyErrorDetail(file_item, "没有对应的hash记录"));
                        error_check_folder.addVerifyErrorDetail(new VerifyErrorDetail(path, $"{file.Name} 没有对应的hash记录"));
                        continue;
                    }
                    // 获取各个hash方法的结果
                    for (int hash_name_index = 0; hash_name_index < hash_method_name.Count; hash_name_index++)
                    {
                        string item_hash_method_name = hash_method_name[hash_name_index];

                        if (!verify_method_use[hash_name_index])
                        {
                            continue;
                        }
                        if (verify_method_use[hash_name_index] && !_file_hash_dict.ContainsKey(hash_method_name[hash_name_index]))
                        {
                            Console.WriteLine("      -" + item_hash_method_name + " 没有记录hash值！");
                            error_check_file.addVerifyErrorDetail(new VerifyErrorDetail(file_item, $"没有记录[{item_hash_method_name}]的hash值"));
                            error_check_folder.addVerifyErrorDetail(new VerifyErrorDetail(path, $"{file.Name} 没有记录[{item_hash_method_name}]的hash值"));
                            continue;
                        }

                        var item_hash_method_value = _file_hash_dict[item_hash_method_name];
                        string item_hash_compute_value = ComputeHash.getHashByName(item_hash_method_name, file.FullName, setting);
                        // 输出对比结果
                        if (item_hash_method_value.Equals(item_hash_compute_value))
                        {
                            Console.WriteLine("      -" + item_hash_method_name + " 期望[OK]: " + item_hash_method_value);
                            Console.WriteLine("      -" + item_hash_method_name + " 计算[OK]: " + item_hash_compute_value);
                        }
                        else
                        {
                            Console.WriteLine("      -" + item_hash_method_name + " 期望[FAIL]: " + item_hash_method_value);
                            Console.WriteLine("      -" + item_hash_method_name + " 计算[FAIL]: " + item_hash_compute_value);
                            error_check_file.addVerifyErrorDetail(new VerifyErrorDetail(file_item, $"[{item_hash_method_name}]的hash值不正确"));
                            error_check_folder.addVerifyErrorDetail(new VerifyErrorDetail(path, $"{file.Name} [{item_hash_method_name}]的hash值不正确"));
                        }
                    }

                    DateTime file_after_time = DateTime.Now;
                    double use_time_second = (file_after_time - file_before_time).TotalSeconds;
                    Console.WriteLine("      -开始时间：" + file_before_time.ToString("yyyy-MM-dd HH:mm:ss") + "，结束时间：" + file_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共同时：" + use_time_second.ToString("0.0000") + " 秒 ≈≈ " + (use_time_second / 60.0).ToString("0.0000") + " 分");

                    /* 预估剩余时间 START */
                    handle_file_num++;
                    handle_file_byte += file.Length;
                    handle_file_time_second += use_time_second;
                    double per_byte_average = handle_file_byte / handle_file_time_second;
                    double remain_second = (all_file_byte - handle_file_byte) / per_byte_average;
                    Console.WriteLine($"      -当前进度：{handle_file_num} / {all_file_num} = {(handle_file_num * 100.0d / all_file_num).ToString("0.00")}%，当前文件：{handle_file_num}，总共文件：{all_file_num}");
                    Console.WriteLine("      -剩余时间：" + (remain_second / 86400.0).ToString("0.0000") + " 天 ≈≈ " + (remain_second / 3600.0).ToString("0.0000") + " 小时 ≈≈ " + (remain_second / 60.0).ToString("0.0000") + " 分 ≈≈ " + remain_second.ToString("0.0000") + " 秒");
                    Console.WriteLine("      -预计结束时间：" + DateTime.Now.AddSeconds(remain_second).ToString("yyyy-MM-dd HH:mm:ss"));
                    /* 预估剩余时间 END */
                }
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss"));
                Console.WriteLine("  总共用时：" + (folder_after_time - folder_before_time).TotalDays.ToString("0.0000") + " 天 ≈≈ " + (folder_after_time - folder_before_time).TotalHours.ToString("0.0000") + " 小时 ≈≈ " + (folder_after_time - folder_before_time).TotalMinutes.ToString("0.0000") + " 分 ≈≈ " + (folder_after_time - folder_before_time).TotalSeconds.ToString("0.0000") + " 秒\n");
            }
            DateTime after_all = DateTime.Now;
            Console.WriteLine("\n结束执行，结束时间：" + after_all.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalDays.ToString("0.0000") + " 天 ≈≈ " + (after_all - before_all).TotalHours.ToString("0.0000") + " 小时 ≈≈ " + (after_all - before_all).TotalMinutes.ToString("0.0000") + " 分 ≈≈ " + (after_all - before_all).TotalSeconds.ToString("0.0000") + " 秒");

            if (error_check_file.Count > 0)
            {
                Console.WriteLine("\n\n#### 出错文件 ####");
                no = 0;
                foreach (var file_item in error_check_file.errorItemNameList)
                {
                    Console.WriteLine((++no) + ". " + file_item);
                    foreach (var reason_item in error_check_file.errorItemDetailListDict[file_item])
                    {
                        Console.WriteLine("  -" + reason_item);
                    }
                }
            }
            else
            {
                Console.WriteLine("\n\n全部文件全部校验正确！");
            }
            Console.Write("\n\n");

            if (error_check_folder.Count > 0)
            {
                Console.WriteLine("#### 出错文件夹 ####");
                no = 0;
                foreach (var folder_item in error_check_folder.errorItemNameList)
                {
                    Console.WriteLine((++no) + ". " + folder_item);
                    foreach (var reason_item in error_check_folder.errorItemDetailListDict[folder_item])
                    {
                        Console.WriteLine("  -" + reason_item);
                    }
                }
            }
            else
            {
                Console.WriteLine("全部文件夹全部校验正确！");
            }
            Console.Write("\n\n");
        }

        public static void checkSystemInfo()
        {
            Console.WriteLine("[os version] " + System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            Console.WriteLine("[os runtime] " + System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier);
            Console.WriteLine("[.NET version] " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
            Console.WriteLine("[Process Architecture] " + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture);
            Console.WriteLine("[OS Architecture] " + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture);

            Console.WriteLine();
            Console.WriteLine("[.NET SHA3_256] " + (SHA3_256.IsSupported ? "√√ Supported" : "×× Unsupported"));
            Console.WriteLine("[.NET SHA3_384] " + (SHA3_384.IsSupported ? "√√ Supported" : "×× Unsupported"));
            Console.WriteLine("[.NET SHA3_512] " + (SHA3_512.IsSupported ? "√√ Supported" : "×× Unsupported"));
            Console.WriteLine("[.NET Shake128] " + (Shake128.IsSupported ? "√√ Supported" : "×× Unsupported"));
            Console.WriteLine("[.NET Shake256] " + (Shake256.IsSupported ? "√√ Supported" : "×× Unsupported"));

            Console.WriteLine();
            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            foreach (string resourceName in resourceNames)
            {
                Console.WriteLine("[Mainfest Resource] " + resourceName);
            }
        }

        public static void showAndOutputSettingDemo()
        {
            // 屏幕输出文件
            using (StreamReader reader = new StreamReader(Utilities.getMainfestResourceStream(Utilities.EMBEDDED_RESOURCE_NAME_SETTING_DEMO_JSON)))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine(content);
            }

            // 复制文件到指定的目录下
            using (Stream stream = Utilities.getMainfestResourceStream(Utilities.EMBEDDED_RESOURCE_NAME_SETTING_DEMO_JSON))
            {
                FileInfo result = Utilities.copyEmbeddedResourceToTempFolder(stream, new List<string>(), Utilities.EMBEDDED_RESOURCE_NAME_SETTING_TARGET_JSON.Replace("{}", string.Format("{0:yyyyMMddHHmmss}", DateTime.Now)));
                if (result.Exists)
                {
                    Console.WriteLine($"\n文件复制成功，地址：{result.FullName}");
                }
            }
        }

        private static StreamWriter openStream(string path, bool first_time = false)
        {
            StreamWriter result = new StreamWriter(path!, true, Utilities.UTF8_ENCODING);
            result.AutoFlush = true;

            return result;
        }

        private static void outputAndFlushStream(StreamWriter writer, string content, bool closeStream = false)
        {
            writer.WriteLine(content);
            writer.Flush();
            if (closeStream)
            {
                writer.Flush();
                writer.Close();
            }
        }

        private static void outputAndFlushStream(StreamWriter writer, RunLog runLog)
        {
            writer.WriteLine();

            writer.WriteLine(Utilities.PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_START);
            writer.WriteLine($"{Utilities.PROGRAM_LOG_CONTENT_FOLDER_PREFIX}{runLog.folderPath}");
            writer.WriteLine($"{Utilities.PROGRAM_LOG_CONTENT_FILE_PATH_PREFIX}{runLog.filePath}");
            writer.WriteLine($"{Utilities.PROGRAM_LOG_CONTENT_FILE_SIZE_PREFIX}{runLog.fileSize}");
            writer.WriteLine($"[{runLog.hashName}] {runLog.hashValue}");
            writer.WriteLine(Utilities.PROGRAM_LOG_CONTENT_SPLIT_LINE_CONTENT_END);

            writer.WriteLine();
            writer.Flush();
        }
    }

    internal class VerifyErrorDetail
    {
        public VerifyErrorDetail(string errorItem, string message)
        {
            this.errorItem = errorItem;
            this.errorMessage = message;
        }

        public string errorItem { get; set; }
        public string errorMessage { get; set; }
    }

    internal class VerifyErrorDetailHandler
    {
        public List<VerifyErrorDetail> verifyErrorDetailList { get; }
        public List<string> errorItemNameList { get; }
        public Dictionary<string, List<string>> errorItemDetailListDict { get; }

        public int Count { get; set; }

        public VerifyErrorDetailHandler()
        {
            verifyErrorDetailList = new List<VerifyErrorDetail>();
            errorItemNameList = new List<string>();
            errorItemDetailListDict = new Dictionary<string, List<string>>();
            Count = 0;
        }

        public void addVerifyErrorDetail(VerifyErrorDetail detail)
        {
            // 加入verifyErrorDetailList
            verifyErrorDetailList.Add(detail);
            // 加入errorItemNameList
            if (!errorItemNameList.Contains(detail.errorItem))
            {
                errorItemNameList.Add(detail.errorItem);
            }
            // 加入errorItemDetailListDict
            if (!errorItemDetailListDict.ContainsKey(detail.errorItem))
            {
                // 首次初始化
                List<string> list = new List<string>
                {
                    detail.errorMessage
                };
                errorItemDetailListDict[detail.errorItem] = list;
            }
            else
            {
                // 已经存在
                errorItemDetailListDict[detail.errorItem].Add(detail.errorMessage);
            }
            // 设置Count的值
            Count++;
        }
    }
}
