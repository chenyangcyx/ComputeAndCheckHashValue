using ComputeMD5;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeMD5_MulitiThread
{
    class MD5ComputeThread
    {
        public void getOne()
        {
            GlobalData gd = GlobalData.globalData;
            FileInfo fi;
            if (!gd.allfile_queue.TryDequeue(out fi))
                return;
            lock (gd.current_file)
            {
                gd.current_file.Add(fi);
            }
            Console.Write("开始处理" + fi.FullName+"，md5值：");
            GetMd5 md = new GetMd5();
            string md5_value = md.GetMd5HashValue(fi);
            gd.md5_dic.TryAdd(fi.Name, md5_value);
            Console.WriteLine(md5_value);
            lock (gd.current_file)
            {
                gd.current_file.Remove(fi);
            }
        }
    }
}
