namespace ConsoleApp3
{
    internal class RunLog
    {
        // 目录地址
        public string folderPath { get; set; }

        // 文件FileInfo
        public FileInfo fileInfo { get; set; }

        // hash方法名称
        public string hashName { get; set; }

        // hash值
        public string hashValue { get; set; }

        // 构造方法
        public RunLog() { }
        public RunLog(string folderPath, FileInfo fileInfo, string hashName, string hashValue)
        {
            this.folderPath = folderPath;
            this.fileInfo = fileInfo;
            this.hashName = hashName;
            this.hashValue = hashValue;
        }
        public RunLog(string folderPath, string filePath, string hashName, string hashValue)
        {
            this.folderPath = folderPath;
            this.fileInfo = new FileInfo(filePath);
            this.hashName = hashName;
            this.hashValue = hashValue;
        }
    }
}
