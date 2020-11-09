using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ComputeMD5_CMD_NetCore_v2
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> all_path = new List<string>();
            Console.WriteLine("请输入所有要计算的目录地址（以##结束）：");
            string input;
            while (!(input = Console.ReadLine()).Equals(@"##"))
                all_path.Add(input);
            Console.WriteLine("\n所有地址：");
            int no = 0;
            foreach (var path in all_path)
                Console.WriteLine((++no) + ". " + path);

            /*计算剩余时间 变量准备*/
            int all_file_num = 0;
            int handle_file_num = 0;
            double all_handle_time_sum = 0d;
            double handle_time_average = 0d;
            foreach (var path in all_path)
            {
                if (Directory.Exists(path))
                {
                    foreach (var path_file in new DirectoryInfo(path).GetFiles())
                    {
                        if (!path_file.FullName.Contains(@"hash.md5"))
                            ++all_file_num;
                    }
                }
            }
            /*计算剩余时间 变量准备*/

            DateTime before_all = DateTime.Now;
            Console.WriteLine("\n开始执行，开始时间：" + before_all.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            no = 0;
            foreach (var path in all_path)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine((++no) + "." + path + " 不存在，跳过！\n");
                    continue;
                }
                DateTime folder_before_time = DateTime.Now;
                StreamWriter sw = new StreamWriter(path + "\\hash.md5", false, new UTF8Encoding(false));
                Console.WriteLine((++no) + "." + path + "，开始时间：" + folder_before_time.ToString("yyyy-MM-dd HH:mm:ss"));
                FileInfo[] all_file = new DirectoryInfo(path).GetFiles();
                int file_no = 0;
                foreach (var file in all_file)
                {
                    if (file.FullName.Contains(@"hash.md5"))
                        continue;
                    DateTime file_before_time = DateTime.Now;
                    Console.WriteLine("    (" + (++file_no) + ")" + file.FullName);
                    string result = GetMd5.GetMd5HashValue(file);
                    DateTime file_after_time = DateTime.Now;
                    Console.WriteLine("      -result: " + result);
                    Console.WriteLine("      -开始时间：" + file_before_time.ToString("yyyy-MM-dd HH:mm:ss") + "，结束时间：" + file_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共同时：" + (file_after_time - file_before_time).TotalSeconds + " 秒");
                    all_handle_time_sum += (file_after_time - file_before_time).TotalSeconds;
                    ++handle_file_num;
                    handle_time_average = all_handle_time_sum / handle_file_num;
                    Console.WriteLine("      -预计剩余时间：" + handle_time_average * (all_file_num - handle_file_num) / 60d + " 分，" + handle_time_average * (all_file_num - handle_file_num) + " 秒");
                    sw.Write(result + " *" + file.Name + "\n");
                }
                sw.Flush();
                sw.Close();
                DateTime folder_after_time = DateTime.Now;
                Console.WriteLine("  结束时间：" + folder_after_time.ToString("yyyy-MM-dd HH:mm:ss") + "，总共用时：" + (folder_after_time - folder_before_time).TotalMinutes + " 分，" + (folder_after_time - folder_before_time).TotalSeconds + " 秒\n");
            }
            DateTime after_all = DateTime.Now;
            Console.WriteLine("\n\n结束执行，结束时间：" + after_all.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("总共用时：" + (after_all - before_all).TotalMinutes + " 分，" + (after_all - before_all).TotalSeconds + " 秒");

            while (true)
                Console.ReadLine();
        }
    }
}
