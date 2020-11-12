using System;
using System.Collections.Generic;
using System.Text;

namespace ComputeHash
{
    class SettingStruct
    {
        public class Rootobject
        {
            public string[] compute_folder { get; set; }
            public string[] result_save_folder { get; set; }
            public string blake2_exe_path { get; set; }
            public string blake3_exe_path { get; set; }
            public Check_Method check_method { get; set; }
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
