namespace CheckHash
{
    class SettingStruct
    {
        public class Rootobject
        {
            public int use_rclone { get; set; }
            public string rclone_config_file { get; set; }
            public int copy_temp { get; set; }
            public string temp_folder { get; set; }
            public string[] check_folder { get; set; }
            public string[] hash_file_folder { get; set; }
            public string[] result_output_file { get; set; }
            public string blake2_exe_path { get; set; }
            public string blake3_exe_path { get; set; }
            public Check_Method check_method { get; set; }
            public int forecast_remain_time { get; set; }
        }

        public class Check_Method
        {
            public int md5 { get; set; }
            public int sha1 { get; set; }
            public int sha256 { get; set; }
            public int sha384 { get; set; }
            public int sha512 { get; set; }
            public int blake2b { get; set; }
            public int blake2bp { get; set; }
            public int blake2s { get; set; }
            public int blake2sp { get; set; }
            public int blake3 { get; set; }
        }
    }

    class RcloneFileList
    {
        public class FileInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public long Size { get; set; }
            public string MimeType { get; set; }
            public string ModTime { get; set; }
            public bool IsDir { get; set; }
            public string ID { get; set; }
        }
    }
}
