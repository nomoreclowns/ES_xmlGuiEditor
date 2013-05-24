using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        static EditorSettings settings = EditorSettings.Default;

        public EditorController()
        {
            MainWindow baseWindowObject;

            InitializeComponent();

            String settingsPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            settings.progName);



        }
    }


    public class SettingsManager
    {
        private String settingsFile;

        public SettingsManager(String filePath)
        {
            settingsFile= filePath;
        }
    }


    
}
