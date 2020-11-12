using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputeMD5_GUI
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private void button_clear_Click(object sender, EventArgs e)
        {
            textBox_path.Clear();
            textBox_detail.Clear();
            progressBar_path.Value = 0;
            progressBar_all.Value = 0;
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            List<string> all_path = new List<string>();
            foreach (string str in textBox_path.Text.Replace("\r",string.Empty).Split('\n'))
                all_path.Add(str);
            int all_file_num = 0;
            foreach (string path in all_path)
                all_file_num += new DirectoryInfo(path).GetFiles().Length;

            textBox_detail.Clear();
            string newline = Environment.NewLine;
            textBox_detail.AppendText("所有目录：" + newline);
            int path_no = 1;
            foreach (string str in all_path)
                textBox_detail.AppendText((path_no++) + ". " + str + newline);
            textBox_detail.AppendText(newline);

            GetMd5 getMd5 = new GetMd5();
            path_no = 0;
            DateTime start_time = DateTime.Now;
            Stopwatch watch = new Stopwatch();
            double progress_path_add = progressBar_path.Maximum * 1.0 / all_path.Count;
            double progress_all_add = progressBar_all.Maximum * 1.0 / all_file_num;
            double progress_path_value = 0d;
            double progress_all_value = 0d;
            watch.Start();
            foreach (string path in all_path)
            {
                textBox_detail.AppendText((path_no + 1) + "：" + path + newline);
                progress_path_value += progress_path_add;
                progressBar_path.Value = (int)progress_path_value;
                Application.DoEvents();
                StreamWriter sw = new StreamWriter(path + "\\hash.md5", false, new UTF8Encoding(false));
                FileInfo[] all_file = new DirectoryInfo(path).GetFiles();
                int file_no = 0;
                foreach (var file in all_file)
                {
                    if (file.FullName.Contains(@"hash.md5"))
                        continue;
                    textBox_detail.AppendText("--filename "+(file_no+1)+": " + file.FullName + newline);
                    progress_all_value += progress_all_add;
                    progressBar_all.Value = (int)progress_all_value;
                    Application.DoEvents();
                    string result = getMd5.GetMd5HashValue(file);
                    textBox_detail.AppendText("--MD5: "+result + newline);
                    sw.Write(result + " *" + file.Name + "\n");
                    file_no++;
                }
                sw.Flush();
                sw.Close();
                textBox_detail.AppendText(newline + newline);
                path_no++;
            }
            watch.Stop();
            DateTime end_time = DateTime.Now;
            progressBar_path.Value = progressBar_path.Maximum;
            progressBar_all.Value=progressBar_all.Maximum;
            textBox_detail.AppendText("全部完成！耗时：" + watch.ElapsedMilliseconds/1000d + "s"+newline);
            textBox_detail.AppendText("开始时间：" + start_time.ToString() + newline);
            textBox_detail.AppendText("结束时间：" + end_time.ToString() + newline);
        }
    }
}
