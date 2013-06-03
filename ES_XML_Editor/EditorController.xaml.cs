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

    #region delegatesAndContainerClass

    public delegate void controllerManipulateSetting(String delSettingKey, ref String delSettingValue, bool delSaveSetting = false);

    public delegate void controllerOpenFile(out String delFileName, out String delFullFilePath);

    public delegate void controllerBind(ref ListBox delListBox);

    public delegate void controllerShowError(String delMessageOfDoom);

    public delegate void controllerSave();

    public delegate void controlerAddItem();

    public delegate void controllerGetSelectedData(int[] delIndices, ref xmlElement delItemContainer);

    public delegate void controllerClose();

    public delegate void controllerErrorCallback();

    public class controllerDelegateContainer
    {
        private controllerOpenFile pFileOpener;
        private controllerBind pBinder;
        private controllerShowError pErrorDisplayer;
        private controllerGetSelectedData pSelectedDataGetter;
        private controllerSave pFileSaver;
        private controllerClose pProgramCloser;
        private controllerErrorCallback pAccessViolationInformer;
        private controllerManipulateSetting pSettingHandler;

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

        public controllerDelegateContainer(controllerShowError iErrorDisplayer, controllerErrorCallback iAccessViolationInformer, controllerOpenFile iFileOpener = null, controllerBind iBinder = null, controllerGetSelectedData iSelectedDataGetter = null, controllerManipulateSetting iSettingHandler = null, controllerSave iFileSaver = null, controllerClose iProgramCloser = null)
        {
            pErrorDisplayer = iErrorDisplayer;
            pFileOpener = iFileOpener;
            pBinder = iBinder;
            pSelectedDataGetter = iSelectedDataGetter;
            pFileSaver = iFileSaver;
            pProgramCloser = iProgramCloser;
            pAccessViolationInformer = iAccessViolationInformer;
            pSettingHandler = iSettingHandler;
        }

        public void unauthorizedAccess()
        {
            ;
        }

        public void retrieveDelegates(out controllerShowError iErrorDisplayer, out controllerOpenFile iFileOpener, out controllerBind iBinder, out controllerGetSelectedData iSelectedDataGetter, out controllerManipulateSetting iSettingHandler, out controllerSave iFileSaver, out controllerClose iProgramCloser)
        {
            iErrorDisplayer = pErrorDisplayer;
            iFileOpener = pFileOpener;
            iBinder = pBinder;
            iSelectedDataGetter = pSelectedDataGetter;
            iFileSaver = pFileSaver;
            iProgramCloser = pProgramCloser;
            iSettingHandler = pSettingHandler;
        }

    }

    #endregion

    /// <summary>
    /// Interaction logic for EditorController.xaml
    /// </summary>
    public partial class EditorController : Window
    {
        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings embeddedSettings = ProgramSettings.Default;

        // list that stores the contents of a file
        private xmlElement dataContainer;

        // the current "view" of the list
        private CollectionView dataContainerView;

        String currentFile;

        //String currentDirectory;

        String ProgramFolder;

        // Manager for the users' settings file
        private KeyValuePairDataBase userSettings;

        // String to hold the users' last visited directory when using this program.
        // Eventually gets stored as a setting in a file
        private String directoryLastUsed;

        //instance of the base window
        private MainWindow baseWindowObject;


        //default constructor
        public EditorController()
        {
            InitializeComponent();

            ProgramFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), embeddedSettings.progName);

            retrieveSettingsFile(ProgramFolder);

            directoryLastUsed = userSettings.getValue(EditorSettings.lastUsedDirectory.ToString());

            
            if (Directory.Exists(directoryLastUsed) == false)
            {
                directoryLastUsed= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            baseWindowObject = new MainWindow(new controllerDelegateContainer(showErrorMessage, delegateContainerErrorFunction, openFile, bindFunction, selectedData, setOrGetSetting, saveData, closeProgram));
            baseWindowObject.Show();
        }

        #region publicFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void openFile(out String fileName, out String fullFilePath)
        {
            OpenFileDialog someFileChooser;

            currentFile = fileName = openFileChooser(out someFileChooser, out fullFilePath);
            
            FileHandler someFile = new FileHandler(someFileChooser.FileName);
            //dataList = someFile.open();
            dataContainer = new xmlElement(someFile.open());
            dataContainerView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainer.xmlElements);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        //public void bindFunction(ref ListBox guiListBox)
        //{
        //    try
        //    {
        //        //guiListBox.ItemTemplate = (DataTemplate)FindResource("WeaponModule");
        //        //guiListBox.DataContext = dataList.Elements();
        //        guiListBox.ItemsSource = dataListView;

        //    }
        //    catch
        //    {
        //        showErrorMessage("Could not bind to xml");
        //    }
        //}

        protected void bindFunction(ref ListBox guiListBox)
        {
            try
            {
                guiListBox.ItemsSource = dataContainerView;
                //detailBox.Content = dataListView.CurrentItem;
                
            }
            catch
            {
                showErrorMessage("Could not bind to xml");
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void selectedData(int[] indices, ref xmlElement itemContainer)
        {
            if (indices.Length == 1)
            {
                itemContainer = dataContainer.xmlElements[indices[0]]
                    ;
            }
            else
            {
                itemContainer = createItem();
            }
        }
        
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void addData(xmlElement data)
        {
            dataContainer.Add(data);
        }


        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected xmlElement createItem()
        {
            return recuresiveElementCreator(dataContainer.xmlElements[0]);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected xmlElement recuresiveElementCreator( xmlElement source)
        {
            xmlElement temp = new xmlElement(source.Name.ToString());

            temp.SetValue("");
            foreach (XAttribute attr in source.Attributes())
            {
                temp.SetAttributeValue(attr.Name, "");
            }
            foreach (xmlElement item in source.xmlElements)
            {
                try
                {
                    temp.Add(recuresiveElementCreator(item));
                }
                catch
                {
                }
            }
            return temp;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void setOrGetSetting(String settingKey, ref String settingValue, bool saveSetting = false)
        {
            if (saveSetting == false)
            {
                settingValue = userSettings.getValue(settingKey);
            }
            else
            {
                userSettings.setKeyValuePair(settingKey, settingValue);
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void showErrorMessage(String messageOfDoom)
        {
            MessageBox.Show(this, messageOfDoom, "Error!", MessageBoxButton.OK);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void saveData()
        {
            String fileName= baseWindowObject.currentFile();
            if (File.Exists(fileName))
            {
                dataContainer.Save(fileName);
            }
            
        }
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void closeProgram()
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

        protected void delegateContainerErrorFunction()
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
                userSettings.setKeyValuePair(EditorSettings.lastUsedDirectory.ToString(), directoryLastUsed);
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
                userSettings.saveToFile(settingsPath);
            }
            catch
            {

                Directory.CreateDirectory(ProgramFolder);
                userSettings = new KeyValuePairDataBase();
                userSettings.saveToFile(settingsPath);
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveSettingsFile(String folderPath)
        {
            String settingsPath = System.IO.Path.Combine(ProgramFolder, "settings.xml");
            try
            {
                userSettings = new KeyValuePairDataBase(settingsPath);
            }
            catch
            {
                userSettings= new KeyValuePairDataBase();
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

    public enum EditorSettings
    {
        lastUsedDirectory,
        listItemHeight,
        itemEditorHeight
    }
    }

}
