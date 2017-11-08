namespace WinApp
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Start = new System.Windows.Forms.Button();
            this.Runsts = new System.Windows.Forms.Label();
            this.Testut = new System.Windows.Forms.Button();
            this.TestRlt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(37, 70);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(75, 23);
            this.Start.TabIndex = 0;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // Runsts
            // 
            this.Runsts.AutoSize = true;
            this.Runsts.Location = new System.Drawing.Point(176, 80);
            this.Runsts.Name = "Runsts";
            this.Runsts.Size = new System.Drawing.Size(29, 12);
            this.Runsts.TabIndex = 2;
            this.Runsts.Text = "init";
            // 
            // Testut
            // 
            this.Testut.Location = new System.Drawing.Point(37, 99);
            this.Testut.Name = "Testut";
            this.Testut.Size = new System.Drawing.Size(75, 23);
            this.Testut.TabIndex = 3;
            this.Testut.Text = "Test";
            this.Testut.UseVisualStyleBackColor = true;
            this.Testut.Click += new System.EventHandler(this.Testut_Click);
            // 
            // TestRlt
            // 
            this.TestRlt.AutoSize = true;
            this.TestRlt.Location = new System.Drawing.Point(176, 104);
            this.TestRlt.Name = "TestRlt";
            this.TestRlt.Size = new System.Drawing.Size(71, 12);
            this.TestRlt.TabIndex = 4;
            this.TestRlt.Text = "Test result";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 151);
            this.Controls.Add(this.TestRlt);
            this.Controls.Add(this.Testut);
            this.Controls.Add(this.Runsts);
            this.Controls.Add(this.Start);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Label Runsts;
        private System.Windows.Forms.Button Testut;
        private System.Windows.Forms.Label TestRlt;
    }
}

