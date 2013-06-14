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
    /*
    #region delegatesAndContainerClasses

    public delegate void controllerManipulateSetting(EditorController.EditorSettings delSettingKey, ref String delSettingValue, bool delSaveSetting = false);

    public delegate void controllerOpenFile(bool delBool = false);

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
    */


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
        private List<xmlElem> dataContainerList;
        private xmlElem oldDataSource;

        // folder where program places important files (like user settings file)
        private String ProgramStoringFolder;

        // Manager for the users' settings file
        private KeyValuePairDataBase userSettings;

        //instance of the base window
        private MainWindow baseWindowObject;

        // the current "view" of the list
        private CollectionView oldDataView;



        private List<FileHandler> filesOpen;
        private FileHandler currentFile;

        private DirectoryHandler workingDirectory;
        
        public xmlElem oldDataContainer
        {
            set
            {
                if (value != null)
                {
                    oldDataSource = value;
                    OnPropertyChanged("dataContainer");
                }
            }
            get
            {
                return oldDataSource;
            }
        }

        #endregion

        //default constructor
        public EditorController()
        {
            InitializeComponent();

            retrieveSettingsFile();

            filesOpen= new List<FileHandler>();

            dataContainerList = new List<xmlElem>();

            String tempWorkingDirectory = userSettings.getValue(EditorSettings.lastDirectoryOpened.ToString());

            if (Directory.Exists(tempWorkingDirectory) == true)
            {
                workingDirectory = new DirectoryHandler(tempWorkingDirectory, new controllerShowError(showErrorMessage));

                String tempFileName = userSettings.getValue(EditorSettings.lastFileOpened.ToString());

                //testListView.ItemsSource = workingDirectory.contents;

                try
                {
                    currentFile = new FileHandler(workingDirectory.path + tempFileName);

                    oldDataSource = new xmlElem(currentFile.open());
                    oldDataView = (CollectionView)CollectionViewSource.GetDefaultView(oldDataSource.xmlElements);
                }
                catch
                {
                    currentFile = new FileHandler(tempWorkingDirectory, "");
                }
            }
            else
            {
                workingDirectory = new DirectoryHandler(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), new controllerShowError(showErrorMessage));
                currentFile = new FileHandler(workingDirectory.path, "");
            }


            showPrimaryWindow();
        }

        #region delegateFunctions

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void openFile(out String shortFileName, bool useNewVersion = true)
        {
            String fileDirectory;

            if (useNewVersion == false)
            {
                if (openFileChooser(out shortFileName, out fileDirectory) == true)
                {
                    currentFile = new FileHandler(fileDirectory, shortFileName);

                    setOrGetSetting(EditorSettings.lastDirectoryOpened, ref fileDirectory, true);

                    setOrGetSetting(EditorSettings.lastFileOpened, ref shortFileName, true);

                    oldDataSource = new xmlElem(currentFile.open());

                    oldDataView = (CollectionView)CollectionViewSource.GetDefaultView(oldDataContainer.xmlElements);
                }
                return;
            }

            if (openFileChooser(out shortFileName, out fileDirectory) == true)
            {
                FileHandler tempFileHandler = new FileHandler(fileDirectory, shortFileName);

                setOrGetSetting(EditorSettings.lastDirectoryOpened, ref fileDirectory, true);

                //setOrGetSetting(EditorSettings.lastFileOpened, ref shortFileName, true);

                xmlElem tempDataElement = new xmlElem(currentFile.open());

                //dataContainerView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainer.xmlElements);
                filesOpen.Clear();
                filesOpen.Add(tempFileHandler);
                dataContainerList.Clear();
                dataContainerList.Add(tempDataElement);
            }

        }

        protected void openAnotherFile(out CollectionView guiCollectionView, String fileName)
        {
            FileHandler tempFileHandler = new FileHandler(workingDirectory.path, fileName);
            xmlElem tempDataElement = new xmlElem(tempFileHandler.open());

            try
            {
                guiCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(tempDataElement.xmlElements);
            }
            catch
            {
                showErrorMessage("Could not bind to xml");
                guiCollectionView = null;
            }

            filesOpen.Add(tempFileHandler);
            dataContainerList.Add(tempDataElement);
        }

        protected void openAnotherFile(out CollectionView guiCollectionView, int fileNameIndex)
        {
            FileHandler tempFileHandler = new FileHandler(workingDirectory.path, filesOpen[fileNameIndex].fileName);
            xmlElem tempDataElement = new xmlElem(tempFileHandler.open());

            try
            {
                guiCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(tempDataElement.xmlElements);
            }
            catch
            {
                showErrorMessage("Could not bind to xml");
                guiCollectionView = null;
            }

            filesOpen.Add(tempFileHandler);
            dataContainerList.Add(tempDataElement);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void passFolderContents(out CollectionView guiContentsCollectionView)
        {
            guiContentsCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(workingDirectory.contents);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void bindFunction(out CollectionView guiCollectionView)
        {
            //guiListBox.ItemsSource = dataContainerView;
            try
            {
                //guiCollectionView = dataContainerView;

                guiCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(oldDataContainer.xmlElements);
            }
            catch
            {
                //temporary error message
                showErrorMessage("Could not bind to xml");
                guiCollectionView = null;
            }
        }

        protected void bindFunction(out CollectionView guiCollectionView, int fileDataSelector)
        {
            try
            {
                guiCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(dataContainerList[fileDataSelector].xmlElements);

            }
            catch
            {
                //temporary error message
                showErrorMessage("Could not bind to xml");
                guiCollectionView = null;
            }
        }

        /* ************************************************************************************************************************
         * if there is a single item selected, it returns that items data,
         * if there are multiple items, returns an empty item that mimics the structure of the items
         *************************************************************************************************************************/
        protected void selectData(int[] indices, ref xmlElem itemContainer)
        {
            if (indices.Length == 0)
            {
                return;
            }
            else if (indices.Length == 1)
            {
                itemContainer = oldDataSource.xmlElements[indices[0]];
            }
            else
            {
                itemContainer = createItem(oldDataSource.xmlElements[indices[0]]);
            }
        }

        protected void selectData(int[] indices, ref xmlElem itemContainer, int fileDataSelector)
        {
            if (indices.Length == 0)
            {
                return;
            }
            else if (indices.Length == 1)
            {
                itemContainer = dataContainerList[fileDataSelector].xmlElements[indices[0]];
                //itemContainer = oldDataSource.xmlElements[indices[0]];
            }
            else
            {
                itemContainer = createItem(dataContainerList[fileDataSelector].xmlElements[indices[0]]);
                //itemContainer = createItem(oldDataSource.xmlElements[indices[0]]);
            }
        }

        /* ************************************************************************************************************************
         * a wrapper function for the Xcontainer Add() method that gets passed to other classes through a delegate
         *************************************************************************************************************************/
        protected void addData(xmlElem source)
        {
            oldDataSource.AddElement(source);
            //OnPropertyChanged("dataContainer");
        }

        protected void addData(xmlElem source, int fileDataSelector)
        {
            dataContainerList[fileDataSelector].AddElement(source);
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
                    oldDataSource.replaceChildByAttrValue("Name", oldDataSource.xmlElements[index].Attribute("Name").Value, data);
                    
                    
                }
            }
            OnPropertyChanged("dataContainer");
        }

        protected void editData(xmlElem data, int[] selectedIndeces, int fileDataSelector)
        {
            //List<xmlElem> temp= new List<xmlElem>(dataContainer.xmlElements);

            if (selectedIndeces.Length == 1)
            {
                foreach (int index in selectedIndeces)
                {
                    //XAttribute childAttribute = temp[index].Attribute("Name");
                    dataContainerList[fileDataSelector].replaceChildByAttrValue("Name", oldDataSource.xmlElements[index].Attribute("Name").Value, data);


                }
            }
            //OnPropertyChanged("dataContainer");
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
            currentFile.setFileContent(oldDataSource);
            currentFile.save();
        }

        protected void saveData(int fileDataSelector)
        {
            //showErrorMessage(dataContainerSource.ToString());
            filesOpen[fileDataSelector].setFileContent(oldDataSource);
            filesOpen[fileDataSelector].save();
            //currentFile.setFileContent(oldDataSource);
            //currentFile.save();
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

            try
            {
                userSettings = new KeyValuePairDataBase(System.IO.Path.Combine(ProgramStoringFolder, embeddedSettings.settingsFileName));
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
            controllerMiscDelegateContainer tempItem3 = new controllerMiscDelegateContainer(openFile, setOrGetSetting, saveData, passFolderContents);

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
            fileChooser.InitialDirectory = workingDirectory.path;

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
            String settingsPath = System.IO.Path.Combine(ProgramStoringFolder, embeddedSettings.settingsFileName);
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

        //private void listViewItemDoubleCLick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{

        //}

        //protected void listViewItemActivation()
        //{
        //    ioObject temp = testListView.SelectedItem as ioObject;

        //    if (temp.fileKind == ioObject.Filetype.File)
        //    {
        //        ;
        //    }
        //    else
        //    {

        //    }

        //}

    }//end of class

    public class ioObject
    {
        public enum Filetype
        {
            File,
            Folder
        }

        protected String pName = "";

        protected Filetype pKind;

        public String name
        {
            get
            {
                return pName;
            }
        }

        public String kind
        {
            get
            {
                return pKind.ToString();
            }
        }

        public ioObject.Filetype fileKind
        {
            get
            {
                return pKind;
            }
        }

        public ioObject(String iName, ioObject.Filetype iKind)
        {
            pName = iName;

            pKind = iKind;
        }
    }

    public class DirectoryHandler
    {
        private controllerShowError contDisplayError;
        private String pDirectoryPath;
        private DirectoryInfo parent;

        private List<ioObject> pContents;

        public String path
        {
            get
            {
                return pDirectoryPath;
            }
        }

        public List<ioObject> contents
        {
            get
            {
                return pContents;
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public DirectoryHandler(String dirPath, controllerShowError DisplayError)
        {
            contDisplayError = DisplayError;

            pContents= new List<ioObject>();

            pDirectoryPath = dirPath;

            retrieveContents();

            try
            {
                parent = Directory.GetParent(pDirectoryPath);
            }
            catch
            {
                contDisplayError("unable to retrieve parent of folder "+dirPath);
            }


        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        protected void retrieveContents()
        {
            if (Directory.Exists(pDirectoryPath) == true)
            {

                int pathLength = pDirectoryPath.Length;

                try
                {
                    String[] directoryArray = Directory.GetDirectories(pDirectoryPath, "*", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < directoryArray.Length; i++)
                    {
                        pContents.Add(new ioObject(directoryArray[i].Substring(pathLength), ioObject.Filetype.Folder));
                    }
                }
                catch
                {
                    contDisplayError("Could not retrieve directories.");
                }
                try
                {
                    String[] fileArray = Directory.GetFiles(pDirectoryPath, "*", SearchOption.TopDirectoryOnly);

                    for (int i = 0; i < fileArray.Length; i++)
                    {
                        pContents.Add(new ioObject(fileArray[i].Substring(pathLength), ioObject.Filetype.File));
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                return;
            }
            else
            {
                contDisplayError("Cannot show a directory that does not exist");
            }
        }
    }

}
