using System.Text.Json.Serialization;

namespace ConsoleApp4
{
    internal class SettingStruct
    {
        public class SettingConfig
        {
            [JsonPropertyName("check_folder_config_file")]
            public required string check_folder_config_file { get; set; }

            [JsonPropertyName("generate_method")]
            public required Hash_Method generate_method { get; set; }

            [JsonPropertyName("verify_method")]
            public required Hash_Method verify_method { get; set; }
        }

        public class Hash_Method
        {
            [JsonPropertyName("md5")]
            public int md5 { get; set; }

            [JsonPropertyName("sha1")]
            public int sha1 { get; set; }

            [JsonPropertyName("sha2-256")]
            public int sha2_256 { get; set; }

            [JsonPropertyName("sha2-384")]
            public int sha2_384 { get; set; }

            [JsonPropertyName("sha2-512")]
            public int sha2_512 { get; set; }

            [JsonPropertyName("sha3-224")]
            public int sha3_224 { get; set; }

            [JsonPropertyName("sha3-256")]
            public int sha3_256 { get; set; }

            [JsonPropertyName("sha3-384")]
            public int sha3_384 { get; set; }

            [JsonPropertyName("sha3-512")]
            public int sha3_512 { get; set; }

            [JsonPropertyName("shake128")]
            public int shake128 { get; set; }

            [JsonPropertyName("shake256")]
            public int shake256 { get; set; }

            [JsonPropertyName("blake2b")]
            public int blake2b { get; set; }

            [JsonPropertyName("blake2bp")]
            public int blake2bp { get; set; }

            [JsonPropertyName("blake2s")]
            public int blake2s { get; set; }

            [JsonPropertyName("blake2sp")]
            public int blake2sp { get; set; }

            [JsonPropertyName("blake3")]
            public int blake3 { get; set; }
        }
    }
}
