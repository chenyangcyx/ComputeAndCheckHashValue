using System.Text;

namespace ConsoleApp4
{
    internal class RunLog
    {
        // 目录地址
        public string folderPath { get; set; }

        // 文件路径
        public string filePath { get; set; }

        // 文件大小
        public long fileSize { get; set; }

        // hash方法名称
        public string hashName { get; set; }

        // hash值
        public string hashValue { get; set; }

        // 构造方法
        public RunLog() { }
        public RunLog(string folderPath, FileInfo fileInfo, string hashName, string hashValue)
        {
            this.folderPath = folderPath;
            this.filePath = fileInfo.FullName;
            this.fileSize = fileInfo.Length;
            this.hashName = hashName;
            this.hashValue = hashValue;
        }
        public RunLog(string folderPath, string filePath, long fileSize, string hashName, string hashValue)
        {
            this.folderPath = folderPath;
            this.filePath = filePath;
            this.fileSize = fileSize;
            this.hashName = hashName;
            this.hashValue = hashValue;
        }

        // 获取数据的hash
        public string getRunLogHash()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(folderPath)
                .Append(filePath)
                .Append(fileSize)
                .Append(hashName);
            return ComputeHash.getSHA2_256(stringBuilder.ToString());
        }

        // 传入指定值，获取数据hash
        public static string getRunLogHash(string folderPath, FileInfo fileInfo, string hashName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(folderPath)
                .Append(fileInfo.FullName)
                .Append(fileInfo.Length)
                .Append(hashName);
            return ComputeHash.getSHA2_256(stringBuilder.ToString());
        }
    }
}
