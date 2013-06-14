using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES_XML_Editor
{
    public class FileHandler
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings settings = ProgramSettings.Default;

        private String handledDirectory;

        private String handledFileName;

        private String pFilePath;

        //public String fileDirectory
        //{
        //    get
        //    {
        //        return handledDirectory;
        //    }
        //}

        public String fileName
        {
            get
            {
                return handledFileName;
            }
        }

        private xmlDoc xmlFile;



        public FileHandler(String tempFileDirectory, String tempFileName)
        {
            pFilePath = tempFileDirectory + tempFileName;
            handledFileName = tempFileName;

        }

        public FileHandler(String tempFullFilePath)
        {
            pFilePath = tempFullFilePath;

        }

        public xmlElem open()
        {
            try
            {
                //load xml file as a single element representing the root node
                XDocument temp = new XDocument(XDocument.Load(pFilePath));

                xmlFile = new xmlDoc(temp);

                return new xmlElem(xmlFile.root);
            }
            catch
            {
                return null;
            }
        }

        public void setFileContent(xmlElem content)
        {
            xmlFile.root = content;
        }

        public void save()
        {
            File.WriteAllText((pFilePath), xmlFile.ToString(), System.Text.Encoding.UTF8);
        }

        //public void save(XElement data)
        //{
        //    String temp = handledDirectory + handledFileName;

        //    if (File.Exists(pFilePath))
        //    {
        //        data.Save(pFilePath);
        //    }
        //}

        //public void save(xmlDoc data)
        //{
        //    File.WriteAllText((handledDirectory + handledFileName), data.ToString(), System.Text.Encoding.UTF8);
        //}

        //public void save(xmlElement data)
        //{
        //    String temp = handledDirectory + handledFileName;

        //    if (File.Exists(pFilePath))
        //    {
        //        data.Save(pFilePath);
        //    }
        //}

    }
}
