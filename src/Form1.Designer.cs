namespace binFileMerger
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Reset = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnRunMerge = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.btnInputFile = new System.Windows.Forms.Button();
            this.btnTargetFolder = new System.Windows.Forms.Button();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.chkBxMakeSiard = new System.Windows.Forms.CheckBox();
            this.txtZip64Jar = new System.Windows.Forms.TextBox();
            this.btnZip64Jar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(11, 319);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(164, 53);
            this.Reset.TabIndex = 7;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(195, 254);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(538, 118);
            this.textBox1.TabIndex = 6;
            // 
            // btnRunMerge
            // 
            this.btnRunMerge.Location = new System.Drawing.Point(11, 252);
            this.btnRunMerge.Name = "btnRunMerge";
            this.btnRunMerge.Size = new System.Drawing.Size(164, 60);
            this.btnRunMerge.TabIndex = 5;
            this.btnRunMerge.Text = "Merge files";
            this.btnRunMerge.UseVisualStyleBackColor = true;
            this.btnRunMerge.Click += new System.EventHandler(this.btnRunMerge_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(11, 169);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(722, 69);
            this.listBox1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 11);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(95, 26);
            this.button2.TabIndex = 8;
            this.button2.Text = "Choose init-file";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 378);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(721, 23);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 9;
            // 
            // txtInputFile
            // 
            this.txtInputFile.Location = new System.Drawing.Point(111, 63);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(623, 20);
            this.txtInputFile.TabIndex = 10;
            // 
            // btnInputFile
            // 
            this.btnInputFile.Location = new System.Drawing.Point(11, 46);
            this.btnInputFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnInputFile.Name = "btnInputFile";
            this.btnInputFile.Size = new System.Drawing.Size(95, 37);
            this.btnInputFile.TabIndex = 11;
            this.btnInputFile.Text = "Choose input-file siard/metadata";
            this.btnInputFile.UseVisualStyleBackColor = true;
            this.btnInputFile.Click += new System.EventHandler(this.btnInputFile_Click);
            // 
            // btnTargetFolder
            // 
            this.btnTargetFolder.Location = new System.Drawing.Point(11, 87);
            this.btnTargetFolder.Margin = new System.Windows.Forms.Padding(2);
            this.btnTargetFolder.Name = "btnTargetFolder";
            this.btnTargetFolder.Size = new System.Drawing.Size(95, 37);
            this.btnTargetFolder.TabIndex = 12;
            this.btnTargetFolder.Text = "Choose target folder";
            this.btnTargetFolder.UseVisualStyleBackColor = true;
            this.btnTargetFolder.Click += new System.EventHandler(this.btnTargetFolder_Click);
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Location = new System.Drawing.Point(111, 104);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(623, 20);
            this.txtTargetFolder.TabIndex = 13;
            // 
            // chkBxMakeSiard
            // 
            this.chkBxMakeSiard.AutoSize = true;
            this.chkBxMakeSiard.Location = new System.Drawing.Point(122, 17);
            this.chkBxMakeSiard.Name = "chkBxMakeSiard";
            this.chkBxMakeSiard.Size = new System.Drawing.Size(81, 17);
            this.chkBxMakeSiard.TabIndex = 14;
            this.chkBxMakeSiard.Text = "Make .siard";
            this.chkBxMakeSiard.UseVisualStyleBackColor = true;
            // 
            // txtZip64Jar
            // 
            this.txtZip64Jar.Location = new System.Drawing.Point(308, 17);
            this.txtZip64Jar.Name = "txtZip64Jar";
            this.txtZip64Jar.Size = new System.Drawing.Size(425, 20);
            this.txtZip64Jar.TabIndex = 15;
            // 
            // btnZip64Jar
            // 
            this.btnZip64Jar.Location = new System.Drawing.Point(208, 11);
            this.btnZip64Jar.Margin = new System.Windows.Forms.Padding(2);
            this.btnZip64Jar.Name = "btnZip64Jar";
            this.btnZip64Jar.Size = new System.Drawing.Size(95, 28);
            this.btnZip64Jar.TabIndex = 16;
            this.btnZip64Jar.Text = "zip64 jar";
            this.btnZip64Jar.UseVisualStyleBackColor = true;
            this.btnZip64Jar.Click += new System.EventHandler(this.btnZip64Jar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 412);
            this.Controls.Add(this.btnZip64Jar);
            this.Controls.Add(this.txtZip64Jar);
            this.Controls.Add(this.chkBxMakeSiard);
            this.Controls.Add(this.txtTargetFolder);
            this.Controls.Add(this.btnTargetFolder);
            this.Controls.Add(this.btnInputFile);
            this.Controls.Add(this.txtInputFile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnRunMerge);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "KDRS SIARD TOOLS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnRunMerge;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.Button btnInputFile;
        private System.Windows.Forms.Button btnTargetFolder;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox chkBxMakeSiard;
        private System.Windows.Forms.TextBox txtZip64Jar;
        private System.Windows.Forms.Button btnZip64Jar;
    }
}

