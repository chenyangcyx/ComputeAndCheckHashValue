﻿using System;
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
            // 检查blake程序是否存在
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
            // 检查hash_file_folder是否存在
            foreach (var path in setting.hash_file_folder)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"hash_file_folder: {path} 不存在，退出程序！");
                    while (true)
                        Console.ReadLine();
                }
            }
            // 设置temp目录
            Directory.CreateDirectory(setting.temp_folder);
            // 输出设置
            Console.WriteLine("所有设置：");
            Console.WriteLine("## use_rclone: " + (setting.use_rclone == 1));
            Console.WriteLine("## rclone_config_file: " + setting.rclone_config_file);
            Console.WriteLine("## copy_temp: " + (setting.copy_temp == 1));
            Console.WriteLine("## temp_folder: " + setting.temp_folder);
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
            Dictionary<string, List<RcloneFileList.FileInfo>> rclone_all_file_dic_list = new Dictionary<string, List<RcloneFileList.FileInfo>>();
            Dictionary<string, List<FileInfo>> local_all_file_dic_list = new Dictionary<string, List<FileInfo>>();
            Utilities.setAllFolderInfo(rclone_all_file_dic_list, local_all_file_dic_list, setting);
            if (setting.use_rclone == 1)
            {
                foreach (var info in rclone_all_file_dic_list.Values)
                {
                    foreach (var file in info)
                    {
                        if (file.IsDir == false)
                        {
                            all_file_num++;
                            all_file_byte += file.Size;
                        }
                    }
                }
            }
            else
            {
                foreach (var info in local_all_file_dic_list.Values)
                {
                    foreach (var file in info)
                    {
                        all_file_num++;
                        all_file_byte += file.Length;
                    }
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
                if (!Directory.Exists(path) && setting.use_rclone != 1)
                {
                    Console.WriteLine((++no) + "." + path + " 不存在，跳过！\n");
                    continue;
                }
                DateTime folder_before_time = DateTime.Now;
                Directory.CreateDirectory(new FileInfo(setting.result_output_file[path_no]).DirectoryName);
                StreamWriter sw_result = new StreamWriter(setting.result_output_file[path_no], false, Utilities.utf8_encoding);
                sw_result.AutoFlush = true;
                Console.WriteLine((++no) + "." + path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                sw_result.WriteLine((no) + "." + path);
                // 获取所有的文件信息
                List<string> all_file = Utilities.getAllFileInFolder(path, rclone_all_file_dic_list, local_all_file_dic_list, setting);
                int file_no = 0;

                // 导入该文件夹下的所有文件的hash值
                Dictionary<string, string>[] folder_hash_dic = new Dictionary<string, string>[hash_method_name.Count];
                for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                {
                    if (hash_method_use[hash_no])
                    {
                        folder_hash_dic[hash_no] = new Dictionary<string, string>();
                        string[] all_value = File.ReadAllLines(setting.hash_file_folder[path_no] + "hash." + hash_method_name[hash_no]);
                        foreach (string hash_line in all_value)
                        {
                            if (hash_line.Length != 0)
                                folder_hash_dic[hash_no].Add(hash_line.Substring(hash_line.IndexOf(" ") + 1, hash_line.Length - hash_line.IndexOf(" ") - 1), hash_line.Substring(0, hash_line.IndexOf(" ")));
                        }
                    }
                }

                // 用来记录错误的文件
                List<string> error_hash_check_file = new List<string>();
                // 开始逐个遍历文件
                foreach (var file in all_file)
                {
                    DateTime file_before_time = DateTime.Now;
                    Console.WriteLine("    (" + (++file_no) + ")" + file);
                    sw_result.WriteLine("    (" + (file_no) + ")" + file);
                    // 复制文件
                    FileInfo new_file;
                    if (setting.use_rclone == 1)
                    {
                        // 使用rclone，复制到temp目录
                        if (setting.copy_temp == 1)
                        {
                            new_file = new FileInfo(setting.temp_folder + file.Substring(file.LastIndexOf("/") + 1));
                            Console.WriteLine("      -复制文件 " + file + " -> " + new_file.FullName);
                            Utilities.copyFile(file, new_file.DirectoryName, setting.use_rclone == 1, setting.rclone_config_file);
                        }
                        // 使用rclone，不复制到temp目录
                        // 不允许该组合
                        else
                        {
                            Console.WriteLine("不支持的组合：use_rclone: True, copy_temp: False");
                            while (true)
                                Console.ReadLine();
                        }
                    }
                    else
                    {
                        // 本地校验，复制到temp目录
                        if (setting.copy_temp == 1)
                        {
                            FileInfo old_file = new FileInfo(file);
                            new_file = new FileInfo(setting.temp_folder + old_file.Name);
                            Console.WriteLine("      -复制文件 " + old_file.FullName + " -> " + new_file.FullName);
                            Utilities.copyFile(old_file.FullName, new_file.FullName, setting.use_rclone == 1, setting.rclone_config_file);
                        }
                        // 本地校验，不复制到temp目录
                        else
                        {
                            new_file = new FileInfo(file);
                        }
                    }

                    for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                    {
                        if (hash_method_use[hash_no])
                        {
                            // 先计算这个文件的hash值
                            string hash_compute_value = ComputeHash.getHashByName(hash_method_name[hash_no], new_file.FullName, setting);
                            Console.WriteLine("      -" + hash_method_name[hash_no] + "_hash: " + hash_compute_value);
                            sw_result.WriteLine("      -" + hash_method_name[hash_no] + "_hash: " + hash_compute_value);
                            // 从字典中寻找这个文件之前计算的hash值
                            string hash_dic_value = "";
                            folder_hash_dic[hash_no].TryGetValue(new_file.Name, out hash_dic_value);
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
                        handle_file_byte += new_file.Length;
                        handle_file_time_second += use_time_second;
                        double per_byte_average = handle_file_byte / handle_file_time_second;
                        double remain_second = (all_file_byte - handle_file_byte) / per_byte_average;
                        Console.WriteLine("      -剩余时间：" + (remain_second / 86400.0).ToString("0.0000000") + " 天 ≈≈ " + (remain_second / 3600.0).ToString("0.0000000") + " 小时 ≈≈ " + (remain_second / 60.0).ToString("0.0000000") + " 分 ≈≈ " + remain_second.ToString("0.0000000") + " 秒");
                    }
                    // 删除temp目录下的文件
                    if (setting.copy_temp == 1)
                    {
                        new_file.Delete();
                    }
                }
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共用时：" + (folder_after_time - folder_before_time).TotalDays.ToString("0.0000000") + " 天 ≈≈ " + (folder_after_time - folder_before_time).TotalHours.ToString("0.0000000") + " 小时 ≈≈ " + (folder_after_time - folder_before_time).TotalMinutes.ToString("0.0000000") + " 分 ≈≈ " + (folder_after_time - folder_before_time).TotalSeconds.ToString("0.0000000") + " 秒\n");

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
                        Console.WriteLine($"({(++fail_file_no)}): {error_file_info}");
                        sw_result.WriteLine($"({fail_file_no}): {error_file_info}");
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
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalDays.ToString("0.0000000") + " 天 ≈≈ " + (after_all - before_all).TotalHours.ToString("0.0000000") + " 小时 ≈≈ " + (after_all - before_all).TotalMinutes.ToString("0.0000000") + " 分 ≈≈ " + (after_all - before_all).TotalSeconds.ToString("0.0000000") + " 秒");

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
