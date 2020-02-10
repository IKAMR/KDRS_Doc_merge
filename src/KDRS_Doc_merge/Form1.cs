using System;
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
        string siardFolderOutput;
        string sourceFolder;
        string initFolder;
        string zip64jar;
        string inputFileName;
        string metadataXmlName = String.Empty;

        Boolean filesAdded = false;
        Boolean useSeg = true;

        XmlWriter xmlWriter;

        List<Lob> lobs = new List<Lob>();
        XmlNodeList comments;

        List<string> files = new List<string>();
        List<string> log = new List<string>();

        SiardZipper zipper = new SiardZipper();
        InfoFinder finder = new InfoFinder();

        public Form1()
        {
            InitializeComponent();

            Text = Globals.toolName + " v" + Globals.toolVersion;
        }

        //--------------------------------------------------------------------------------
        // Merges contents of the input list to a file with name 'outFileName'.
        private void BinMergeLob(List<Lob> lobsToMerge, string outFileName)
        {
            using (var outputStream = File.Create(outFileName))
            {
                foreach (Lob lob in lobsToMerge)
                {
                    if (lob.LobType == "lob")
                    {
                        log.Add(lob.FileId + ";" + outFileName + ";" + "newSeg" + ";" + "droid" + ";" + lobs.Count + ";" + "oldSeg" + ";" + lob.LobString);
                        byte[] byteArray = Encoding.ASCII.GetBytes(lob.LobString);
                        using (var inputstream = new MemoryStream(byteArray))
                        {
                            inputstream.CopyTo(outputStream);
                        }
                    }
                    else
                    {
                        log.Add(lob.FileId + ";" + outFileName + ";" + "newSeg" + ";" + "droid" + ";" + lobs.Count + ";" + "oldSeg" + ";" + lob.LobString);
                        using (var inputStream = File.OpenRead(lob.LobPath))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------
        // Traverses the list of lobs from the table.xml file and merges the .bin files and text strings with the same fileID.
        private void TableMerge(string tableName)
        {
            string fileID;
            string prevFileID;
            string prevLobType;
            long prevFileCount;

            int fileCount = 0;
            string newFileName;
            // int countFileInline = 0;
            // int countFileLob = 0;
            int debugCountFilesMax = 500;


            List<Lob> mergeLobs = new List<Lob>();

            prevFileID = lobs[0].FileId;
            prevLobType = lobs[0].LobType;
            prevFileCount = lobs[0].FileCount;
            mergeLobs.Add(lobs[0]);
            fileCount++;

            int counter = 0;

            int lobCount = 0;
            int lobTotal = lobs.Count-2;
            int progress = 0;
            int progressCount = 0;

            foreach (Lob lob in lobs.Skip(1))
            {

                fileID = lob.FileId;

                if (fileID != prevFileID)
                {
                    if (prevLobType == "lob" && lob.FileCount == 1)
                    {
                        string lobFileName = Path.Combine("seg", "lobRec" + lobCount + ".bin");
                        newFileName = FileNameChanger(lob.LobFolder, lobFileName, fileCount);
                        BinMergeLob(mergeLobs, newFileName);
                        lobCount++;

                        AddXMLFileInfo(prevFileID, newFileName);
                        counter++;
                    }
                    else
                    {
                        newFileName = FileNameChanger(lob.LobFolder, mergeLobs[0].LobString, fileCount);
                        BinMergeLob(mergeLobs, newFileName);

                        AddXMLFileInfo(prevFileID, newFileName);
                        counter++;
                    }

                    mergeLobs.Clear();
                    fileCount = 0;
                    prevFileID = lob.FileId;
                    prevLobType = lob.LobType;
                    prevFileCount = lob.FileCount;

                }
                mergeLobs.Add(lob);
                fileCount++;

                fileID = lob.FileId;

                if (lob == lobs.Last())
                {
                    if (lob.LobType == "lob" && lob.FileCount == 1)
                    {
                        // inline xml data
                        string lobFileName = Path.Combine("seg", "lobRec" + lobCount + ".bin");
                        newFileName = FileNameChanger(lob.LobFolder, lobFileName, fileCount);
                        BinMergeLob(mergeLobs, newFileName);
                        lobCount++;

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                        // countFileInline++;                        
                    }
                    else
                    {
                        // file referred by attribute
                        newFileName = FileNameChanger(lob.LobFolder, mergeLobs[0].LobString, fileCount);
                        BinMergeLob(mergeLobs, newFileName);

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                        // countFileLob++;
                    }
                }
                progressCount++;
                progress = progressCount * 100 / lobTotal ;

                backgroundWorker1.ReportProgress(progress);

                // Debug
                if (debugCountFilesMax >= counter)
                    break;
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finshed at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            UpdateTableRows(tableName, counter);

            MakeLogFile(tableName);

            // backgroundWorker1.ReportProgress(progress, ", merged into " + counter + " files from " + prevFileCount + " rows");
            backgroundWorker1.ReportProgress(progress, ", merged into " + counter + " files");
            // backgroundWorker1.ReportProgress(progress, ", merged into " + counter + " (" + countFileInline + " from inline, " + countFileLob + " lob)");
        }

        //--------------------------------------------------------------------------------
        private void UpdateTableRows(string table, int rows)
        {
            XmlDocument metadata = new XmlDocument();
            string newMetadataPath = Path.Combine(siardFolderOutput, "header", "metadata.xml");
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
        // Reads the chosen table.xml and put all info in a list of lobs.
        private void ReadTableXml(SiardTableXml lobTable)
        {
            string contentPath = Directory.GetParent(Path.GetDirectoryName(lobTable.TableFilePath)).ToString();
            string lobRootPath = Path.Combine(contentPath, lobTable.LobPath);

            lobs.Clear();

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreComments = false;
            XmlReader xmlReader = XmlReader.Create(inputFileName, xmlReaderSettings);

            Console.WriteLine("xml file: " + inputFileName);
            Console.WriteLine("lob root path: " + lobRootPath);

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
                    long lobSize = getNodeInt(row, "descendant::ns:c3", nsmgr);
                    string[] lobString = getAttributeText(row, "descendant::ns:c4", nsmgr);

                    string lobPath = Path.Combine(siardFolderOutput, finder.lobFolder, lobTable.LobPath);
                    lobs.Add(new Lob(fileId, fileCount, lobSize, lobString[0], lobString[1], Path.Combine(lobRootPath, lobString[0]), lobPath));
                }
                rowCount++;
                progress = rowCount * 100 / rowTotal;
                backgroundWorker1.ReportProgress(progress);
            }

            lobs = lobs.OrderBy(o => o.FileId).ThenBy(s => s.FileCount).ToList();

            comments = tableXml.SelectNodes("//comment()", nsmgr);

            backgroundWorker1.ReportProgress(progress, ", table loaded");
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
            File.WriteAllLines(Path.Combine(Directory.GetParent(siardFolderOutput).ToString(), "filelist_" + table + "_" +  GetTimeStamp() + ".csv"), log);
        }
        //--------------------------------------------------------------------------------
        // Starts backgroundworker.
        private void btnRunMerge_Click(object sender, EventArgs e)
        {
            Globals.testMode = false;
            textBox1.Text = "Start file merge";
            initDocMerge();
        }

        //--------------------------------------------------------------------------------
        private void initDocMerge()
        {
            targetFolder = txtTargetFolder.Text;
            string siardNameOutput = "siard_structure_output";
            siardFolderOutput = Path.Combine(targetFolder, siardNameOutput);

            inputFileName = txtInputFile.Text;
            zip64jar = txtZip64Jar.Text;

            string inputExt = Path.GetExtension(inputFileName);

            if (inputExt.Equals(".siard"))
            {
                if (File.Exists(zip64jar))
                {
                    textBox1.AppendText("\r\nUnzipping .siard file");
                    filesAdded = true;

                    string siardNameInput = "siard_structure_input";
                    string siardFolderInput = Path.Combine(targetFolder, siardNameInput);
                    metadataXmlName = finder.MetaFileFinder(inputFileName, siardFolderInput, zip64jar);

                    DirectoryInfo metaDirInfo = new DirectoryInfo(metadataXmlName);
                    string metaGrandParent = metaDirInfo.Parent.Parent.FullName;

                    sourceFolder = metaGrandParent;
                }
                else textBox1.AppendText("\r\nA .siard file needs zip64.jar file selected");
            }
            else if (Path.GetFileName(inputFileName).Equals("metadata.xml"))
            {
                textBox1.AppendText("\r\nReading unzipped metadata.xml");                
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
                if (!Globals.testMode)
                    Directory.CreateDirectory(siardFolderOutput);

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
            bool doMergeFiles;
            bool hasLob;
            string tempString;
            string tempString2;
            int countTableStd = 0;
            int countTableLob = 0;

            if (!Globals.testMode)
            {
                backgroundWorker1.ReportProgress(0, "\r\n" + @"Copying \header\metadata.xml");
                DirectoryCopy(Path.Combine(sourceFolder, "header"), Path.Combine(siardFolderOutput, "header"));
            }

            List<SiardTableXml> siardListe = finder.TableXMLFinder(metadataXmlName);

            if (!Globals.testMode)
            {
                backgroundWorker1.ReportProgress(0, "\r\nCreating root lobFolder: " + finder.LobFolder);
                Directory.CreateDirectory(Path.Combine(siardFolderOutput, finder.LobFolder));
            }

            Console.WriteLine("SIARD version: " + finder.SiardVersion);
            Console.WriteLine("Root lobFolder: " + finder.LobFolder);
            backgroundWorker1.ReportProgress(0, "\r\nSIARD version: " + finder.SiardVersion);
            backgroundWorker1.ReportProgress(0, "\r\n\r\nTables:\r\n");

            foreach (SiardTableXml table in siardListe)
            {
                doMergeFiles = false;
                hasLob = false;
                inputFileName = table.TableFilePath + @"\" + table.TableFileName + ".xml";

                if (!String.IsNullOrEmpty(table.LobPath))
                {
                    hasLob = true;
                    Console.WriteLine("LOB: " + table.TableNameDb +" | "+ table.TableNameDb);
                    tempString = table.TableNameDb;
                    tempString = tempString.ToUpper();                    
                    Console.WriteLine("tempString: " + tempString);
                    tempString2 = tempString.Substring(0, 9);
                    Console.WriteLine("tempString2: " + tempString2);
                    if (String.Equals("POSTKASSE", tempString))
                    {
                        Console.WriteLine("Match POSTKASSE: " + table.TableNameDb);
                        doMergeFiles = true;
                    }
                    else if (String.Equals("DGDOKLAGER", tempString.Substring(0, 10)))
                    {
                        Console.WriteLine("Match DGDOKLAGERn: " + table.TableNameDb);
                        doMergeFiles = true;
                    }
                }

                if (!doMergeFiles)
                {
                    countTableStd++;
                    Console.WriteLine("Copy table: " + table.TableFileName  +" | "+ table.TableNameDb);
                    backgroundWorker1.ReportProgress(0, "\r\n" + table.TableFileName + " | " + table.TableNameDb);
                    if (hasLob)
                        backgroundWorker1.ReportProgress(0, " | LOB");
                    else
                        backgroundWorker1.ReportProgress(0, " |");
                    if (!Globals.testMode)
                    {
                        DirectoryCopy(table.TableFilePath, Path.Combine(siardFolderOutput, finder.lobFolder, table.TableSchema, table.TableFileName));
                        backgroundWorker1.ReportProgress(0, " copied");
                    }
                }
                else
                {
                    countTableLob++;
                    Console.WriteLine("LOB table: " + table.TableNameDb);
                    Console.WriteLine(table.TableFileName + " " + table.LobPath + " " + table.TableSchema + " " + table.TableFilePath);
                    backgroundWorker1.ReportProgress(0, "\r\n" + table.TableFileName + " | " + table.TableNameDb + " | LOB");
                    if (!Globals.testMode)
                    {
                        Directory.CreateDirectory(Path.Combine(siardFolderOutput, finder.lobFolder, table.TableSchema, table.TableFileName));

                        ReadTableXml(table);
                        CreateTableXML(table);

                        TableMerge(table.TableFileName);

                        string tableXSDFilePath = Path.Combine(table.TableFilePath, table.TableFileName + ".xsd");
                        string newXsdFilePath = Path.Combine(siardFolderOutput, finder.lobFolder, table.TableSchema, table.TableFileName, table.TableFileName + ".xsd");

                        File.Copy(tableXSDFilePath, newXsdFilePath);
                        // backgroundWorker1.ReportProgress(0, " merged");
                    }
                }
            }
            backgroundWorker1.ReportProgress(0, "\r\n\r\nStandard tables  = " + countTableStd);
            backgroundWorker1.ReportProgress(0, "\r\nLOB tables = " + countTableLob);
            backgroundWorker1.ReportProgress(0, "\r\nTotal number of tables = " + (countTableStd + countTableLob));

            if (chkBxMakeSiard.Checked)
            {
                string targetFolderName = Path.GetFileName(targetFolder);
                string zipName = Path.Combine(targetFolder, targetFolderName); 
                zipper.SiardZip(siardFolderOutput, zipName, zip64jar);
            }
        }
        //--------------------------------------------------------------------------------
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //Console.WriteLine("Progress: " + e.ProgressPercentage);
            if (e.UserState != null)
            {
                textBox1.AppendText(e.UserState.ToString());
            }
        }
        //--------------------------------------------------------------------------------
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox1.AppendText("\r\n\r\nJob complete");

            // Save logfile
            string logFile = Path.Combine(targetFolder, "kdrs-doc-merge_log_" + DateTime.Now.ToString("yyyy-MM-dd-HHmm") + ".txt");
            System.IO.File.AppendAllText(logFile, textBox1.Text);
        }

        //--------------------------------------------------------------------------------
        // Resets the form.
        #region Reset
        private void Reset_Click(object sender, EventArgs e)
        {
            textBox1.Clear();

            sourceFolder = null;
            siardFolderOutput = null;
            csvFileName = null;

            lobs.Clear();

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

            /* listBox1.Items.Clear();
            listBox1.Items.Add("csv file: " + csvFileName);

            listBox1.Items.Add("Source folder: " + sourceFolder);
            listBox1.Items.Add("Destination folder: " + siardFolderOutput);
            listBox1.Items.Add("metadata.xml: " + metadataXmlName);

            listBox1.Items.Add(GetTimeStamp()); */
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
            siardFolderOutput = dic["destPath"];
            zip64jar = dic["zip64jar"];


            metadataXmlName = Path.Combine(sourceFolder, @"header\metadata.xml");

            Directory.CreateDirectory(siardFolderOutput);

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

            xmlWriter = XmlWriter.Create(Path.Combine(siardFolderOutput, finder.LobFolder, table.TableSchema, table.TableFileName, table.TableFileName + ".xml"), xmlWriterSettings);

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
                    varText[1] = "lob";
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

        //--------------------------------------------------------------------------------
        private void btnTest_Click(object sender, EventArgs e)
        {
            Globals.testMode = true;
            textBox1.Text = "Start testmode";
            initDocMerge();
        }
    }

    //=============================================================================================

    public static class Globals
    {
        public static readonly String toolName = "KDRS Doc merge";
        public static readonly String toolVersion = "0.3.1";

        public static int countFiles = 0;
        public static bool testMode = false;
    }
    public class BinFile
    {
        public string FileId { get; set; }
        public string FileCount { get; set; }
        public string FilePath { get; set; }
    }

    public class Lob
    {
        public string FileId { get; set; }
        public long FileCount { get; set; }
        public long LobSize { get; set; }
        public string LobString { get; set; }
        public string LobType { get; set; }
        public string LobPath { get; set; }
        public string LobFolder { get; set; }

        public Lob(string fileId, long fileCount, long lobSize, string lobString, string lobtype, string lobPath, string lobFolder)
        {
            FileId = fileId;
            FileCount = fileCount;
            LobSize = lobSize;
            LobString = lobString;
            LobType = lobtype;
            LobPath = lobPath;
            LobFolder = lobFolder;
        }
    }
}
