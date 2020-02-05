﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace binFileMerger
{
    public partial class Form1 : Form
    {

        string csvFileName;
        string destFolder;
        string sourceFolder;
        string initFolder;
        string actualTable;
        string tableXmlName;

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
        // Appends content of inFileName to the end of outFileName.
        #region BinMerge
        private void BinMerge(string inFileName, string outFileName)
        {
            textBox1.AppendText("Merging: " + inFileName + " to: " + outFileName + "\n");

            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(outFileName)))
            {
                writer.Seek(0, SeekOrigin.End);

                using (BinaryReader reader = new BinaryReader(File.OpenRead(inFileName)))
                {
                    byte[] buffer = new byte[1024];

                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        int count = reader.Read(buffer, 0, buffer.Length);
                        writer.Write(buffer, 0, count);
                    }
                }
            }
        }
        #endregion
        //--------------------------------------------------------------------------------
        #region BinMerge2
        private void BinMerge2(List<string> files, string outFileName)
        {
            using (var outputStream = File.Create(outFileName))
            {
                foreach (var file in files)
                {
                    //   Console.WriteLine(file);
                    log.Add(outFileName + ";" + "newSeg" + ";" + "droid" + ";" + files.Count + ";" + "oldSeg" + ";" + file);
                    textBox1.AppendText("Merging2: " + file + " to: " + outFileName + "\n");
                    using (var inputStream = File.OpenRead(file))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }
        #endregion
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
                        //textBox1.AppendText("Merging: clob to: " + outFileName + "\n");
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
                       // textBox1.AppendText("Merging: " + clob.ClobString + " to: " + outFileName + "\n");
                        using (var inputStream = File.OpenRead(Path.Combine(sourceFolder, clob.ClobString)))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }
            }
        }
        //--------------------------------------------------------------------------------
        // Reads the .csv file line by line and merges .bin files with the same file-ID.
        #region RunTableMerge
        private void RunTableMerge()
        {
            string currentLine;
            string fileID;
            string prevFileID;
            int fileCount;

            string newFileName;

            using (StreamReader sr = new StreamReader(csvFileName))
            {
                fileCount = 0;

                currentLine = sr.ReadLine();
                //  Console.WriteLine(currentLine);

                string[] parts = currentLine.Split(';');

                prevFileID = parts[0];
                files.Add(Path.Combine(sourceFolder, parts[2]));
                fileCount++;

                while ((currentLine = sr.ReadLine()) != null)
                {
                    parts = currentLine.Split(';');
                    fileID = parts[0];

                    //Console.WriteLine("FileID: " + fileID + ", file: " + parts[2]);

                    // If a new fileId is encountered -> merge collection of files with same fileID.
                    if (fileID != prevFileID)
                    {
                        newFileName = FileNameChanger(destFolder, files[0], fileCount);

                        BinMerge2(files, newFileName);

                        AddXMLFileInfo(prevFileID, newFileName);

                        fileCount = 0;
                        prevFileID = fileID;
                        files.Clear();
                    }

                    files.Add(Path.Combine(sourceFolder, parts[2]));
                    fileCount++;
                }

                fileID = parts[0];
                newFileName = FileNameChanger(destFolder, files[0], fileCount);
                //textBox1.AppendText("Last fileId \n");

                BinMerge2(files, newFileName);

                AddXMLFileInfo(fileID, newFileName);

                xmlWriter.WriteEndDocument();

                xmlWriter.Close();

                MakeLogFile();
                textBox1.AppendText("Merging complete! \n");
            }
        }
        #endregion
        //--------------------------------------------------------------------------------
        // Traverses the list of clobs from the table.xml file and merges the .bin files and text strings with the same fileID.
        private void TableMerge()
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

            foreach (Clob clob in clobs.Skip(1))
            {
                //Console.WriteLine("PrevFileID: " + prevFileID);

                fileID = clob.FileId;
                // Console.WriteLine("FileID: " + clob.ClobString);

                if (fileID != prevFileID)
                {
                    if (prevClobType == "clob" && clob.FileCount == 1)
                    {
                        string clobFileName = "clobRec" + clobCount + ".bin";
                        newFileName = FileNameChanger(destFolder, clobFileName, fileCount);
                        BinMergeClob(mergeClobs, newFileName);
                        clobCount++;

                        AddXMLFileInfo(prevFileID, newFileName);
                        counter++;
                    }
                    else
                    {
                        newFileName = FileNameChanger(destFolder, mergeClobs[0].ClobString, fileCount);
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

               // textBox1.AppendText("FileId :" + clob.FileId);

                if (clob == clobs.Last())
                {
                    if (clob.ClobType == "clob" && clob.FileCount == 1)
                    {
                        string clobFileName = "clobRec" + clobCount + ".bin";
                        newFileName = FileNameChanger(destFolder, clobFileName, fileCount);
                        BinMergeClob(mergeClobs, newFileName);
                        clobCount++;

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                    }
                    else
                    {
                        newFileName = FileNameChanger(destFolder, mergeClobs[0].ClobString, fileCount);
                        BinMergeClob(mergeClobs, newFileName);

                        AddXMLFileInfo(fileID, newFileName);
                        counter++;
                    }
                }
                progress = counter * 100 / clobTotal;
                backgroundWorker1.ReportProgress(progress);
            }

            xmlWriter.WriteComment("Row count: " + counter);
            xmlWriter.WriteComment("Finshed at: " + GetTimeStamp(DateTime.Now));

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            MakeLogFile();

            backgroundWorker1.ReportProgress(progress, "Merging complete!");
        }
        //--------------------------------------------------------------------------------
        // Reads the chosen table.xml and put all info in a list of clobs.
        private void ReadTableXml()
        {
            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.IgnoreComments = false;
            XmlReader xmlReader = XmlReader.Create(tableXmlName, xmlReaderSettings);

            Console.WriteLine("xml file: " + tableXmlName);

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
                    clobs.Add(new Clob(fileId, fileCount, clobSize, clobString[0], clobString[1]));
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

            /*if (InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                }));
                return;
            }*/
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
                return Path.Combine(folder, String.Concat(fName, '_', fileCounter, fExt));
            }
            else
            {
                return Path.Combine(folder, newSegFolder, String.Concat(fName, '_', fileCounter, fExt));
            }
        }
        #endregion

        //--------------------------------------------------------------------------------
        // Prints the log list to a .csv file.
        public void MakeLogFile()
        {
            File.WriteAllLines(Path.Combine(Directory.GetParent(destFolder).ToString(), "filelist_" + GetTimeStamp() + ".csv"), log);
        }

        //--------------------------------------------------------------------------------
        // Starts file merging.
        private void button1_Click(object sender, EventArgs e)
        {
            if (filesAdded)
            {
                backgroundWorker1 = new BackgroundWorker();
                backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
                backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
                backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                backgroundWorker1.WorkerReportsProgress = true;
                backgroundWorker1.RunWorkerAsync();

            }
        }
        //--------------------------------------------------------------------------------

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            /*List<SiardTable> siardListe = finder.TableXMLFinder(@"D:\arkiv-work\1529\1529_6_E-1529-2018-0007\content\sip\content\unziped\header\metadata.xml");

            Console.WriteLine(finder.SiardVersion);
            tableXmlName = siardListe[0].FilePath + @"\" + siardListe[0].TableFileName + ".xml";*/
            // Console.WriteLine(table.TableFileName + " " + table.LobPath + " " + table.FilePath + @"\" + table.TableFileName +".xml");

            // RunTableMerge();
             ReadTableXml();

            CreateTableXML();

            //textBox1.AppendText("Merging files");

            TableMerge();
            //zipper.siardZip(destFolder, destFolder);

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            //Console.WriteLine("Progress: " + e.ProgressPercentage);
            if(e.UserState != null)
                textBox1.AppendText(e.UserState.ToString() + "\r\n");
        }

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
            destFolder = null;
            csvFileName = null;

            clobs.Clear();

            xmlWriter.Close();
        }
        #endregion

        //--------------------------------------------------------------------------------
        #region Form_Load
        private void Form1_Load(object sender, EventArgs e)
        {

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
            listBox1.Items.Add("Destination folder: " + destFolder);

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
            destFolder = dic["destPath"];
            csvFileName = dic["csvFilePath"];
            actualTable = dic["actualTable"];
            tableXmlName = dic["tableXml"];

            Directory.CreateDirectory(destFolder);

            filesAdded = true;
        }

        //--------------------------------------------------------------------------------
        // Creates table.xml file containing information about all the table files.
        private void CreateTableXML()
        {
            string tableName = Path.GetFileName(Directory.GetParent(sourceFolder).ToString());
            string tableSchema = Path.GetFileName(Directory.GetParent(Directory.GetParent(sourceFolder).ToString()).ToString());

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            xmlWriter = XmlWriter.Create(Path.Combine(Directory.GetParent(destFolder).ToString(), tableName + "merged_test.xml"), xmlWriterSettings);

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

        public Clob(string fileId, long fileCount, long clobSize, string clobString, string clobtype)
        {
            FileId = fileId;
            FileCount = fileCount;
            ClobSize = clobSize;
            ClobString = clobString;
            ClobType = clobtype;
        }
    }
}
