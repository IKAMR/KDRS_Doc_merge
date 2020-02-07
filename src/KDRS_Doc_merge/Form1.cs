﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace binFileMerger
{
    public partial class Form1 : Form
    {

        string csvFileName;
        string targetFolder;
        string siardFolder;
        string sourceFolder;
        string initFolder;
        string zip64jar;
        string inputFileName;
        string metadataXmlName = String.Empty;

        Boolean filesAdded = false;
        Boolean useSeg = true;

        XmlWriter xmlWriter;

        List<Clob> clobs = new List<Clob>();
        XmlNodeList comments;

        List<string> files = new List<string>();
        List<string> log = new List<string>();

        SiardZipper zipper = new SiardZipper();
        InfoFinder finder = new InfoFinder();

        public Form1()
        {
            InitializeComponent();
        }

        //--------------------------------------------------------------------------------
        // Merges contents of the input list to a file with name 'outFileName'.
        private void BinMergeClob(List<Clob> clobsToMerge, string outFileName)
        {
            using (var outputStream = File.Create(outFileName))
            {
                foreach (Clob clob in clobsToMerge)
                {
                    if (clob.ClobType == "clob")
                    {
                        log.Add(clob.FileId + ";" + outFileName + ";" + "newSeg" + ";" + "droid" + ";" + clobs.Count + ";" + "oldSeg" + ";" + clob.ClobString);
                        byte[] byteArray = Encoding.ASCII.GetBytes(clob.ClobString);
                        using (var inputstream = new MemoryStream(byteArray))
                        {
                            inputstream.CopyTo(outputStream);
                        }
                    }
                    else
                    {
                        //Console.WriteLine(clob.ClobString);
                        log.Add(clob.FileId + ";" + outFileName + ";" + "newSeg" + ";" + "droid" + ";" + clobs.Count + ";" + "oldSeg" + ";" + clob.ClobString);
                        using (var inputStream = File.OpenRead(clob.ClobPath))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------
        // Traverses the list of clobs from the table.xml file and merges the .bin files and text strings with the same fileID.
        private void TableMerge(string tableName)
        {
            string fileID;
            string prevFileID;
            string prevClobType;
            long prevFileCount;

            int fileCount = 0;
            string newFileName;

            List<Clob> mergeClobs = new List<Clob>();

            prevFileID = clobs[0].FileId;
            prevClobType = clobs[0].ClobType;
            prevFileCount = clobs[0].FileCount;
            mergeClobs.Add(clobs[0]);
            fileCount++;

            int counter = 0;

            int clobCount = 0;
            int clobTotal = clobs.Count-2;
            int progress = 0;
            int progressCount = 0;

            foreach (Clob clob in clobs.Skip(1))
            {

                fileID = clob.FileId;

                if (fileID != prevFileID)
                {
                    if (prevClobType == "clob" && clob.FileCount == 1)
                    {
                        string clobFileName = Path.Combine("seg", "clobRec" + clobCount + ".bin");
                        newFileName = FileNameChanger(clob.LobFolder, clobFileName, fileCount);
                        BinMergeClob(mergeClobs, newFileName);
                        clobCount++;

                        AddXMLFileInfo(prevFileID, newFileName);
                        counter++;
                    }
                    else
                    {
                        newFileName = FileNameChanger(clob.LobFolder, mergeClobs[0].ClobString, fileCount);
                        BinMergeClob(mergeClobs, newFileName);

                        AddXMLFileInfo(prevFileID, newFileName);
                        counter++;
                    }

                    mergeClobs.Clear();
                    fileCount = 0;
                    prevFileID = clob.FileId;
                    prevClobType = clob.ClobType;
                    prevFileCount = clob.FileCount;

                }
                mergeClobs.Add(clob);
                fileCount++;

                fileID = clob.FileId;

                if (clob == clobs.Last())
                {
                    if (clob.ClobType == "clob" && clob.FileCount == 1)
                    {
                        string clobFileName = Path.Combine("seg", "clobRec" + clobCount + ".bin");
                        newFileName = FileNameChanger(clob.LobFolder, clobFileName, fileCount);
                        BinMergeClob(mergeClobs, newFileName);
                        clobCount++;

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                    }
                    else
                    {
                        newFileName = FileNameChanger(clob.LobFolder, mergeClobs[0].ClobString, fileCount);
                        BinMergeClob(mergeClobs, newFileName);

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                    }
                }
                progressCount++;
                progress = progressCount * 100 / clobTotal ;

                backgroundWorker1.ReportProgress(progress);
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finshed at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            UpdateTableRows(tableName, counter);

            MakeLogFile(tableName);

            backgroundWorker1.ReportProgress(progress, "Merging complete!");
        }

        //--------------------------------------------------------------------------------
        private void UpdateTableRows(string table, int rows)
        {
            XmlDocument metadata = new XmlDocument();
            string newMetadataPath = Path.Combine(siardFolder, "header", "metadata.xml");
            metadata.Load(newMetadataPath);

            var nsmgr = new XmlNamespaceManager(metadata.NameTable);
            var nameSpace = metadata.DocumentElement.NamespaceURI;
            nsmgr.AddNamespace("ns", nameSpace);

            string query = "descendant::ns:table[ns:folder = '" + table + "']/ns:rows";
            XmlNode root = metadata.DocumentElement;
            XmlNode rowsNode = root.SelectSingleNode(query, nsmgr);

            rowsNode.InnerText = rows.ToString();

            metadata.Save(newMetadataPath);
        }
        //--------------------------------------------------------------------------------
        // Reads the chosen table.xml and put all info in a list of clobs.
        private void ReadTableXml(SiardTableXml lobTable)
        {
            string contentPath = Directory.GetParent(Path.GetDirectoryName(lobTable.TableFilePath)).ToString();
            string clobPath = Path.Combine(contentPath, lobTable.LobPath);

            clobs.Clear();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreComments = false;
            XmlReader xmlReader = XmlReader.Create(inputFileName, xmlReaderSettings);

            Console.WriteLine("xml file: " + inputFileName);
            Console.WriteLine("clob path: " + clobPath);

            XmlDocument tableXml = new XmlDocument();
            tableXml.Load(xmlReader);
            //tableXml.LoadXml("<!--comment--><root><!--comment--></root>");
            XmlNode table = tableXml.DocumentElement;

            var nsmgr = new XmlNamespaceManager(tableXml.NameTable);
            var nameSpace = tableXml.DocumentElement.NamespaceURI;

            nsmgr.AddNamespace("ns", nameSpace);

            int rowCount = 0;
            int rowTotal = table.ChildNodes.Count;
            int progress = 0;
            foreach (XmlNode row in table.ChildNodes)
            {
                if (row.NodeType == XmlNodeType.Comment)
                {
                    // comments.Add(row);
                }
                else
                {
                    string fileId = getNodeText(row, "descendant::ns:c1", nsmgr);

                    long fileCount = getNodeInt(row, "descendant::ns:c2", nsmgr);
                    long clobSize = getNodeInt(row, "descendant::ns:c3", nsmgr);
                    string[] clobString = getAttributeText(row, "descendant::ns:c4", nsmgr);

                    string lobPath = Path.Combine(siardFolder, finder.lobFolder, lobTable.LobPath);
                    clobs.Add(new Clob(fileId, fileCount, clobSize, clobString[0], clobString[1], Path.Combine(clobPath, clobString[0]), lobPath));
                }
                rowCount++;
                progress = rowCount * 100 / rowTotal;
                backgroundWorker1.ReportProgress(progress);
            }

            /*
            foreach (Clob clob in clobs)
            {
                 Console.WriteLine(clob.FileId + "; " + clob.FileCount);
            }*/

            clobs = clobs.OrderBy(o => o.FileId).ThenBy(s => s.FileCount).ToList();

            /*
            foreach (Clob clob in clobs)
            {
                 Console.WriteLine(clob.FileId + "; " + clob.FileCount + "; " + clob.ClobString + "; " + clob.ClobType);
            }
            */
            //   tableXml.CreateComment("test comment");

            comments = tableXml.SelectNodes("//comment()", nsmgr);
            /*
               Console.WriteLine("Comments in file: " + comments.Count);
               foreach (XmlComment comment in tableXml.SelectNodes("//comment()"))
               {
                   Console.WriteLine("Comment: "+ comment.Value);
               }
              */

            //Console.WriteLine("Comment: " + comments[1].Value);

            backgroundWorker1.ReportProgress(progress, "Table xml file read");
        }
        //--------------------------------------------------------------------------------
        // Creates a new filename which includes the number of files the new file consists of.
        #region FileNameChanger
        public string FileNameChanger(string folder, string oldName, int fileCounter)
        {
            string fSegFolder = Path.GetFileName(Path.GetDirectoryName(oldName));
            string fName = Path.GetFileNameWithoutExtension(oldName);
            string fExt = Path.GetExtension(oldName);

            string newSegFolder = Path.Combine(folder, fSegFolder);

            if (!Directory.Exists(newSegFolder))
            {
                Directory.CreateDirectory(newSegFolder);
            }

            if (!useSeg)
            {
                if (fileCounter == 1)
                    return Path.Combine(folder, String.Concat(fName, fExt));

               // return Path.Combine(folder, String.Concat(fName, '_', fileCounter, fExt));
                return Path.Combine(folder, String.Concat(fName, fExt));
            }
            else
            {
                if (fileCounter == 1)
                    return Path.Combine(folder, newSegFolder, String.Concat(fName, fExt));
                //return Path.Combine(folder, newSegFolder, String.Concat(fName, '_', fileCounter, fExt));
                return Path.Combine(folder, newSegFolder, String.Concat(fName, fExt));
            }
        }
        #endregion

        //--------------------------------------------------------------------------------
        // Prints the log list to a .csv file.
        public void MakeLogFile(string table)
        {
            File.WriteAllLines(Path.Combine(Directory.GetParent(siardFolder).ToString(), "filelist_" + table + "_" +  GetTimeStamp() + ".csv"), log);
        }
        //--------------------------------------------------------------------------------
        // Starts backgroundworker.
        private void btnRunMerge_Click(object sender, EventArgs e)
        {

            targetFolder = txtTargetFolder.Text;
            string targetFolderName = Path.GetFileName(targetFolder);
            string siardName = targetFolderName + "_siard";
            siardFolder = Path.Combine(targetFolder, siardName);

            inputFileName = txtInputFile.Text;
            zip64jar = txtZip64Jar.Text;
            
            string inputExt = Path.GetExtension(inputFileName);

            if (inputExt.Equals(".siard"))
            {
                filesAdded = true;

                metadataXmlName = finder.MetaFileFinder(inputFileName, targetFolder, zip64jar);

                DirectoryInfo metaDirInfo = new DirectoryInfo(metadataXmlName);
                string metaGrandParent = metaDirInfo.Parent.Parent.FullName;

                sourceFolder = metaGrandParent;
            }
            else if (Path.GetFileName(inputFileName).Equals("metadata.xml"))
            {
                filesAdded = true;

                metadataXmlName = inputFileName;

                DirectoryInfo metaDirInfo = new DirectoryInfo(inputFileName);
                string metaGrandParent = metaDirInfo.Parent.Parent.FullName;

                sourceFolder = metaGrandParent;
            }
            else
                textBox1.Text = "Please choose a valid file type! (.siard or metadata.xml)";
              

            if (filesAdded)
            {
                Directory.CreateDirectory(siardFolder);

                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.RunWorkerAsync();

            }
        }
        //--------------------------------------------------------------------------------
        // Starts file transfering and file handling.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.ReportProgress(0, "Copying header folder.");
            DirectoryCopy(Path.Combine(sourceFolder, "header"), Path.Combine(siardFolder, "header"));
            backgroundWorker1.ReportProgress(0, "header folder copy complete.");

            List<SiardTableXml> siardListe = finder.TableXMLFinder(metadataXmlName);

            backgroundWorker1.ReportProgress(0, "Creating lobFolder.");
            Directory.CreateDirectory(Path.Combine(siardFolder, finder.LobFolder));

            Console.WriteLine(finder.SiardVersion);
            Console.WriteLine(finder.LobFolder);

            foreach (SiardTableXml table in siardListe)
            {
                inputFileName = table.TableFilePath + @"\" + table.TableFileName + ".xml";

                if (String.IsNullOrEmpty(table.LobPath))
                {
                    DirectoryCopy(table.TableFilePath, Path.Combine(siardFolder, finder.lobFolder, table.TableSchema, table.TableFileName));
                }else
                {
                    Console.WriteLine(table.TableFileName + " " + table.LobPath + " " + table.TableSchema + " " + table.TableFilePath);

                    Directory.CreateDirectory(Path.Combine(siardFolder, finder.lobFolder, table.TableSchema, table.TableFileName));

                    ReadTableXml(table);
                    CreateTableXML(table);

                    backgroundWorker1.ReportProgress(0, "Merging files");

                    TableMerge(table.TableFileName);

                    string tableXSDFilePath = Path.Combine(table.TableFilePath, table.TableFileName + ".xsd");
                    string newXsdFilePath = Path.Combine(siardFolder, finder.lobFolder, table.TableSchema, table.TableFileName, table.TableFileName + ".xsd");

                    File.Copy(tableXSDFilePath, newXsdFilePath);
                }
            }

            if (chkBxMakeSiard.Checked)
            {
                string targetFolderName = Path.GetFileName(targetFolder);
                string zipName = Path.Combine(targetFolder, targetFolderName); 
                zipper.SiardZip(siardFolder, zipName, zip64jar);
            }
        }
        //--------------------------------------------------------------------------------
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //Console.WriteLine("Progress: " + e.ProgressPercentage);
            if(e.UserState != null)
                textBox1.AppendText(e.UserState.ToString() + "\r\n");
        }
        //--------------------------------------------------------------------------------
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.AppendText("\r\nJob complete");
        }

        //--------------------------------------------------------------------------------
        // Resets the form.
        #region Reset
        private void Reset_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            textBox1.Clear();

            sourceFolder = null;
            siardFolder = null;
            csvFileName = null;

            clobs.Clear();

            xmlWriter.Close();
        }
        #endregion

        //--------------------------------------------------------------------------------
        // Opens file dialog. Adds filenames to UI.
        private void button2_Click(object sender, EventArgs e)
        {
            string initFileName;

            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                initFileName = openFileDialog1.FileName;

                ReadInitFile(initFileName);
            }

            textBox1.Clear();

            listBox1.Items.Clear();
            listBox1.Items.Add("csv file: " + csvFileName);

            listBox1.Items.Add("Source folder: " + sourceFolder);
            listBox1.Items.Add("Destination folder: " + siardFolder);
            listBox1.Items.Add("metadata.xml: " + metadataXmlName);

            listBox1.Items.Add(GetTimeStamp());

        }

        //--------------------------------------------------------------------------------
        // Reads the init.txt file and assigns values to the parameters.
        private void ReadInitFile(string initFileName)
        {
            initFolder = Path.GetDirectoryName(initFileName);
            var dic = File.ReadAllLines(initFileName)
                .Select(l => l.Split(new[] { '=' }))
                .ToDictionary(s => s[0].Trim(), s => s[1].Trim());

            sourceFolder = dic["sourcePath"];
            siardFolder = dic["destPath"];
            zip64jar = dic["zip64jar"];


            metadataXmlName = Path.Combine(sourceFolder, @"header\metadata.xml");

            Directory.CreateDirectory(siardFolder);

            filesAdded = true;
        }

        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateTableXML(SiardTableXml table)
        {
            //string tableName = Path.GetFileName(Directory.GetParent(sourceFolder).ToString());

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            xmlWriter = XmlWriter.Create(Path.Combine(siardFolder, finder.LobFolder, table.TableSchema, table.TableFileName, table.TableFileName + ".xml"), xmlWriterSettings);

            xmlWriter.WriteStartDocument();

            string timeStampDate = DateTime.Now.ToShortDateString();
            string timeStampTime = DateTime.Now.ToLongTimeString();

            xmlWriter.WriteComment("Create time: " + timeStampDate + " " + timeStampTime);
            //xmlWriter.WriteComment("Table " + tableSchema + "/" + tableName + " corresponds to actual table " + actualTable);
            xmlWriter.WriteComment(comments[1].Value);

            xmlWriter.WriteStartElement("table", "http://www.bar.admin.ch/xmlns/siard/2/table.xsd");
            xmlWriter.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.bar.admin.ch/xmlns/siard/2/table.xsd table.xsd");
            xmlWriter.WriteAttributeString("version", "2.1");

        }

        //-------------------------------------------------------------------------------
        private void DirectoryCopy(string sourceFolder, string targetPath)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceFolder);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source folder does not exist or could not be found: " + sourceFolder);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            DirectoryInfo targetDir = new DirectoryInfo(targetPath);
            if (dir.Exists)
            {
                // throw new Exception("Target folder already exist: " + targetPath);
            }
           // Console.WriteLine("Creating:" + targetPath);

            Directory.CreateDirectory(targetPath);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(targetPath, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(targetPath, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath);
            }
        }

        //--------------------------------------------------------------------------------
        // Adds info for all table files to the table.xml file.
        private void AddXMLFileInfo(string fileId, string fileName)
        {

            FileInfo fi = new FileInfo(fileName);
            long fileLength = fi.Length;

            xmlWriter.WriteStartElement("row");

            xmlWriter.WriteStartElement("c1");
            xmlWriter.WriteString(fileId);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("c2");
            xmlWriter.WriteString("1");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("c3");
            xmlWriter.WriteString(fileLength.ToString());
            xmlWriter.WriteEndElement();

            string digest = CalculateMD5(fileName);

            string fSegFolder = Path.GetFileName(Path.GetDirectoryName(fileName));
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string fType = Path.GetExtension(fileName);

            xmlWriter.WriteStartElement("c4");
            xmlWriter.WriteAttributeString("file", fSegFolder + "/" + fName + fType);
            xmlWriter.WriteAttributeString("length", fileLength.ToString());
            xmlWriter.WriteAttributeString("digestType", "MD5");
            xmlWriter.WriteAttributeString("digest", digest);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        //--------------------------------------------------------------------------------
        // Calculated MD5 digest of input file.
        static string CalculateMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", String.Empty);
                }
            }
        }
        //--------------------------------------------------------------------------------
        // Returns text in the queried node.
        private string getNodeText(XmlNode table, string query, XmlNamespaceManager nsmgr)
        {
            string varText = "";
            if (table != null)
            {
                XmlNode node = table.SelectSingleNode(query, nsmgr);
                if (node != null)
                {
                    varText = node.InnerText;
                }
            }
            return varText;
        }
        //--------------------------------------------------------------------------------
        // Returns number in the queried node.
        private long getNodeInt(XmlNode table, string query, XmlNamespaceManager nsmgr)
        {
            long varText = 0;
            if (table != null)
            {
                XmlNode node = table.SelectSingleNode(query, nsmgr);
                if (node != null)
                {
                    varText = Convert.ToInt64(node.InnerText);
                }
            }
            return varText;
        }
        //--------------------------------------------------------------------------------
        // Returns attribute text from the queried node.
        private string[] getAttributeText(XmlNode table, string query, XmlNamespaceManager nsmgr)
        {
            string[] varText = { "", "" };
            if (table != null)
            {
                XmlNode node = table.SelectSingleNode(query, nsmgr);
                if (node != null && node.Attributes["file"] != null)
                {
                    varText[0] = node.Attributes["file"].Value;
                    varText[1] = "file";
                }
                else
                {
                    varText[0] = getNodeText(table, query, nsmgr);
                    varText[1] = "clob";
                }
            }
            return varText;
        }
        //--------------------------------------------------------------------------------
        private static String GetTimeStamp(DateTime value)
        {
            return value.ToString("dd.mm.yyyy HH.mm.ss");
        }
        //--------------------------------------------------------------------------------
        // Timestamp of desired format.
        private string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HHmm");
        }
        //--------------------------------------------------------------------------------
        private void btnInputFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                txtInputFile.Text = openFileDialog1.FileName;
        }
        //--------------------------------------------------------------------------------
        private void btnTargetFolder_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                txtTargetFolder.Text = folderBrowserDialog1.SelectedPath;
        }
        //--------------------------------------------------------------------------------
        private void btnZip64Jar_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                txtZip64Jar.Text = openFileDialog1.FileName;
        }
    }

    //=============================================================================================

    public class BinFile
    {
        public string FileId { get; set; }
        public string FileCount { get; set; }
        public string FilePath { get; set; }
    }

    public class Clob
    {
        public string FileId { get; set; }
        public long FileCount { get; set; }
        public long ClobSize { get; set; }
        public string ClobString { get; set; }
        public string ClobType { get; set; }
        public string ClobPath { get; set; }
        public string LobFolder { get; set; }

        public Clob(string fileId, long fileCount, long clobSize, string clobString, string clobtype, string clobPath, string lobFolder)
        {
            FileId = fileId;
            FileCount = fileCount;
            ClobSize = clobSize;
            ClobString = clobString;
            ClobType = clobtype;
            ClobPath = clobPath;
            LobFolder = lobFolder;
        }
    }
}