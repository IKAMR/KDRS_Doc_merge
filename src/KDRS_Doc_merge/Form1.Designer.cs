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
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(12, 256);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(79, 53);
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
            this.textBox1.Location = new System.Drawing.Point(196, 166);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(538, 326);
            this.textBox1.TabIndex = 6;
            // 
            // btnRunMerge
            // 
            this.btnRunMerge.Location = new System.Drawing.Point(12, 189);
            this.btnRunMerge.Name = "btnRunMerge";
            this.btnRunMerge.Size = new System.Drawing.Size(164, 60);
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
            this.progressBar1.Location = new System.Drawing.Point(10, 498);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(723, 13);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 9;
            // 
            // txtInputFile
            // 
            this.txtInputFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFile.Location = new System.Drawing.Point(111, 17);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.Size = new System.Drawing.Size(623, 20);
            this.txtInputFile.TabIndex = 10;
            // 
            // btnInputFile
            // 
            this.btnInputFile.Location = new System.Drawing.Point(11, 8);
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
            this.btnTargetFolder.Location = new System.Drawing.Point(11, 49);
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
            this.txtTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetFolder.Location = new System.Drawing.Point(111, 58);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.Size = new System.Drawing.Size(623, 20);
            this.txtTargetFolder.TabIndex = 13;
            // 
            // chkBxMakeSiard
            // 
            this.chkBxMakeSiard.AutoSize = true;
            this.chkBxMakeSiard.Checked = true;
            this.chkBxMakeSiard.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBxMakeSiard.Location = new System.Drawing.Point(12, 138);
            this.chkBxMakeSiard.Name = "chkBxMakeSiard";
            this.chkBxMakeSiard.Size = new System.Drawing.Size(81, 17);
            this.chkBxMakeSiard.TabIndex = 14;
            this.chkBxMakeSiard.Text = "Make .siard";
            this.chkBxMakeSiard.UseVisualStyleBackColor = true;
            // 
            // txtZip64Jar
            // 
            this.txtZip64Jar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZip64Jar.Location = new System.Drawing.Point(196, 136);
            this.txtZip64Jar.Name = "txtZip64Jar";
            this.txtZip64Jar.Size = new System.Drawing.Size(425, 20);
            this.txtZip64Jar.TabIndex = 15;
            // 
            // btnZip64Jar
            // 
            this.btnZip64Jar.Location = new System.Drawing.Point(98, 133);
            this.btnZip64Jar.Margin = new System.Windows.Forms.Padding(2);
            this.btnZip64Jar.Name = "btnZip64Jar";
            this.btnZip64Jar.Size = new System.Drawing.Size(95, 24);
            this.btnZip64Jar.TabIndex = 16;
            this.btnZip64Jar.Text = "Choose zip64.jar";
            this.btnZip64Jar.UseVisualStyleBackColor = true;
            this.btnZip64Jar.Click += new System.EventHandler(this.btnZip64Jar_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(97, 255);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(79, 53);
            this.btnTest.TabIndex = 17;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // chkBxFilesLimit
            // 
            this.chkBxFilesLimit.AutoSize = true;
            this.chkBxFilesLimit.Location = new System.Drawing.Point(12, 166);
            this.chkBxFilesLimit.Name = "chkBxFilesLimit";
            this.chkBxFilesLimit.Size = new System.Drawing.Size(139, 17);
            this.chkBxFilesLimit.TabIndex = 18;
            this.chkBxFilesLimit.Text = "Limit to 500 files pr table";
            this.chkBxFilesLimit.UseVisualStyleBackColor = true;
            // 
            // txtSchemaName
            // 
            this.txtSchemaName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchemaName.Location = new System.Drawing.Point(111, 99);
            this.txtSchemaName.Name = "txtSchemaName";
            this.txtSchemaName.Size = new System.Drawing.Size(197, 20);
            this.txtSchemaName.TabIndex = 19;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Schema name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(315, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "New schema name (optional)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 523);
            this.Controls.Add(this.label2);
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
            this.Name = "Form1";
            this.Text = "KDRS SIARD TOOLS";
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
        private System.Windows.Forms.Label label2;
    }
}

