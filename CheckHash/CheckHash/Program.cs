using System;
using System.Collections.Generic;
using System.IO;

namespace CheckHash
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("没有输入配置文件！");
                while (true)
                    Console.ReadLine();
            }
            SettingStruct.Rootobject setting = Utilities.getSetting(args[0]);
            // 检查设置选项
            if (!File.Exists(setting.blake2_exe_path))
            {
                Console.WriteLine("BLAKE2程序" + setting.blake2_exe_path + "不存在！");
                while (true)
                    Console.ReadLine();
            }
            if (!File.Exists(setting.blake3_exe_path))
            {
                Console.WriteLine("BLAKE3程序" + setting.blake3_exe_path + "不存在！");
                while (true)
                    Console.ReadLine();
            }
            // 输出设置
            Console.WriteLine("所有设置：");
            Console.WriteLine("## check_folder");
            int no = 0;
            foreach (var path in setting.check_folder)
                Console.WriteLine((++no) + ". " + path);
            Console.WriteLine("## hash_file_folder");
            no = 0;
            foreach (var path in setting.hash_file_folder)
                Console.WriteLine((++no) + ". " + path);
            Console.WriteLine("## result_output_file");
            no = 0;
            foreach (var file_path in setting.result_output_file)
                Console.WriteLine((++no) + ". " + file_path);
            Console.WriteLine("## blake2_exe_path: " + setting.blake2_exe_path);
            Console.WriteLine("## blake3_exe_path: " + setting.blake3_exe_path);
            List<string> hash_method_name = new List<string>()
            {
                Utilities.MD5_NAME,
                Utilities.SHA1_NAME,
                Utilities.SHA256_NAME,
                Utilities.SHA384_NAME,
                Utilities.SHA512_NAME,
                Utilities.BLAKE2b_NAME,
                Utilities.BLAKE2s_NAME,
                Utilities.BLAKE2bp_NAME,
                Utilities.BLAKE2sp_NAME,
                Utilities.BLAKE3_NAME
            };
            List<bool> hash_method_use = new List<bool>();
            if (setting.check_method.md5 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.sha1 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.sha256 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.sha384 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.sha512 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.blake2b != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.blake2s != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.blake2bp != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.blake2sp != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            if (setting.check_method.blake3 != 0)
                hash_method_use.Add(true);
            else
                hash_method_use.Add(false);
            no = 0;
            Console.Write("## check_method: ");
            for (int i = 0; i < hash_method_name.Count; i++)
            {
                if (hash_method_use[i])
                    if (i == 0)
                        Console.Write(hash_method_name[i]);
                    else
                        Console.Write(", " + hash_method_name[i]);
            }
            Console.Write("\n");
            Console.WriteLine("## forecast_remain_time: " + (setting.forecast_remain_time == 1));

            DateTime before_all = DateTime.Now;
            Console.WriteLine("\n开始检查，开始时间：" + before_all.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            /*预测剩余时间*/
            int all_file_num = 0;
            long all_file_byte = 0L;
            double handle_file_time_second = 0d;
            int handle_file_num = 0;
            long handle_file_byte = 0L;
            // 统计所有文件个数及大小
            foreach (var dir in setting.check_folder)
            {
                if (!Directory.Exists(dir))
                    continue;
                foreach (var file in new DirectoryInfo(dir).GetFiles())
                {
                    all_file_num++;
                    all_file_byte += file.Length;
                }
            }
            // 开关，是否开启剩余时间预测
            bool forecast_remain_time = (setting.forecast_remain_time == 1);
            /*预测剩余时间*/

            List<string> error_check_folder = new List<string>();
            no = 0;
            for (int path_no = 0; path_no < setting.check_folder.Length; path_no++)
            {
                string path = setting.check_folder[path_no];
                if (!Directory.Exists(path))
                {
                    Console.WriteLine((++no) + "." + path + " 不存在，跳过！\n");
                    continue;
                }
                DateTime folder_before_time = DateTime.Now;
                StreamWriter sw_result = new StreamWriter(setting.result_output_file[path_no], false, Utilities.utf8_encoding);
                Console.WriteLine((++no) + "." + path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw_result.WriteLine((no) + "." + path);
                FileInfo[] all_file = new DirectoryInfo(path).GetFiles();
                int file_no = 0;

                // 导入该文件夹下的所有文件的hash值
                Dictionary<string, string>[] folder_hash_dic = new Dictionary<string, string>[hash_method_name.Count];
                for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                {
                    if (hash_method_use[hash_no])
                    {
                        folder_hash_dic[hash_no] = new Dictionary<string, string>();
                        string[] all_value = File.ReadAllLines(setting.hash_file_folder[path_no] + "hash." + hash_method_name[hash_no]);
                        foreach(string hash_line in all_value)
                        {
                            if (hash_line.Length != 0)
                                folder_hash_dic[hash_no].Add(hash_line.Split(" ")[1], hash_line.Split(" ")[0]);
                        }
                    }
                }

                // 用来记录错误的文件
                List<FileInfo> error_hash_check_file = new List<FileInfo>();
                foreach (var file in all_file)
                {
                    DateTime file_before_time = DateTime.Now;
                    Console.WriteLine("    (" + (++file_no) + ")" + file.FullName);
                    sw_result.WriteLine("    (" + (file_no) + ")" + file.FullName);
                    for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                    {
                        if (hash_method_use[hash_no])
                        {
                            // 先计算这个文件的hash值
                            string hash_compute_value = ComputeHash.getHashByName(hash_method_name[hash_no], file.FullName, setting);
                            Console.WriteLine("      -" + hash_method_name[hash_no] + "_hash: " + hash_compute_value);
                            sw_result.WriteLine("      -" + hash_method_name[hash_no] + "_hash: " + hash_compute_value);
                            // 从字典中寻找这个文件之前计算的hash值
                            string hash_dic_value = "";
                            folder_hash_dic[hash_no].TryGetValue(file.Name, out hash_dic_value);
                            Console.WriteLine("       " + hash_method_name[hash_no] + "_dict: " + hash_dic_value);
                            sw_result.WriteLine("       " + hash_method_name[hash_no] + "_dict: " + hash_dic_value);
                            // 输出比对结果
                            if (hash_compute_value.Equals(hash_dic_value))
                            {
                                // hash结果一致
                                Console.WriteLine("       result " + hash_method_name[hash_no] + ": OK");
                                sw_result.WriteLine("       result " + hash_method_name[hash_no] + ": OK");
                            }
                            else
                            {
                                // hash结果不一致
                                if (!error_hash_check_file.Contains(file))
                                    error_hash_check_file.Add(file);
                                if (!error_check_folder.Contains(path))
                                    error_check_folder.Add(path);
                                Console.WriteLine("       result " + hash_method_name[hash_no] + ": Fail");
                                sw_result.WriteLine("       result " + hash_method_name[hash_no] + ": Fail");
                            }
                        }
                    }
                    DateTime file_after_time = DateTime.Now;
                    double use_time_second = (file_after_time - file_before_time).TotalSeconds;
                    Console.WriteLine("      -开始时间：" + file_before_time.ToString("yyyy-MM-dd HH:mm:ss") + "，结束时间：" + file_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共同时：" + use_time_second.ToString("0.0000000") + " 秒");
                    if (forecast_remain_time)
                    {
                        handle_file_num++;
                        handle_file_byte += file.Length;
                        handle_file_time_second += use_time_second;
                        double per_byte_average = handle_file_byte / handle_file_time_second;
                        double remain_second = (all_file_byte - handle_file_byte) / per_byte_average;
                        Console.WriteLine("      -剩余时间：" + (remain_second / 60).ToString("0.0000000") + " 分，" + remain_second.ToString("0.0000000") + " 秒");
                    }
                }
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共用时：" + (folder_after_time - folder_before_time).TotalMinutes.ToString("0.0000000") + " 分，" + (folder_after_time - folder_before_time).TotalSeconds.ToString("0.0000000") + " 秒\n");

                // 输出总结信息
                if (error_hash_check_file.Count > 0)
                {
                    Console.WriteLine("## 报错信息 ##");
                    sw_result.WriteLine("\n## 报错信息 ##");
                    Console.WriteLine("error file num: " + error_hash_check_file.Count);
                    sw_result.WriteLine("error file num: " + error_hash_check_file.Count);
                    int fail_file_no = 0;
                    foreach (var error_file_info in error_hash_check_file)
                    {
                        Console.WriteLine($"({(++fail_file_no)}): {error_file_info.Name}");
                        sw_result.WriteLine($"({fail_file_no}): {error_file_info.Name}");
                    }
                }
                else
                {
                    Console.WriteLine("全部文件夹检验全部正确！");
                    sw_result.WriteLine("\n文件夹检验全部正确！");
                }
                sw_result.Flush();
                sw_result.Close();
            }
            DateTime after_all = DateTime.Now;
            Console.WriteLine("\n结束执行，结束时间：" + after_all.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalMinutes.ToString("0.0000000") + " 分，" + (after_all - before_all).TotalSeconds.ToString("0.0000000") + " 秒");

            if (error_check_folder.Count > 0)
            {
                Console.WriteLine("\n#### 出错文件夹 ####");
                no = 0;
                foreach (var folder_path in error_check_folder)
                    Console.WriteLine((++no) + ". " + folder_path);
            }
            else
            {
                Console.WriteLine("全部文件夹检验全部正确！");
            }
            Console.Write("\n\n\n\n");

            while (true)
                Console.ReadLine();
        }
    }
}
