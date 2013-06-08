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

        public FileHandler(String tempFileDirectory, String tempFileName)
        {
            handledDirectory = tempFileDirectory;
            handledFileName = tempFileName;

        }

        public String fileDirectory
        {
            get
            {
                return handledDirectory;
            }
        }

        public String fileName
        {
            get
            {
                return handledFileName;
            }
        }

        private xmlDoc xmlFile;

        public xmlElem open()
        {
            try
            {
                //load xml file as a single element representing the root node
                //XElement xmlRootNode = XDocument.Load(handledDirectory+handledFileName).Element(settings.xmlRootElementName);

                XDocument temp= new XDocument(XDocument.Load(handledDirectory + handledFileName));

                //MessageBox.Show(temp.ToString(), "Error!", MessageBoxButton.OK);
                
                xmlFile = new xmlDoc(temp);


                // An object to enumerate over the XML nodes
                //IEnumerable<XElement> xmlElements = (from c in xmlFile.root.Elements() select c);

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

        public void save(XElement data)
        {
            String temp = handledDirectory + handledFileName;

            if (File.Exists(temp))
            {
                data.Save(temp);
            }
        }

        public void save(xmlDoc data)
        {
            File.WriteAllText((handledDirectory + handledFileName), data.ToString(), System.Text.Encoding.UTF8);
        }

        public void save()
        {
            File.WriteAllText((handledDirectory + handledFileName), xmlFile.ToString(), System.Text.Encoding.UTF8);
        }

        public void save(xmlElement data)
        {
            String temp = handledDirectory + handledFileName;

            if (File.Exists(temp))
            {
                data.Save(temp);
            }
        }

    }
}
