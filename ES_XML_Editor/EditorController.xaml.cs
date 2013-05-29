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

            baseWindowObject = new MainWindow(this);
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
                //guiListBox.DataContext = dataList;
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
            dataList.Save(baseWindowObject.currentFile());
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
