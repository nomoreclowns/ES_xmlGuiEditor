using System;
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

        public XElement open()
        {
            try
            {
                //load xml file as a single element representing the root node
                XElement xmlRootNode = XDocument.Load(handledDirectory+handledFileName).Element(settings.xmlRootElementName);

                // An object to enumerate over the XML nodes
                IEnumerable<XElement> xmlElements = (from c in xmlRootNode.Elements() select c);

                return xmlRootNode;

            }
            catch
            {
                return null;
            }
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
