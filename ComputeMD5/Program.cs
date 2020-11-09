using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputeMD5
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
            Console.WriteLine("所有地址：");
            int no = 0;
            foreach (var path in all_path)
                Console.WriteLine((++no) + ". " + path);

            Console.WriteLine("\n开始执行：");
            no = 0;
            GetMd5 getMd5 = new GetMd5();
            foreach(var path in all_path)
            {
                StreamWriter sw = new StreamWriter(path + "\\hash.md5", false, new UTF8Encoding(false));
                Console.WriteLine((++no) + ". " + path);
                FileInfo[] all_file = new DirectoryInfo(path).GetFiles();
                foreach (var file in all_file)
                {
                    if (file.FullName.Contains(@"hash.md5"))
                        continue;
                    Console.Write("计算" + file.FullName + "的MD5值：");
                    string result = getMd5.GetMd5HashValue(file);
                    Console.WriteLine(result);
                    sw.Write(result + " *" + file.Name + "\n");
                }
                sw.Flush();
                sw.Close();
                Console.WriteLine();
            }
            Console.WriteLine("\n\n\nDone!");
            while (true)
                Console.ReadLine();
        }
    }
}
