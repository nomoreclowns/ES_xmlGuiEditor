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
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Media.Animation;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
using Microsoft.Win32;
using System.Timers;


namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for EditorController.xaml
    /// </summary>
    public partial class EditorController : Window
    {
        private static EditorSettings settings = EditorSettings.Default;

        private SettingsManager settingsManagerInstance;

        private String directoryLastUsed;

        MainWindow baseWindowObject;

        //default constructor
        public EditorController()
        {
            InitializeComponent();

            String settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), settings.progName);

            settingsManagerInstance = new SettingsManager(settingsPath);

            directoryLastUsed = settingsManagerInstance.findSetting(EditorSettings.lastUsedDirectory);

            baseWindowObject = new MainWindow(this);
            baseWindowObject.Show();
            
        }

        public void openFileChooser()
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Multiselect = false;
            fileChooser.Filter = "XML|*.xml;";
        }


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
            catch (InvalidOperationException e)
            {
                showErrorMessage("Invalid operation exception caught in function EditorController.closeProgram().");
            }
        }

    }

    //class for handling the user's settings file
    public class SettingsManager
    {
        private KeyValuePairDatabase settingsFile;

        public SettingsManager(String filePath)
        {
            //settingsFile= filePath;
        }

        public String findSetting(EditorSettings setting)
        {
            String returnableSetting = null;

            switch (setting)
            {
                case EditorSettings.lastUsedDirectory:
                    break;
                default:
                    return null;
            }
            return null;
        }
    }


    public class FileHandler
    {
    }


    public enum EditorSettings
    {
        lastUsedDirectory
    }
}
