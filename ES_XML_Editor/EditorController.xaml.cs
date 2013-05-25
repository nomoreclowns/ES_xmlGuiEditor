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

        public void openFileChooser()
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Multiselect = false;
            fileChooser.Filter = "XML|*.xml;";
            fileChooser.InitialDirectory = @directoryLastUsed;

            if (fileChooser.ShowDialog(this) == true)
            {
                ;
            }
        }

        

        public void retrieveSettingsFile(String filePath)
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

        public void showErrorMessage(String messageOfDoom)
        {
            MessageBox.Show(this, messageOfDoom,"Error!", MessageBoxButton.OK);
        }

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

    }

    //class for handling the user's settings file
    public class SettingsManager
    {

    }


    public class FileHandler
    {
    }


    public enum EditorSettings
    {
        lastUsedDirectory
    }
}
