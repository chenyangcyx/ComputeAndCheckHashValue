using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckHash2
{
    internal class SettingStruct
    {
        public class SettingConfig
        {
            public string check_folder_config_file { get; set; }
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
}
