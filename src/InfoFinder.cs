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
        string siardVersion;

        public string SiardVersion { get => siardVersion; set => siardVersion = value; }

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

            XmlNodeList liste = root.SelectNodes("descendant::ns:table[ns:columns/ns:column/ns:name='fil']", nsmgr);

            Console.WriteLine("Node list length: " + liste.Count);

            DirectoryInfo metaDirInfo = new DirectoryInfo(metaFilePath);
            String metaGrandParent = metaDirInfo.Parent.Parent.FullName;

            siardVersion = root.Attributes["version"].Value;

            foreach (XmlNode node in liste)
            {
                Console.WriteLine("Reading node");
                string lobpath = node.SelectSingleNode("ns:columns/ns:column/ns:lobFolder", nsmgr).InnerText.ToString();
                string schema = node.ParentNode.ParentNode["folder"].InnerText.ToString();
                string tableName = Path.GetFileName(Directory.GetParent(lobpath).ToString());

                tableList.Add(new SiardTableXml(tableName, lobpath, Path.Combine(metaGrandParent,"content", schema, tableName)));
            }

            return tableList;
        }

        private string MetaFileFinder(string siardFile)
        {
            string unZipFolder = Path.GetDirectoryName(siardFile) + "_unziped";

            unzipper.SiardUnZip(siardFile, unZipFolder);

            return unZipFolder + @"\header\metadata.xml";
        }

    }

    public class SiardTableXml
    {
        public SiardTableXml(string tableFileName, string lobPath, string filePath)
        {
            TableFileName = tableFileName;
            LobPath = lobPath;
            this.FilePath = filePath;
        }

        public string TableFileName { get; set;}
        public string LobPath { get; set; }
        public string FilePath { get; set; }
    }

}
