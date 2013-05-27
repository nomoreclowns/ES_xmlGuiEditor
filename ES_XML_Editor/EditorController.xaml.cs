using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using Microsoft.Win32;
using System.Timers;


namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for EditorController.xaml
    /// </summary>
    public partial class EditorController : Window
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings settings = ProgramSettings.Default;

        // list that stores the contents of a file
        private List<Object> dataList; 

        // String to hold the directory for program files on the users computer
        String ProgramFolder;

        // Manager for the users' settings file
        private KeyValuePairDataBase settingsFile;

        // String to hold the users' last visited directory when using this program.
        // Eventually gets stored as a setting in a file
        private String directoryLastUsed;

        //instance of the base window
        MainWindow baseWindowObject;

        //default constructor
        public EditorController()
        {
            InitializeComponent();

            ProgramFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), settings.progName);

            String settingsPath = System.IO.Path.Combine(ProgramFolder, "settings.xml");

            retrieveSettingsFile(settingsPath);

            directoryLastUsed = settingsFile.getValue(EditorSettings.lastUsedDirectory.ToString());

            if (directoryLastUsed == null)
            {
                directoryLastUsed= Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            }

            baseWindowObject = new MainWindow(this);
            baseWindowObject.Show();
            
        }

        #region publicFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void openFile(out String fileName)
        {
            fileName = openFileChooser();

            FileHandler someFile = new FileHandler(fileName);
            dataList = someFile.open();

        }

        //public void openFile()
        //{
        //    String fileName = openFileChooser();

        //    FileHandler someFile = new FileHandler(fileName);
        //    dataList = someFile.open();

        //}

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void showErrorMessage(String messageOfDoom)
        {
            MessageBox.Show(this, messageOfDoom, "Error!", MessageBoxButton.OK);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void closeProgram()
        {
            try
            {
                this.Close();
            }
            catch (InvalidOperationException)
            {
                showErrorMessage("Invalid operation exception caught in function EditorController.closeProgram().");
            }
        }

        #endregion

        #region protectedFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected String openFileChooser()
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Multiselect = false;
            fileChooser.Filter = "XML|*.xml;";
            fileChooser.InitialDirectory = @directoryLastUsed;

            if (fileChooser.ShowDialog(this) == true)
            {
                return fileChooser.FileName;
            }
            return null;
        }


        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveSettingsFile(String filePath)
        {
            try
            {
                settingsFile = new KeyValuePairDataBase(filePath);
            }
            catch
            {
                settingsFile= new KeyValuePairDataBase();
            }
        }

        #endregion

        //public String findSetting(EditorSettings setting)
        //{
        //    String returnableSetting = null;

        //    switch (setting)
        //    {
        //        case EditorSettings.lastUsedDirectory:
        //            returnableSetting = settingsFile.getValue(setting.ToString());
        //            break;
        //    }
        //    return returnableSetting;
        //}

        //public void setSetting(EditorSettings setting, String settingValue)
        //{
        //    settingsFile.setKeyValuePair(setting.ToString(), settingValue);
        //}


    }

    //class for handling the user's settings file
    public class SettingsManager
    {

    }


    public class FileHandler
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings settings = ProgramSettings.Default;

        private String handledFile;

        private String ext;

        public FileHandler()
        {
            handledFile= null;
        }

        public FileHandler(String tempFileName)
        {
            handledFile= tempFileName;
        }

        public String fileName
        {
            set
            {
                handledFile = value;
            }
            get
            {
                return handledFile;
            }
        }

        public List<Object> open()
        {
            ;
            return null;
        }

        public List<Object> open(String tempFileName)
        {
            handledFile = tempFileName;
            return open();
        }

        private List<Object> parseXml()
        {
            List<Object> returningList=null;

            try
            {
                //load xml file as a single element representing the root node
                XElement xmlRootNode = XDocument.Load(handledFile).Element(settings.xmlRootElementName);

                // An object to enumerate over the XML nodes
                IEnumerable<XElement> xmlElements = (from c in xmlRootNode.Elements() select c);

                switch (handledFile)
                {
                    case "WeaponModule.xml":
                        //zztoWeaponModule(out returningList, xmlElements);
                        //return returningList;

                        break;

                        
                }
                return returningList;

            }
            catch
            {
                return null;
            }
            //return null;
        }

        private void toWeaponModule(List<Object> workingList, IEnumerable<XElement> nodes)
        {
            ;
        }


        private void zztoWeaponModule(out List<Object> workingList, IEnumerable<XElement> nodes)
        {
            workingList=null;

            zzWeaponModule someModule;

            foreach (XElement elementNode in nodes)
            {
                someModule = new zzWeaponModule();
                someModule.name = elementNode.Attribute("Name").Value;
                someModule.cost = elementNode.Attribute("Cost").Value;
                someModule.weight = elementNode.Attribute("Weight").Value;
                someModule.militaryPower = elementNode.Attribute("MilitaryPower").Value;

                XElement childElement = elementNode.Element("Simulation");

                someModule.damageMin = childElement.Attribute("DamageMin").Value;
                someModule.damageMax = childElement.Attribute("DamageMax").Value;
                someModule.criticMultiplier = childElement.Attribute("CriticMultiplier").Value;
                someModule.criticChance = childElement.Attribute("CriticChance").Value;
                someModule.interceptionEvasion = childElement.Attribute("InterceptionEvasion").Value;
                someModule.numberPerSalve = childElement.Attribute("NumberPerSalve").Value;
                someModule.turnBeforeReach = childElement.Attribute("TurnBeforeReach").Value;
                someModule.turnToReload = childElement.Attribute("TurnToReload").Value;
                someModule.weaponClass = childElement.Element("WeaponClass").Value;

                childElement = elementNode.Element("Reflection");

                someModule.speed = childElement.Attribute("Speed").Value;
                someModule.priority = childElement.Attribute("Priority").Value;
                someModule.boardSideMaxDuration = childElement.Attribute("BoardSideMaxDuration").Value;
                someModule.boardSideFireDelay = childElement.Attribute("BoardSideFireDelay").Value;
                someModule.projectilesPrefabs = childElement.Element("Projectiles-Prefabs").Element("Prefab").Attribute("Path").Value;

                childElement = elementNode.Element("Gui");

                someModule.title = childElement.Element("Title").Value;
                someModule.description = childElement.Element("Description").Value;
                someModule.icon = childElement.Element("Icon").Attribute("Small").Value;
                //UNFINISHED CLASS! MISSING  ICON ATTRIBUTE "LARGE"
                someModule.charCode = childElement.Element("CharCode").Value;

            }
        }
    }


    public enum EditorSettings
    {
        lastUsedDirectory
    }
}
