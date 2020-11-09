using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputeMD5_MulitiThread
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\音乐";
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] all_file = directoryInfo.GetFiles();
            GlobalData gd = GlobalData.globalData;
            foreach (var file in all_file)
                gd.allfile_queue.Enqueue(file);

            DateTime start_time = DateTime.Now;
            while (!gd.allfile_queue.IsEmpty)
            {
                Thread.Sleep(10);
                lock (gd.current_file)
                {
                    if (gd.current_file.Count < 4)
                        new Thread(new ThreadStart(new MD5ComputeThread().getOne)).Start();
                }
            }
            while(!gd.allfile_queue.IsEmpty && ! (gd.current_file.Count == 0))
            {
                Thread.Sleep(10);
            }
            DateTime end_time = DateTime.Now;
            Console.WriteLine("\n\nAll Done!  用时：" + (end_time - start_time).TotalSeconds + "s");
        }
    }
}
