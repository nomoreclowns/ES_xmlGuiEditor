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

    #region delegatesAndContainerClasses

    public delegate void controllerManipulateSetting(EditorController.EditorSettings delSettingKey, ref String delSettingValue, bool delSaveSetting = false);

    public delegate void controllerOpenFile();

    public delegate void controllerBind(ref CollectionView delcollectionView);

    public delegate void controllerShowError(String delMessageOfDoom);

    public delegate void controllerSave();

    public delegate void controllerAddItem(xmlElem delData);

    public delegate void controllerEditItem(xmlElem delData, int[] delSelectedIndeces);

    public delegate void controllerGetSelectedData(int[] delIndices, ref xmlElem delItemContainer);

    public delegate void controllerClose();

    public class controllerDataDelegateContainer
    {
        private controllerGetSelectedData pSelectedDataGetter;
        private controllerBind pBinder;
        private controllerAddItem pItemAdder;
        private controllerEditItem pItemEditor;

        public controllerDataDelegateContainer(controllerBind iBinder, controllerGetSelectedData iSelectedDataGetter, controllerAddItem iItemAdder, controllerEditItem iItemEditor)
        {
            pBinder = iBinder;
            pSelectedDataGetter = iSelectedDataGetter;
            pItemAdder = iItemAdder;
            pItemEditor = iItemEditor;
        }

        public void retrieveDelegates(out controllerBind iBinder, out controllerGetSelectedData iSelectedDataGetter, out controllerAddItem iItemAdder, out controllerEditItem iItemEditor)
        {
            iBinder = pBinder;
            iSelectedDataGetter = pSelectedDataGetter;
            iItemAdder = pItemAdder;
            iItemEditor = pItemEditor;
        }
    }

    public class controllerMiscDelegateContainer
    {
        private controllerSave pFileSaver;
        private controllerOpenFile pFileOpener;
        private controllerManipulateSetting pSettingHandler;

        public controllerMiscDelegateContainer(controllerOpenFile iFileOpener, controllerManipulateSetting iSettingHandler, controllerSave iFileSaver)
        {
            pFileOpener = iFileOpener;
            pFileSaver = iFileSaver;
            pSettingHandler = iSettingHandler;
        }

        public void retrieveDelegates(out controllerOpenFile iFileOpener, out controllerManipulateSetting iSettingHandler, out controllerSave iFileSaver)
        {
            iFileOpener = pFileOpener;
            iFileSaver = pFileSaver;
            iSettingHandler = pSettingHandler;
        }
    }

    public class controllerDelegateContainer
    {
        private controllerShowError pErrorDisplayer;
        private controllerClose pProgramCloser;

        public controllerDelegateContainer(controllerShowError iErrorDisplayer, controllerClose iProgramCloser = null)
        {
            pErrorDisplayer = iErrorDisplayer;
            if (iProgramCloser == null)
            {
                pProgramCloser = new controllerClose(emptyControllerCloseFunc);
                return;
            }
            pProgramCloser = iProgramCloser;
        }

        public void retrieveDelegates(out controllerShowError iErrorDisplayer, out controllerClose iProgramCloser)
        {
            iErrorDisplayer = pErrorDisplayer;
            iProgramCloser = pProgramCloser;
        }

        private void emptyControllerCloseFunc()
        {
        }
    }

    #endregion

    /// <summary>
    /// Interaction logic for EditorController.xaml
    /// </summary>
    public partial class EditorController : Window, INotifyPropertyChanged
    {
        public enum EditorSettings
        {
            lastDirectoryOpened,
            lastFileOpened,
            listItemHeight,
            itemEditorHeight,
            sidePanelWidth,
            editingWindowHeight,
            editingWindowWidth
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region membersSettersAndGetters

        // Shortcut variable for quicker access to the .settings file
        private static ProgramSettings embeddedSettings = ProgramSettings.Default;

        // list that stores the contents of a file
        private xmlElem dataContainerSource;

        // folder where program places important files (like user settings file)
        private String ProgramStoringFolder;

        private String userSettingsFilePath;

        // Manager for the users' settings file
        private KeyValuePairDataBase userSettings;

        //instance of the base window
        private MainWindow baseWindowObject;

        // the current "view" of the list
        private CollectionView dataContainerView;

        private FileHandler currentFile;
        
        public xmlElem dataContainer
        {
            set
            {
                if (value != null)
                {
                    dataContainerSource = value;
                    OnPropertyChanged("dataContainer");
                }
            }
            get
            {
                return dataContainerSource;
            }
        }

        #endregion

        //default constructor
        public EditorController()
        {
            InitializeComponent();


            retrieveSettingsFile();

            String workingDirectory = userSettings.getValue(EditorSettings.lastDirectoryOpened.ToString());
            
            String tempFileName = userSettings.getValue(EditorSettings.lastFileOpened.ToString());
            try
            {
                currentFile = new FileHandler(workingDirectory, tempFileName);

                dataContainerSource = new xmlElem(currentFile.open());
                dataContainerView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainerSource.xmlElements);
            }
            catch
            {
                if (Directory.Exists(workingDirectory) == false)
                {
                    currentFile = new FileHandler(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "");
                }
                else
                {
                    currentFile = new FileHandler(workingDirectory, "");
                }
            }

            showPrimaryWindow();
        }

        #region delegateFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void openFile()
        {
            String fileDirectory;
            String shortFileName;

            if (openFileChooser(out shortFileName, out fileDirectory) == true)
            {
                currentFile = new FileHandler(fileDirectory, shortFileName);

                setOrGetSetting(EditorSettings.lastDirectoryOpened, ref fileDirectory, true);

                setOrGetSetting(EditorSettings.lastFileOpened, ref shortFileName, true);

                dataContainerSource = new xmlElem(currentFile.open());

                dataContainerView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainer.xmlElements);
            }

            
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void bindFunction(ref CollectionView guiCollectionView)
        {
            //guiListBox.ItemsSource = dataContainerView;
            try
            {
                //guiCollectionView = dataContainerView;

                guiCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainer.xmlElements);

            }
            catch
            {
                //temporary error message
                showErrorMessage("Could not bind to xml");
            }
        }

        /* ************************************************************************************************************************
         * if there is a single item selected, it returns that items data,
         * if there are multiple items, returns an empty item that mimics the structure of the items
         *************************************************************************************************************************/
        protected void selectData(int[] indices, ref xmlElem itemContainer)
        {
            if (indices.Length == 1)
            {
                itemContainer = new xmlElem(dataContainerSource.xmlElements[indices[0]]);
            }
            else
            {
                itemContainer = createItem(dataContainerSource.xmlElements[0]);
            }
        }
        
        /* ************************************************************************************************************************
         * a wrapper function for the Xcontainer Add() method that gets passed to other classes through a delegate
         *************************************************************************************************************************/
        protected void addData(xmlElem source)
        {
            dataContainerSource.AddElement(source);
            OnPropertyChanged("dataContainer");
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void editData(xmlElem data, int[] selectedIndeces)
        {
            //List<xmlElem> temp= new List<xmlElem>(dataContainer.xmlElements);

            if (selectedIndeces.Length == 1)
            {
                foreach (int index in selectedIndeces)
                {
                    //XAttribute childAttribute = temp[index].Attribute("Name");
                    dataContainerSource.replaceChildByAttrValue("Name", dataContainerSource.xmlElements[index].Attribute("Name").Value, data);
                    
                    
                }
            }
            OnPropertyChanged("dataContainer");

        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void setOrGetSetting(EditorController.EditorSettings settingKey, ref String settingValue, bool saveAllSettings = false)
        {

            if (settingValue == "" || settingValue == null)
            {
                settingValue = userSettings.getValue(settingKey.ToString());
            }
            else
            {
                userSettings.setKeyValuePair(settingKey.ToString(), settingValue);
            }
            if (saveAllSettings == true)
            {
                saveSettings();
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
            //showErrorMessage(dataContainerSource.ToString());
            currentFile.setFileContent(dataContainerSource);
            currentFile.save();
        }
        
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void closeProgram()
        {
            try
            {
                saveSettings();
                //saveData();
                this.Close();
            }
            catch (InvalidOperationException)
            {
                showErrorMessage("Invalid operation exception caught in function EditorController.closeProgram().");
            }
        }




        #endregion

        #region nonDelegateFunctions


        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveSettingsFile()
        {
            ProgramStoringFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), embeddedSettings.progName);

            userSettingsFilePath = System.IO.Path.Combine(ProgramStoringFolder, embeddedSettings.settingsFileName);
            try
            {
                userSettings = new KeyValuePairDataBase(userSettingsFilePath);
            }
            catch
            {
                userSettings = new KeyValuePairDataBase();
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void showPrimaryWindow()
        {
            controllerDelegateContainer tempItem1 = new controllerDelegateContainer(showErrorMessage, closeProgram);
            controllerDataDelegateContainer tempItem2 = new controllerDataDelegateContainer(bindFunction, selectData, addData, editData);
            controllerMiscDelegateContainer tempItem3 = new controllerMiscDelegateContainer(openFile, setOrGetSetting, saveData);

            Double height = Convert.ToDouble(userSettings.getValue(EditorSettings.editingWindowHeight.ToString()));
            Double width = Convert.ToDouble(userSettings.getValue(EditorSettings.editingWindowWidth.ToString()));

            baseWindowObject = new MainWindow(tempItem1, tempItem2, tempItem3);
            try
            {
                baseWindowObject.Height = height;
                baseWindowObject.Width = width;
            }
            catch { }

            baseWindowObject.optionalInitialSetup();
            baseWindowObject.Show();
        }
        
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected bool openFileChooser(out String fileName, out String directory)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();

            fileChooser.Multiselect = false;
            fileChooser.Filter = "XML|*.xml;";
            fileChooser.InitialDirectory = currentFile.fileDirectory;

            if (fileChooser.ShowDialog(this) == true)
            {
                String fullFileName = fileChooser.FileName;
                fileName = fileChooser.SafeFileName;
                directory = fileChooser.FileName.TrimEnd(fileName.ToCharArray());
                return true;
            }
            directory = fileName = null;
            return false;
        }

        /* ************************************************************************************************************************
         * temporary functions for cloning the structure of an xmlElement, but wiping the data
         * once functions works well, move to xml classes
         *************************************************************************************************************************/
        protected xmlElem createItem(xmlElem cloneable)
        {
            return recuresiveElementCloner(cloneable);
        }

        protected xmlElem recuresiveElementCloner(xmlElem source)
        {
            xmlElem temp = new xmlElem(source.Name.ToString(), "");
            xmlAttrib tempAttr;
            temp.Value = "";
            foreach (xmlAttrib attr in source.Attributes)
            {
                tempAttr = temp.Attribute(attr.Name.ToString());
                tempAttr.Value = "";
                 
                
            }
            foreach (xmlElem item in source.Elements)
            {
                try
                {
                    temp.AddElement(recuresiveElementCloner(item));
                }
                catch { }
            }
            return temp;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void saveSettings()
        {
            String settingsPath = System.IO.Path.Combine(ProgramStoringFolder, "settings.xml");
            try
            {
                userSettings.saveToFile(settingsPath);
            }
            catch
            {

                Directory.CreateDirectory(ProgramStoringFolder);
                userSettings = new KeyValuePairDataBase();
                userSettings.saveToFile(settingsPath);
            }
        }

        /* ************************************************************************************************************************
         * event handler for when this window(program) recieves focus
         *************************************************************************************************************************/
        protected void programActivated(object sender, EventArgs e)
        {
            //redirect focus to primary window for now
            baseWindowObject.Activate();
        }

        #endregion

    }//end of class



    public class DirectoryHandler
    {
        private String pDirectoryPath;
        private String[] pFileNameList;
        private String[] pDirectoryList;
        private DirectoryInfo parent;

        public String workingDirectory
        {
            get
            {
                return pDirectoryPath;
            }
        }

        public String[] Files
        {
            get
            {
                return pFileNameList;
            }
        }

        public String[] SubDirectories
        {
            get
            {
                return pDirectoryList;
            }
        }

        public DirectoryHandler(String dirPath)
        {
            if (Directory.Exists(dirPath) == true)
            {
                pDirectoryPath = dirPath;

                try
                {
                    parent = Directory.GetParent(dirPath);
                }
                catch
                {
                    parent = null;
                }
                try
                {
                    pFileNameList = Directory.GetDirectories(dirPath, "", SearchOption.TopDirectoryOnly);

                    for(int i=0; i<pDirectoryList.Length;i++)
                    {
                        pFileNameList[i]= pFileNameList[i].Substring(dirPath.Length+1, (pFileNameList[i].Length- dirPath.Length));;
                    }
                }
                catch
                {
                    pFileNameList = null;
                }
                try
                {
                    pDirectoryList = Directory.GetFiles(dirPath, "", SearchOption.TopDirectoryOnly);

                    foreach (String filePath in pFileNameList)
                    {
                    }
                }
                catch
                {
                    pDirectoryList = null;
                }
            }

        }
    }

}
