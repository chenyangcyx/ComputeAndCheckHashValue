using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace ConsoleApp3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 获取配置文件
                FileInfo programSettingFileinfo = getSettingFile(args);
                SettingStruct.SettingConfig setting = Utilities.getSetting(programSettingFileinfo.FullName);

                // 检查设置选项
                // 检查check_folder_config_file是否存在
                if (!File.Exists(setting.check_folder_config_file))
                {
                    throw new Exception("配置【check_folder_config_file】" + setting.check_folder_config_file + "不存在！");
                }

                // 准备程序运行必要的文件和参数
                prepareProgram(setting);

                string programFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                // 读入检查目录内容
                string check_folder_path1 = programFolderPath + setting.check_folder_config_file;
                string check_folder_path2 = setting.check_folder_config_file;
                if (File.Exists(check_folder_path1))
                {
                    setting.check_folder_config_file = check_folder_path1;
                }
                else if (File.Exists(check_folder_path2))
                {
                    setting.check_folder_config_file = check_folder_path2;
                }
                else
                {
                    throw new Exception("检查目录" + setting.check_folder_config_file + "不存在！");
                }
                List<string> check_folder_list = new List<string>();
                using (StreamReader stream_reader = new StreamReader(setting.check_folder_config_file, Utilities.UTF8_ENCODING))
                {
                    while (!stream_reader.EndOfStream)
                    {
                        string line = stream_reader.ReadLine()!;
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
                Console.WriteLine("## blake2_exe_path: " + Utilities.BLAKE2_EXE_PATH);
                Console.WriteLine("## blake3_exe_path: " + Utilities.BLAKE3_EXE_PATH);
                // 设置hash_method_name, generate_method_use
                List<string> hash_method_name = new List<string>();
                List<bool> generate_method_use = new List<bool>();
                setHashMethod(setting, hash_method_name, generate_method_use);
                // 设置verify_method_use
                List<bool> verify_method_use = new List<bool>();
                setCheckMethod(setting, verify_method_use);
                // 输出generate_method
                Console.Write("## generate_method: ");
                List<string> generateMethodList = new List<string>();
                for (int i = 0; i < hash_method_name.Count; i++)
                {
                    if (generate_method_use[i])
                    {
                        generateMethodList.Add(hash_method_name[i]);
                    }
                }
                Console.WriteLine(string.Join(", ", generateMethodList));
                // 输出verify_method
                Console.Write("## verify_method: ");
                List<string> verifyMethodList = new List<string>();
                for (int i = 0; i < hash_method_name.Count; i++)
                {
                    if (verify_method_use[i])
                        verifyMethodList.Add((hash_method_name[i]));
                }
                Console.WriteLine(string.Join(", ", verifyMethodList));

                // 选择运行模式
                Console.WriteLine("请选择程序的运行模式：");
                Console.WriteLine("[1] 生成hash文件");
                Console.WriteLine("[2] 生成hash文件（不使用run_log.txt）");
                Console.WriteLine("[3] 校验文件hash");
                Console.WriteLine("[4] 查看并输出setting文件demo");
                Console.WriteLine("[5] 查看NET8功能支持情况");
                string chooseNo = Console.ReadLine()!;
                switch (chooseNo)
                {
                    case "1":
                        Console.WriteLine("选择了：[1] 生成hash文件\n");
                        Controller.generateHash(setting, check_folder_list, hash_method_name, generate_method_use, true);
                        break;
                    case "2":
                        Console.WriteLine("选择了：[2] 生成hash文件（不使用run_log.txt）\n");
                        Controller.generateHash(setting, check_folder_list, hash_method_name, generate_method_use, false);
                        break;
                    case "3":
                        Console.WriteLine("选择了：[3] 校验文件hash\n");
                        Controller.checkHash(setting, check_folder_list, hash_method_name, verify_method_use);
                        break;
                    case "4":
                        Console.WriteLine("选择了：[4] 查看并输出setting文件demo\n");
                        Controller.showAndOutputSettingDemo();
                        break;
                    case "5":
                        Console.WriteLine("选择了：[5] 查看当前系统信息\n");
                        Controller.checkSystemInfo();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n##### 程序运行时发生异常异常消息 #####");
                Console.WriteLine($"【异常消息】{e.Message}");
                Console.WriteLine($"【异常来源】{e.Source}");
                Console.WriteLine($"【异常详细信息】");
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                if (Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH == null)
                {
                    Console.WriteLine("程序未被正确初始化，直接退出！\n\n");
                    while (true) { Console.ReadLine(); }
                }
                DirectoryInfo directory = new DirectoryInfo(Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH);

                Console.WriteLine("\n\n********************************\n**********程序运行结束**********\n********************************");
                Console.WriteLine($"程序运行的临时目录：{Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH}");
                if (directory.Exists)
                {
                    FileInfo[] directoryFiles = directory.GetFiles();
                    List<string> all_file_name_list = new List<string>();
                    List<string> all_file_byte_size_list = new List<string>();
                    foreach (FileInfo file in directoryFiles)
                    {
                        all_file_name_list.Add(file.Name);
                        all_file_byte_size_list.Add(file.Length.ToString("0.00") + " KB");
                    }
                    int max_space1 = calWriteLineSpace(all_file_name_list, 10, false);
                    int max_space2 = calWriteLineSpace(all_file_byte_size_list, 8, false);
                    int max_space2_left = calWriteLineSpace(all_file_byte_size_list, 8, true);
                    for (int i = 1; i <= directoryFiles.Length; i++)
                    {
                        Console.Write($" {i}. {directoryFiles[i - 1].Name}");
                        for (int j = 1; j <= max_space1 - directoryFiles[i - 1].Name.Length; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.Write($"{(directoryFiles[i - 1].Length / 1024.0d).ToString("0.00")} KB");
                        for (int j = 1; j <= max_space2 - all_file_byte_size_list[i - 1].Length; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.Write("=");
                        for (int j = 1; j <= max_space2_left; j++)
                        {
                            Console.Write(" ");
                        }
                        Console.WriteLine($"{(directoryFiles[i - 1].Length / 1048576.0d).ToString("0.00")} MB");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine(" ● 输入00退出程序");
                Console.WriteLine(" ● 输入11删除临时目录并退出程序");
                Console.Write("\n\n");

                // 输出信息，直至输入00完全退出
                while (true)
                {
                    string input = Console.ReadLine()!;
                    if (input.Equals("00"))
                    {
                        Environment.Exit(0);
                    }
                    else if (input.Equals("11"))
                    {
                        // 清理temp目录
                        if (Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH != null)
                        {

                            if (directory.Exists)
                            {
                                Console.WriteLine($"temp目录存在，清理目录：{directory.FullName}");
                                directory.Delete(true);
                            }
                            else
                            {
                                Console.WriteLine($"目录不存在，不需要清理：{directory.FullName}");
                            }
                        }
                        // 退出程序
                        Environment.Exit(0);
                    }
                }
            }
        }

        static void setHashMethod(SettingStruct.SettingConfig setting, List<string> hash_method_name, List<bool> generate_method_use)
        {
            // 设置hash_method_name
            hash_method_name.Add(Utilities.MD5_NAME);
            hash_method_name.Add(Utilities.SHA1_NAME);
            hash_method_name.Add(Utilities.SHA2_256_NAME);
            hash_method_name.Add(Utilities.SHA2_384_NAME);
            hash_method_name.Add(Utilities.SHA2_512_NAME);
            hash_method_name.Add(Utilities.SHA3_224_NAME);
            hash_method_name.Add(Utilities.SHA3_256_NAME);
            hash_method_name.Add(Utilities.SHA3_384_NAME);
            hash_method_name.Add(Utilities.SHA3_512_NAME);
            hash_method_name.Add(Utilities.SHAKE128_NAME);
            hash_method_name.Add(Utilities.SHAKE256_NAME);
            hash_method_name.Add(Utilities.BLAKE2b_NAME);
            hash_method_name.Add(Utilities.BLAKE2s_NAME);
            hash_method_name.Add(Utilities.BLAKE2bp_NAME);
            hash_method_name.Add(Utilities.BLAKE2sp_NAME);
            hash_method_name.Add(Utilities.BLAKE3_NAME);
            /** MD5 **/
            if (setting.generate_method.md5 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** MD5 **/

            /** SHA1 **/
            if (setting.generate_method.sha1 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** SHA1 **/

            /** SHA2 **/
            if (setting.generate_method.sha2_256 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.sha2_384 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.sha2_512 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** SHA2 **/

            /** SHA3 **/
            if (setting.generate_method.sha3_224 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.sha3_256 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.sha3_384 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.sha3_512 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.shake128 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.shake256 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** SHA3 **/

            /** BLAKE2 **/
            if (setting.generate_method.blake2b != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.blake2s != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.blake2bp != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            if (setting.generate_method.blake2sp != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** BLAKE2 **/

            /** BLAKE3 **/
            if (setting.generate_method.blake3 != 0)
                generate_method_use.Add(true);
            else
                generate_method_use.Add(false);
            /** BLAKE3 **/
        }

        static void setCheckMethod(SettingStruct.SettingConfig setting, List<bool> verify_method_use)
        {
            if (setting.verify_method.md5 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);

            if (setting.verify_method.sha1 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);

            if (setting.verify_method.sha2_256 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.sha2_384 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.sha2_512 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);

            if (setting.verify_method.sha3_224 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.sha3_256 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.sha3_384 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.sha3_512 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.shake128 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.shake256 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);

            if (setting.verify_method.blake2b != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.blake2s != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.blake2bp != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
            if (setting.verify_method.blake2sp != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);

            if (setting.verify_method.blake3 != 0)
                verify_method_use.Add(true);
            else
                verify_method_use.Add(false);
        }

        static FileInfo getSettingFile(string[] args)
        {
            // 如果运行参数中给出且文件存在 则直接返回
            if (args.Length > 0 && File.Exists(args[0]))
            {
                return new FileInfo(args[0]);
            }
            // 其他情况，读取程序所在目录下所有的.json/.txt文件 提供给用户选择
            List<FileInfo> allFolderFiles = new List<FileInfo>(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).GetFiles());
            List<FileInfo> toChooseFiles = new List<FileInfo>();
            foreach (FileInfo file in allFolderFiles)
            {
                if (file.Name.ToLower().EndsWith(".json") || file.Name.ToLower().EndsWith(".txt"))
                {
                    // 对于toChooseFiles中的文件逐个反序列化尝试看是否是符合标准的配置文件
                    try
                    {
                        Utilities.getSetting(file.FullName);
                        toChooseFiles.Add(file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            // 如果没有符合条件的配置文件，则直接退出
            if (toChooseFiles.Count == 0)
            {
                Console.WriteLine($"程序目录下没有找到符合配置文件格式的文件！");
                throw new Exception("程序目录下没有找到符合配置文件格式的文件");
            }

            // 输出提示，让用户选择
            Console.WriteLine($"未能够自动找到合适的配置文件，找到{toChooseFiles.Count}个目录下文件：");
            for (int index = 0; index < toChooseFiles.Count + 1; index++)
            {
                if (index == toChooseFiles.Count)
                {
                    Console.WriteLine($"【{index}】 退出");
                }
                else
                {
                    Console.WriteLine($"【{index}】 {toChooseFiles[index].Name}");
                }
            }
            Console.Write("请选择要执行的操作：");
            string chooseInput = Console.ReadLine()!;
            int chooseNum = int.Parse(chooseInput);
            if (chooseNum >= 0 && chooseNum <= toChooseFiles.Count)
            {
                if (chooseNum == toChooseFiles.Count)
                {
                    throw new Exception("退出程序！");
                }
                Console.WriteLine($"选择了第【{chooseNum}】项: {toChooseFiles[chooseNum].Name}");
                return toChooseFiles[chooseNum];
            }
            else
            {
                throw new Exception("未选择有效的项目，退出程序！");
            }
        }

        static void prepareProgram(SettingStruct.SettingConfig setting)
        {
            // 检测是否可以使用NET原生支持的SHAKE和SHA3算法实现
            Utilities.CAN_USE_NET_SHAKE_ALGORITHM = Shake128.IsSupported && Shake256.IsSupported;
            Utilities.CAN_USE_NET_SHA3_ALGORITHM = SHA3_256.IsSupported && SHA3_384.IsSupported && SHA3_512.IsSupported;

            // 生成程序此次运行的临时目录名称
            List<string> allFolderNameList = new List<string>();
            using (StreamReader stream_reader = new StreamReader(setting.check_folder_config_file, Utilities.UTF8_ENCODING))
            {
                while (!stream_reader.EndOfStream)
                {
                    string line = stream_reader.ReadLine()!;
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    allFolderNameList.Add(line);
                }
            }
            allFolderNameList.Sort((a, b) => a.CompareTo(b));
            Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME = Utilities.PROGRAM_NAME + "-" + ComputeHash.getSHA2_256(string.Join(",", allFolderNameList));
            Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME);
            Utilities.PROGRAM_RUNNING_LOG_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME, Utilities.PROGRAM_LOG_FILE_NAME);
            DirectoryInfo programTempFolderDirectoryInfo = new DirectoryInfo(Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_PATH);

            // 初始化blake2和blake3程序的地址
            string blake2Name, blake3Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                blake2Name = Utilities.EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_WINDOWS;
                blake3Name = Utilities.EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_WINDOWS;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                blake2Name = Utilities.EMBEDDED_RESOURCE_NAME_BLAKE2_AMD64_LINUX;
                blake3Name = Utilities.EMBEDDED_RESOURCE_NAME_BLAKE3_AMD64_LINUX;
            }
            else
            {
                throw new Exception("当前系统暂不支持BLAKE程序！");
            }

            // 复制blake2和blake3程序到临时目录下
            Stream blake2Stream = Utilities.getMainfestResourceStream(blake2Name);
            Stream blake3Stream = Utilities.getMainfestResourceStream(blake3Name);
            FileInfo blake2FileInfo = Utilities.copyEmbeddedResourceToTempFolder(blake2Stream, new List<string>() { Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME }, blake2Name);
            FileInfo blake3FileInfo = Utilities.copyEmbeddedResourceToTempFolder(blake3Stream, new List<string>() { Utilities.PROGRAM_RUNNING_PARAM_TEMP_FOLDER_NAME }, blake3Name);
            if (programTempFolderDirectoryInfo.Exists)
            {
                Console.WriteLine();

                Console.WriteLine($"程序临时目录已存在：{programTempFolderDirectoryInfo.FullName}");

                Console.WriteLine($">> {blake2Name}: {blake2FileInfo.FullName}");
                Utilities.BLAKE2_EXE_PATH = blake2FileInfo.FullName;

                Console.WriteLine($">> {blake3Name}: {blake3FileInfo.FullName}");
                Utilities.BLAKE3_EXE_PATH = blake3FileInfo.FullName;

                Console.WriteLine();
                return;
            }
            else
            {
                Console.WriteLine();

                Console.WriteLine($">> 复制文件[{blake2Name}]到目标地址：{blake2FileInfo.FullName}");
                Utilities.BLAKE2_EXE_PATH = blake2FileInfo.FullName;

                Console.WriteLine($">> 复制文件[{blake3Name}]到目标地址：{blake3FileInfo.FullName}");
                Utilities.BLAKE3_EXE_PATH = blake3FileInfo.FullName;

                Console.WriteLine();
            }
        }

        static int calWriteLineSpace(List<string> lines, int unit, bool just_need_space = true)
        {
            // 获取lines中的最大长度
            int max_length = -1;
            foreach (string item in lines)
            {
                max_length = item.Length >= max_length ? item.Length : max_length;
            }
            // 计算大于max_length的最大长度
            int count = 1;
            while (true)
            {
                int all_spaces = count * unit;
                if (all_spaces <= max_length)
                {
                    count++;
                    continue;
                }
                if (just_need_space)
                {
                    return all_spaces - max_length;
                }
                else
                {
                    return all_spaces;
                }
            }
        }
    }
}
