using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeMD5_MulitiThread
{
    class GlobalData
    {
        public static GlobalData globalData = new GlobalData();

        public ConcurrentQueue<FileInfo> allfile_queue = new ConcurrentQueue<FileInfo>();
        public List<FileInfo> current_file = new List<FileInfo>();
        public ConcurrentDictionary<string, string> md5_dic = new ConcurrentDictionary<string, string>();
    }
}
