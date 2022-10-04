using CheckHash2;

if (args.Length == 0 || !File.Exists(args[0]))
{
    Console.WriteLine("没有输入配置文件！");
    while (true)
        Console.ReadLine();
}
SettingStruct.SettingConfig setting = Utilities.getSetting(args[0]);

// 检查设置选项
// 检查check_folder_config_file是否存在
if (!File.Exists(setting.check_folder_config_file))
{
    Console.WriteLine("hash文件夹配置文件" + setting.check_folder_config_file + "不存在！");
    while (true)
        Console.ReadLine();
}
// 检查blake程序是否存在
if (!File.Exists(setting.blake2_exe_path))
{
    Console.WriteLine("BLAKE2程序" + setting.blake2_exe_path + "不存在！");
    while (true)
        Console.ReadLine();
}
if (!File.Exists(setting.blake3_exe_path))
{
    Console.WriteLine("BLAKE3程序" + setting.blake3_exe_path + "不存在！");
    while (true)
        Console.ReadLine();
}
// 读入检查目录内容
List<string> check_folder_list = new List<string>();
using (StreamReader stream_reader = new StreamReader(setting.check_folder_config_file, Utilities.utf8_encoding))
{
    while (!stream_reader.EndOfStream)
    {
        string line = stream_reader.ReadLine();
        if (string.IsNullOrEmpty(line))
        {
            continue;
        }
        check_folder_list.Add(line);
    }
}
// 输出设置
Console.WriteLine("所有设置：");
Console.WriteLine("## check_folder");
int no = 0;
foreach (var path in check_folder_list)
    Console.WriteLine((++no) + ". " + path);
Console.WriteLine("## blake2_exe_path: " + setting.blake2_exe_path);
Console.WriteLine("## blake3_exe_path: " + setting.blake3_exe_path);
// 设置hash_method_name
List<string> hash_method_name = new List<string>()
            {
                Utilities.MD5_NAME,
                Utilities.SHA1_NAME,
                Utilities.SHA256_NAME,
                Utilities.SHA384_NAME,
                Utilities.SHA512_NAME,
                Utilities.BLAKE2b_NAME,
                Utilities.BLAKE2s_NAME,
                Utilities.BLAKE2bp_NAME,
                Utilities.BLAKE2sp_NAME,
                Utilities.BLAKE3_NAME
            };
// 设置hash_method_use
List<bool> hash_method_use = new List<bool>();
if (setting.check_method.md5 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.sha1 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.sha256 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.sha384 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.sha512 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.blake2b != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.blake2s != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.blake2bp != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.blake2sp != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
if (setting.check_method.blake3 != 0)
    hash_method_use.Add(true);
else
    hash_method_use.Add(false);
Console.Write("## check_method: ");
for (int i = 0; i < hash_method_name.Count; i++)
{
    if (hash_method_use[i])
        if (i == 0)
            Console.Write(hash_method_name[i]);
        else
            Console.Write(", " + hash_method_name[i]);
}
Console.Write("\n");
Console.WriteLine("## forecast_remain_time: " + (setting.forecast_remain_time == 1));

// 选择运行模式
Console.WriteLine("请选择程序的运行模式：");
Console.WriteLine("[1] 生成hash文件");
Console.WriteLine("[2] 校验文件hash");
string chooseNo = Console.ReadLine();
switch (chooseNo)
{
    case "1":
        Console.WriteLine("选择了：[1] 生成hash文件\n");
        Controller.generateHash(setting, check_folder_list, hash_method_name, hash_method_use);
        break;
    case "2":
        Console.WriteLine("选择了：[2] 校验文件hash\n");
        Controller.checkHash(setting, check_folder_list);
        break;
    default:
        break;
}
