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
using System.Windows.Controls;


namespace ES_XML_Editor
{
    public delegate void controllerOpenFile(out String delFileName, out String delFullFilePath);

    public delegate void controllerBind(ref ListBox delListBox);

    public delegate void controllerShowError(String delMessageOfDoom);

    public delegate void controllerSave();

    public delegate void controllerClose();

    public delegate void controllerErrorCallback();

    public class controllerDelegateContainer
    {
        private controllerOpenFile pFileOpener;
        private controllerBind pBinder;
        private controllerShowError pErrorDisplayer;
        private controllerSave pFileSaver;
        private controllerClose pProgramCloser;
        private controllerErrorCallback pAccessViolationInformer;

        public controllerOpenFile fileOpener
        {
            get
            {
                return pFileOpener;
            }
        }

        public controllerBind binder
        {
            get
            {
                return pBinder;
            }
        }

        public controllerShowError errorDisplayer
        {
            get
            {
                return pErrorDisplayer;
            }
        }

        public controllerErrorCallback accessViolationInformer
        {
            get
            {
                return pAccessViolationInformer;
            }
        }

        public controllerSave fileSaver
        {
            get
            {
                return pFileSaver;
            }
        }

        public controllerClose programCloser
        {
            get
            {
                return pProgramCloser;
            }
        }

        public controllerDelegateContainer(controllerShowError iErrorDisplayer, controllerErrorCallback iAccessViolationInformer, controllerOpenFile iFileOpener = null, controllerBind iBinder = null, controllerSave iFileSaver = null, controllerClose iProgramCloser = null)
        {
            pErrorDisplayer = iErrorDisplayer;
            pFileOpener = iFileOpener;
            pBinder = iBinder;
            pFileSaver = iFileSaver;
            pProgramCloser = iProgramCloser;
            pAccessViolationInformer = iAccessViolationInformer;
        }

        public void unauthorizedAccess()
        {
            ;
        }

        public void retrieveDelegates(out controllerShowError iErrorDisplayer, out controllerOpenFile iFileOpener, out controllerBind iBinder, out controllerSave iFileSaver, out controllerClose iProgramCloser)
        {
            iErrorDisplayer = pErrorDisplayer;
            iFileOpener = pFileOpener;
            iBinder = pBinder;
            iFileSaver = pFileSaver;
            iProgramCloser = pProgramCloser;
        }

    }


    /// <summary>
    /// Interaction logic for EditorController.xaml
    /// </summary>
    public partial class EditorController : Window
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings settings = ProgramSettings.Default;

        // list that stores the contents of a file
        private XElement dataList;

        public IEnumerable<XElement> bindableList
        {
            get
            {
                return null;
            }
        }

        // String to hold the directory for program files on the users computer
        String ProgramFolder;

        // Manager for the users' settings file
        private KeyValuePairDataBase settingsFile;

        // String to hold the users' last visited directory when using this program.
        // Eventually gets stored as a setting in a file
        private String directoryLastUsed;

        //instance of the base window
        private MainWindow baseWindowObject;


        //default constructor
        public EditorController()
        {
            InitializeComponent();

            ProgramFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), settings.progName);

            retrieveSettingsFile(ProgramFolder);

            directoryLastUsed = settingsFile.getValue(EditorSettings.lastUsedDirectory.ToString());

            
            if (Directory.Exists(directoryLastUsed) == false)
            {
                directoryLastUsed= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            baseWindowObject = new MainWindow(new controllerDelegateContainer(showErrorMessage, delegateContainerErrorFunction, openFile, bindFunction, saveData, closeProgram));
            baseWindowObject.Show();
        }

        #region publicFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void openFile(out String fileName, out String fullFilePath)
        {
            OpenFileDialog someFileChooser;

            fileName = openFileChooser(out someFileChooser, out fullFilePath);
            
            FileHandler someFile = new FileHandler(someFileChooser.FileName);
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
        public void bindFunction(ref ListBox guiListBox)
        {
            try
            {
                //guiListBox.ItemTemplate = (DataTemplate)FindResource("WeaponModule");
                //guiListBox.DataContext = dataList.Elements();
                guiListBox.ItemsSource = dataList.Elements();
            }
            catch
            {
                showErrorMessage("Could not bind to xml");
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void showErrorMessage(String messageOfDoom)
        {
            MessageBox.Show(this, messageOfDoom, "Error!", MessageBoxButton.OK);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void saveData()
        {
            String fileName= baseWindowObject.currentFile();
            if (File.Exists(fileName))
            {
                dataList.Save(fileName);
            }
            
        }
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void closeProgram()
        {
            try
            {
                saveData();
                this.Close();
            }
            catch (InvalidOperationException)
            {
                showErrorMessage("Invalid operation exception caught in function EditorController.closeProgram().");
            }
        }

        public void delegateContainerErrorFunction()
        {
            ;
        }


        #endregion

        #region protectedFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected String openFileChooser(out OpenFileDialog fileChooser, out String fullPath)
        {
            fileChooser = new OpenFileDialog();

            fileChooser.Multiselect = false;
            fileChooser.Filter = "XML|*.xml;";
            fileChooser.InitialDirectory = directoryLastUsed;

            if (fileChooser.ShowDialog(this) == true)
            {
                fullPath = fileChooser.FileName;
                directoryLastUsed = fileChooser.FileName.TrimEnd(fileChooser.SafeFileName.ToCharArray());
                //showErrorMessage(directoryLastUsed);
                settingsFile.setKeyValuePair(EditorSettings.lastUsedDirectory.ToString(), directoryLastUsed);
                saveSettings();
                return fileChooser.SafeFileName;
            }
            return fullPath=null;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void saveSettings()
        {
            String settingsPath = System.IO.Path.Combine(ProgramFolder, "settings.xml");
            try
            {
                settingsFile.saveToFile(settingsPath);
            }
            catch
            {

                Directory.CreateDirectory(ProgramFolder);
                settingsFile = new KeyValuePairDataBase();
                settingsFile.saveToFile(settingsPath);
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveSettingsFile(String folderPath)
        {
            String settingsPath = System.IO.Path.Combine(ProgramFolder, "settings.xml");
            try
            {
                settingsFile = new KeyValuePairDataBase(settingsPath);
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

    public enum EditorSettings
    {
        lastUsedDirectory
    }
}
