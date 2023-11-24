namespace CheckHash3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> fileList1 = new List<string>
            {
                @"E:\重新上传\压缩\年度备份\2022年\2022年.part01.rar",
                @"E:\重新上传\压缩\年度备份\2022年\2022年.part01.rev",
                @"E:\重新上传\压缩\年度备份\2022年\2022年.part02.rar",
                @"E:\重新上传\压缩\年度备份\2022年\2022年.part02.rev"
            };

            fileList1.ForEach(file =>
            {
                long startTime= DateTime.Now.Ticks;
                MergedMemoryStream mergedMemoryStream = new MergedMemoryStream(file);
               
                string md5=ComputeHash.getMD5(mergedMemoryStream);
                string sha1=ComputeHash.getSHA1(mergedMemoryStream);
                string sha2256=ComputeHash.getSHA2_256(mergedMemoryStream);
                long endTime= DateTime.Now.Ticks;
                Console.WriteLine($"处理成功，文件【{file}】，大小【{mergedMemoryStream.Length}】，处理时间【{endTime-startTime}】");
                Console.WriteLine($"【MD5】{md5}\n【SHA1】{sha1}\n【SHA2-256】{sha2256}");
            });

            /*
            List<string> fileList2 = new List<string>
            {
                @"E:\重新上传\压缩\收集资源\IT资料\C++从初级到高级全套教程\C++从初级到高级全套教程.part01.rar",
                @"E:\重新上传\压缩\收集资源\IT资料\C++从初级到高级全套教程\C++从初级到高级全套教程.part01.rev",
                @"E:\重新上传\压缩\收集资源\IT资料\C++从初级到高级全套教程\C++从初级到高级全套教程.part02.rar",
                @"E:\重新上传\压缩\收集资源\IT资料\C++从初级到高级全套教程\C++从初级到高级全套教程.part02.rev"
            };
            fileList2.ForEach(file =>
            {
                long startTime = DateTime.Now.Ticks;
                FileInfo fileInfo = new FileInfo(file);
                ComputeHash.getMD5(fileInfo);
                ComputeHash.getSHA1(fileInfo);
                ComputeHash.getSHA2_256(fileInfo);
                long endTime = DateTime.Now.Ticks;
                Console.WriteLine($"处理成功，文件【{file}】，大小【{fileInfo.Length}】，处理时间【{endTime - startTime}】");
            });
            */
        }
    }
}