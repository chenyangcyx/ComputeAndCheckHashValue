namespace ComputeMD5_GUI
{
    partial class Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label_path = new System.Windows.Forms.Label();
            this.textBox_path = new System.Windows.Forms.TextBox();
            this.textBox_detail = new System.Windows.Forms.TextBox();
            this.label_detail = new System.Windows.Forms.Label();
            this.button_start = new System.Windows.Forms.Button();
            this.progressBar_path = new System.Windows.Forms.ProgressBar();
            this.progressBar_all = new System.Windows.Forms.ProgressBar();
            this.button_clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_path
            // 
            this.label_path.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_path.Location = new System.Drawing.Point(13, 13);
            this.label_path.Name = "label_path";
            this.label_path.Size = new System.Drawing.Size(252, 31);
            this.label_path.TabIndex = 0;
            this.label_path.Text = "计算地址";
            // 
            // textBox_path
            // 
            this.textBox_path.Location = new System.Drawing.Point(15, 47);
            this.textBox_path.Multiline = true;
            this.textBox_path.Name = "textBox_path";
            this.textBox_path.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_path.Size = new System.Drawing.Size(250, 379);
            this.textBox_path.TabIndex = 1;
            // 
            // textBox_detail
            // 
            this.textBox_detail.Location = new System.Drawing.Point(272, 47);
            this.textBox_detail.Multiline = true;
            this.textBox_detail.Name = "textBox_detail";
            this.textBox_detail.ReadOnly = true;
            this.textBox_detail.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_detail.Size = new System.Drawing.Size(491, 379);
            this.textBox_detail.TabIndex = 2;
            // 
            // label_detail
            // 
            this.label_detail.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_detail.Location = new System.Drawing.Point(272, 13);
            this.label_detail.Name = "label_detail";
            this.label_detail.Size = new System.Drawing.Size(491, 31);
            this.label_detail.TabIndex = 3;
            this.label_detail.Text = "处理详情";
            // 
            // button_start
            // 
            this.button_start.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_start.Location = new System.Drawing.Point(658, 432);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(106, 35);
            this.button_start.TabIndex = 4;
            this.button_start.Text = "开始";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // progressBar_path
            // 
            this.progressBar_path.Location = new System.Drawing.Point(15, 432);
            this.progressBar_path.Name = "progressBar_path";
            this.progressBar_path.Size = new System.Drawing.Size(636, 35);
            this.progressBar_path.TabIndex = 5;
            // 
            // progressBar_all
            // 
            this.progressBar_all.Location = new System.Drawing.Point(16, 473);
            this.progressBar_all.Name = "progressBar_all";
            this.progressBar_all.Size = new System.Drawing.Size(636, 35);
            this.progressBar_all.TabIndex = 6;
            // 
            // button_clear
            // 
            this.button_clear.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_clear.Location = new System.Drawing.Point(658, 473);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(105, 35);
            this.button_clear.TabIndex = 7;
            this.button_clear.Text = "清空";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 517);
            this.Controls.Add(this.button_clear);
            this.Controls.Add(this.progressBar_all);
            this.Controls.Add(this.progressBar_path);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.label_detail);
            this.Controls.Add(this.textBox_detail);
            this.Controls.Add(this.textBox_path);
            this.Controls.Add(this.label_path);
            this.Name = "Form";
            this.Text = "Form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_path;
        private System.Windows.Forms.TextBox textBox_path;
        private System.Windows.Forms.TextBox textBox_detail;
        private System.Windows.Forms.Label label_detail;
        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.ProgressBar progressBar_path;
        private System.Windows.Forms.ProgressBar progressBar_all;
        private System.Windows.Forms.Button button_clear;
    }
}

