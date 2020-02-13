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
            this.btnTest = new System.Windows.Forms.Button();
            this.chkBxFilesLimit = new System.Windows.Forms.CheckBox();
            this.txtSchemaName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(22, 473);
            this.Reset.Margin = new System.Windows.Forms.Padding(6);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(145, 98);
            this.Reset.TabIndex = 7;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(359, 306);
            this.textBox1.Margin = new System.Windows.Forms.Padding(6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(983, 405);
            this.textBox1.TabIndex = 6;
            // 
            // btnRunMerge
            // 
            this.btnRunMerge.Location = new System.Drawing.Point(22, 349);
            this.btnRunMerge.Margin = new System.Windows.Forms.Padding(6);
            this.btnRunMerge.Name = "btnRunMerge";
            this.btnRunMerge.Size = new System.Drawing.Size(301, 111);
            this.btnRunMerge.TabIndex = 5;
            this.btnRunMerge.Text = "Merge files";
            this.btnRunMerge.UseVisualStyleBackColor = true;
            this.btnRunMerge.Click += new System.EventHandler(this.btnRunMerge_Click);
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
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(18, 935);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1326, 24);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 9;
            // 
            // txtInputFile
            // 
            this.txtInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFile.Location = new System.Drawing.Point(204, 31);
            this.txtInputFile.Margin = new System.Windows.Forms.Padding(6);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(1139, 29);
            this.txtInputFile.TabIndex = 10;
            // 
            // btnInputFile
            // 
            this.btnInputFile.Location = new System.Drawing.Point(20, 15);
            this.btnInputFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnInputFile.Name = "btnInputFile";
            this.btnInputFile.Size = new System.Drawing.Size(174, 68);
            this.btnInputFile.TabIndex = 11;
            this.btnInputFile.Text = "Choose input-file siard/metadata";
            this.btnInputFile.UseVisualStyleBackColor = true;
            this.btnInputFile.Click += new System.EventHandler(this.btnInputFile_Click);
            // 
            // btnTargetFolder
            // 
            this.btnTargetFolder.Location = new System.Drawing.Point(20, 90);
            this.btnTargetFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnTargetFolder.Name = "btnTargetFolder";
            this.btnTargetFolder.Size = new System.Drawing.Size(174, 68);
            this.btnTargetFolder.TabIndex = 12;
            this.btnTargetFolder.Text = "Choose target folder";
            this.btnTargetFolder.UseVisualStyleBackColor = true;
            this.btnTargetFolder.Click += new System.EventHandler(this.btnTargetFolder_Click);
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFolder.Location = new System.Drawing.Point(204, 107);
            this.txtTargetFolder.Margin = new System.Windows.Forms.Padding(6);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(1139, 29);
            this.txtTargetFolder.TabIndex = 13;
            // 
            // chkBxMakeSiard
            // 
            this.chkBxMakeSiard.AutoSize = true;
            this.chkBxMakeSiard.Checked = true;
            this.chkBxMakeSiard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxMakeSiard.Location = new System.Drawing.Point(22, 255);
            this.chkBxMakeSiard.Margin = new System.Windows.Forms.Padding(6);
            this.chkBxMakeSiard.Name = "chkBxMakeSiard";
            this.chkBxMakeSiard.Size = new System.Drawing.Size(139, 29);
            this.chkBxMakeSiard.TabIndex = 14;
            this.chkBxMakeSiard.Text = "Make .siard";
            this.chkBxMakeSiard.UseVisualStyleBackColor = true;
            // 
            // txtZip64Jar
            // 
            this.txtZip64Jar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZip64Jar.Location = new System.Drawing.Point(359, 246);
            this.txtZip64Jar.Margin = new System.Windows.Forms.Padding(6);
            this.txtZip64Jar.Name = "txtZip64Jar";
            this.txtZip64Jar.Size = new System.Drawing.Size(978, 29);
            this.txtZip64Jar.TabIndex = 15;
            // 
            // btnZip64Jar
            // 
            this.btnZip64Jar.Location = new System.Drawing.Point(157, 246);
            this.btnZip64Jar.Margin = new System.Windows.Forms.Padding(4);
            this.btnZip64Jar.Name = "btnZip64Jar";
            this.btnZip64Jar.Size = new System.Drawing.Size(174, 44);
            this.btnZip64Jar.TabIndex = 16;
            this.btnZip64Jar.Text = "Choose zip64.jar";
            this.btnZip64Jar.UseVisualStyleBackColor = true;
            this.btnZip64Jar.Click += new System.EventHandler(this.btnZip64Jar_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(178, 471);
            this.btnTest.Margin = new System.Windows.Forms.Padding(6);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(145, 98);
            this.btnTest.TabIndex = 17;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // chkBxFilesLimit
            // 
            this.chkBxFilesLimit.AutoSize = true;
            this.chkBxFilesLimit.Location = new System.Drawing.Point(22, 306);
            this.chkBxFilesLimit.Margin = new System.Windows.Forms.Padding(6);
            this.chkBxFilesLimit.Name = "chkBxFilesLimit";
            this.chkBxFilesLimit.Size = new System.Drawing.Size(245, 29);
            this.chkBxFilesLimit.TabIndex = 18;
            this.chkBxFilesLimit.Text = "Limit to 500 files pr table";
            this.chkBxFilesLimit.UseVisualStyleBackColor = true;
            // 
            // txtSchemaName
            // 
            this.txtSchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchemaName.Location = new System.Drawing.Point(359, 188);
            this.txtSchemaName.Margin = new System.Windows.Forms.Padding(6);
            this.txtSchemaName.Name = "txtSchemaName";
            this.txtSchemaName.Size = new System.Drawing.Size(398, 29);
            this.txtSchemaName.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 188);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(298, 25);
            this.label1.TabIndex = 20;
            this.label1.Text = "Set new schema name (optional)";
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(361, 737);
            this.textBox2.Margin = new System.Windows.Forms.Padding(6);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(983, 177);
            this.textBox2.TabIndex = 21;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 982);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSchemaName);
            this.Controls.Add(this.chkBxFilesLimit);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnZip64Jar);
            this.Controls.Add(this.txtZip64Jar);
            this.Controls.Add(this.chkBxMakeSiard);
            this.Controls.Add(this.txtTargetFolder);
            this.Controls.Add(this.btnTargetFolder);
            this.Controls.Add(this.btnInputFile);
            this.Controls.Add(this.txtInputFile);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnRunMerge);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "KDRS Doc merge";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnRunMerge;
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
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.CheckBox chkBxFilesLimit;
        private System.Windows.Forms.TextBox txtSchemaName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

