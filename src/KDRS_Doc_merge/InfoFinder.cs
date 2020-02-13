using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace binFileMerger
{
    public class InfoFinder
    {
        

        SiardZipper unzipper = new SiardZipper();
        public string siardVersion;
        public string lobFolder;

        public string SiardVersion { get => siardVersion; set => siardVersion = value; }
        public string LobFolder { get => lobFolder; set => lobFolder = value; }
        //--------------------------------------------------------------------------------

        public List<SiardTableXml> TableXMLFinder(string metaFilePath)
        {
            List<SiardTableXml> tableList = new List<SiardTableXml>();

            XmlDocument metadataXml = new XmlDocument();

            if (!File.Exists(metaFilePath))
                Console.WriteLine("Cant find " + metaFilePath);
            metadataXml.Load(metaFilePath);

            var nsmgr = new XmlNamespaceManager(metadataXml.NameTable);
            var nameSpace = metadataXml.DocumentElement.NamespaceURI;


            nsmgr.AddNamespace("ns", nameSpace);

            XmlNode root = metadataXml.DocumentElement;

            XmlNodeList tables = root.SelectNodes("descendant::ns:table", nsmgr);

            XmlNodeList liste = root.SelectNodes("descendant::ns:table[ns:columns/ns:column/ns:name='fil']", nsmgr);

            Console.WriteLine("Node list length: " + tables.Count);

            lobFolder = getNodeText(root, "descendant::ns:lobFolder", nsmgr);

            DirectoryInfo metaDirInfo = new DirectoryInfo(metaFilePath);
            String metaGrandParent = metaDirInfo.Parent.Parent.FullName;

            siardVersion = root.Attributes["version"].Value;

            foreach (XmlNode table in tables)
            {
               // Console.WriteLine("Reading node");

                string lobpath = getNodeText(table, "ns:columns/ns:column/ns:lobFolder", nsmgr);
                string schema = table.ParentNode.ParentNode["folder"].InnerText.ToString();
                string tableName = getNodeText(table, "ns:folder", nsmgr);
                string tableNameDb = getNodeText(table, "ns:name", nsmgr);
                long tableRows = getNodeInt(table, "ns:rows", nsmgr);

                // TAA must be changed to (from tableNameDb and tableName)
                //      tableName == database name, f. ex. "files" 
                //      tableFolder == SIARD folder for the table, f. ex. "table2"
                // TAA: Appended tableNameDb as element = the name of the table in the database
                // TAA: Appended tableRows as element = number of rows in the table
                tableList.Add(new SiardTableXml(tableName, lobpath, schema, Path.Combine(metaGrandParent, lobFolder, schema, tableName), tableNameDb, tableRows));
            }
            /*
            foreach (XmlNode node in liste)
            {
                Console.WriteLine("Reading node");
                string lobpath = node.SelectSingleNode("ns:columns/ns:column/ns:lobFolder", nsmgr).InnerText.ToString();
                string schema = node.ParentNode.ParentNode["folder"].InnerText.ToString();
                string tableName = Path.GetFileName(Directory.GetParent(lobpath).ToString());

                tableList.Add(new SiardTableXml(tableName, lobpath, Path.Combine(metaGrandParent, lobFolder, schema, tableName)));
            }
            */
            return tableList;
        }
        //--------------------------------------------------------------------------------

        public string MetaFileFinder(string siardFile, string targetFolder, string jarPath)
        {
            // string unZipFolder = Path.Combine(targetFolder, Path.GetFileNameWithoutExtension(siardFile) + "_unziped");

            unzipper.SiardUnZip(siardFile, targetFolder, jarPath);

            return Path.Combine(targetFolder, "header" , "metadata.xml");
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
    }

    //--------------------------------------------------------------------------------

    public class SiardTableXml
    {
        public SiardTableXml(string tableFileName, string lobPath, string tableSchema, string tableFilePath, string tableNameDb, long tableRows)
        {
            TableNameDb = tableNameDb;
            TableRows = tableRows;
            TableFileName = tableFileName;
            LobPath = lobPath;
            TableSchema = tableSchema;
            this.TableFilePath = tableFilePath;
        }

        public string TableNameDb { get; set; }
        public long TableRows { get; set; }
        public string TableFileName { get; set;}
        public string LobPath { get; set; }
        public string TableSchema { get; set; }
        public string TableFilePath { get; set; }
    }

}
