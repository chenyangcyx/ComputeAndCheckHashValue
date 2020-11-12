using System;
using System.Collections.Generic;
using System.IO;

namespace ComputeHash
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
            // 赋予权限
            Utilities.chmodBLAKEprogram(setting.blake2_exe_path);
            Utilities.chmodBLAKEprogram(setting.blake3_exe_path);
            // 输出设置
            Console.WriteLine("所有设置：");
            Console.WriteLine("## compute_folder");
            int no = 0;
            foreach (var path in setting.compute_folder)
                Console.WriteLine((++no) + ". " + path);
            Console.WriteLine("## result_save_folder");
            no = 0;
            foreach (var path in setting.result_save_folder)
                Console.WriteLine((++no) + ". " + path);
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
            Console.Write("## hash method: ");
            for (int i = 0; i < hash_method_name.Count; i++)
            {
                if (hash_method_use[i])
                    if (i == 0)
                        Console.Write(hash_method_name[i]);
                    else
                        Console.Write(", " + hash_method_name[i]);
            }
            Console.Write("\n");
            Console.WriteLine("## refresh_before_compute: " + (setting.refresh_before_compute == 1));
            Console.WriteLine("## forecast_remain_time: " + (setting.forecast_remain_time == 1));

            DateTime before_all = DateTime.Now;
            Console.WriteLine("\n开始执行，开始时间：" + before_all.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            /*预测剩余时间*/
            int all_file_num = 0;
            long all_file_byte = 0L;
            double handle_file_time_second = 0d;
            int handle_file_num = 0;
            long handle_file_byte = 0L;
            double per_byte_average = 0d;
            // 统计所有文件个数及大小
            foreach (var dir in setting.compute_folder)
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
            no = 0;
            for (int path_no = 0; path_no < setting.compute_folder.Length; path_no++)
            {
                string path = setting.compute_folder[path_no];
                if (!Directory.Exists(path))
                {
                    Console.WriteLine((++no) + "." + path + " 不存在，跳过！\n");
                    continue;
                }
                DateTime folder_before_time = DateTime.Now;
                Console.WriteLine((++no) + "." + path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                if (setting.refresh_before_compute == 1)
                {
                    Utilities.deleteFolder_CMD(setting.result_save_folder[path_no]);
                    Utilities.createFolder_CMD(setting.result_save_folder[path_no]);
                }
                FileInfo[] all_file = new DirectoryInfo(path).GetFiles();
                int file_no = 0;
                StreamWriter[] sw_all = new StreamWriter[hash_method_name.Count];
                for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                {
                    if (hash_method_use[hash_no])
                    {
                        sw_all[hash_no] = new StreamWriter(setting.result_save_folder[path_no] + "hash." + hash_method_name[hash_no], false, Utilities.utf8_encoding);
                    }
                }
                foreach (var file in all_file)
                {
                    if (file.FullName.Contains(@"hash.md5"))
                        continue;
                    DateTime file_before_time = DateTime.Now;
                    Console.WriteLine("    (" + (++file_no) + ")" + file.FullName);
                    for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                    {
                        if (hash_method_use[hash_no])
                        {
                            string hash_value = ComputeHash.getHashByName(hash_method_name[hash_no], file.FullName, setting);
                            sw_all[hash_no].Write(hash_value + " " + file.Name + "\n");
                            Console.WriteLine("      -result " + hash_method_name[hash_no] + ": " + hash_value);
                        }
                    }
                    DateTime file_after_time = DateTime.Now;
                    double use_time_second = (file_after_time - file_before_time).TotalSeconds;
                    Console.WriteLine("      -开始时间：" + file_before_time.ToString("yyyy-MM-dd HH:mm:ss") + "，结束时间：" + file_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共同时：" + use_time_second + " 秒");
                    if (forecast_remain_time)
                    {
                        handle_file_num++;
                        handle_file_byte += file.Length;
                        handle_file_time_second += use_time_second;
                        per_byte_average = handle_file_byte / handle_file_time_second;
                        double remain_second = (all_file_byte - handle_file_byte) / per_byte_average;
                        Console.WriteLine("      -剩余时间：" + remain_second / 60 + " 分，" + remain_second + " 秒");
                    }
                }
                for (int hash_no = 0; hash_no < hash_method_name.Count; hash_no++)
                {
                    if (hash_method_use[hash_no])
                    {
                        sw_all[hash_no].Flush();
                        sw_all[hash_no].Close();
                        sw_all[hash_no].Dispose();
                    }
                }
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共用时：" + (folder_after_time - folder_before_time).TotalMinutes + " 分，" + (folder_after_time - folder_before_time).TotalSeconds + " 秒\n");
            }
            DateTime after_all = DateTime.Now;
            Console.WriteLine("\n结束执行，结束时间：" + after_all.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalMinutes + " 分，" + (after_all - before_all).TotalSeconds + " 秒");

            while (true)
                Console.ReadLine();
        }
    }
}
