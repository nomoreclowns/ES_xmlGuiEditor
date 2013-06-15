using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region delegateRegion

        private controllerOpenFile contFileOpener;
        private controllerOpenNewFile contNewFileOpener;
        //private controllerBind contBinder;
        private controllerShowError contErrorDisplayer;
        private controllerSave contFileSaver;
        private controllerClose contProgramCloser;
        private controllerGetSelectedData contDataGetter;
        private controllerManipulateSetting contSettingHandler;
        private controllerAddItem contItemAdder;
        private controllerEditItem contItemEditor;
        private controllerFolderContents contGetFolderContents;

        #endregion




        #region enumSettingInstaces

        private const EditorController.EditorSettings heightSettingEnum = EditorController.EditorSettings.editingWindowHeight;
        private const EditorController.EditorSettings widthSettingEnum = EditorController.EditorSettings.editingWindowWidth;
        private const EditorController.EditorSettings editingItemSettingEnum = EditorController.EditorSettings.itemEditorHeight;
        private const EditorController.EditorSettings listItemSettingEnum = EditorController.EditorSettings.listItemHeight;
        private const EditorController.EditorSettings sidePanelWidth= EditorController.EditorSettings.sidePanelWidth;

        #endregion

        #region INotifyPropertyChangedArea

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        private String itemEditorHeadingString= "";

        public String itemEditorHeading
        {
            set
            {
                if (value != null)
                {
                    itemEditorHeadingString = value;
                    OnPropertyChanged("itemEditorHeading");
                }
            }
            get
            {
                return itemEditorHeadingString;
            }
        }

        private xmlElem itemEditorDataElement;

        public xmlElem itemEditorData
        {
            set
            {
                itemEditorDataElement = value;
                OnPropertyChanged("itemEditorData");
            }
            get
            {
                return itemEditorDataElement;
            }
        }

        private CollectionView oldDataViewSource;

        public CollectionView oldDataView
        {
            set
            {
                if (value != null)
                {
                    oldDataViewSource = value;
                    OnPropertyChanged("dataView");
                }
            }
            get
            {
                return oldDataViewSource;
            }
        }

        private CollectionView fileListCollectionView;

        private List<CollectionView> dataSourceContainer;

        //list of indeces for all select items in Listbox
        //private int[] listBoxSelectedItems
        //{
        //    get
        //    {
        //        try
        //        {
        //            int[] someArray = new int[windowListbox.SelectedItems.Count];
        //            for (int i = 0; i < windowListbox.SelectedItems.Count; i++)
        //            {
        //                someArray[i] = windowListbox.Items.IndexOf(windowListbox.SelectedItems[i]);
        //            }
        //            return someArray;
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}


        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public MainWindow(controllerDelegateContainer editorFunctions, controllerDataDelegateContainer editorDataFunctions, controllerMiscDelegateContainer editorMiscFunctions)
        {
            InitializeComponent();

            if (editorFunctions == null)
            {
                throw new NullReferenceException("MainWindow Constructor cannot have a null references to editorFunctions, editorDataFunctions, or editorMiscFunctions");
            }
            editorFunctions.retrieveDelegates(out contErrorDisplayer, out contProgramCloser);
            editorDataFunctions.retrieveDelegates(out contDataGetter, out contItemAdder, out contItemEditor);
            editorMiscFunctions.retrieveDelegates(out contFileOpener, out contNewFileOpener, out contSettingHandler, out contFileSaver, out contGetFolderContents);

            //windowListbox.ItemsSource = dataView;

            dataSourceContainer = new List<CollectionView>();
            itemEditorDataElement = new xmlElem();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void optionalInitialSetup()
        {
            try
            {
                String itemHeight = "";
                contSettingHandler(editingItemSettingEnum, ref itemHeight);
                itemEditorArea.Height = new GridLength(Convert.ToDouble(itemHeight));
            }
            catch
            {
                itemEditorArea.Height = new GridLength(1.0, GridUnitType.Auto);
            }

            try
            {
                String listItemHeight = "";
                contSettingHandler(listItemSettingEnum, ref listItemHeight);
                //listItemHeightSlider.Value = Convert.ToDouble(listItemHeight);
            }
            catch
            {
                //listItemHeightSlider.Value = listItemHeightSlider.Minimum;
            }

            //contBinder(ref oldDataViewSource);
            //oldDataView = oldDataViewSource;
            
            try
            {
                itemEditorText1.Text = "Selected Item Indeces: ";

                Binding myBinding = new Binding("itemEditorData");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.ElementName = "PrimaryWindow";
                myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                listBoxItemViewer.SetBinding(ScrollViewer.ContentProperty, myBinding);
            }
            catch
            {
                itemEditorText1.Text = "";
            }

            try
            {
                String sidePanelWidthString = "";
                contSettingHandler(sidePanelWidth, ref sidePanelWidthString);
                Double sidePanelWidthValue= Convert.ToDouble(sidePanelWidthString);
                if (sidePanelWidthString != null && sidePanelWidthString != "")
                {
                    sidePanelArea.Width = new GridLength(sidePanelWidthValue);
                }
                else
                {
                    sidePanelArea.Width = new GridLength(100.0, GridUnitType.Pixel);
                }
            }
            catch
            {
                sidePanelArea.Width = new GridLength(100.0, GridUnitType.Pixel);
            }

            try
            {
                contGetFolderContents(out fileListCollectionView);
                FileListWindow.ItemsSource = fileListCollectionView;
            }
            catch { }
        }
        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void closingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contProgramCloser();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void openFileButtonClick(object sender, RoutedEventArgs e)
        {
            dataSourceContainer.Clear();

            xmlTabControl.Items.Clear();

            String shortFileName;

            CollectionView tempDataView;

            contFileOpener(out tempDataView, out shortFileName);

            dataSourceContainer.Add(tempDataView);

            contGetFolderContents(out fileListCollectionView);

            //dataSourceContainer.Add(tempDataView);

            //getFileContents(shortFileName);
            if (tempDataView == null)
            {
                return;
            }

            

            FileListWindow.ItemsSource = fileListCollectionView;

            xmlTabControl.Items.Add(generateTabItem(ref tempDataView, shortFileName));

            (xmlTabControl.Items[0] as TabItem).IsSelected = true;

            itemEditorText1.Text = "Selected Item Indeces: ";
            //itemEditorText2.Text = "None";

            openFileButton.IsEnabled = false;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        //private void getFileContents(String tabItemHeader)
        //{
        //    CollectionView tempDataView;

        //    contBinder(out tempDataView, 0);

        //    dataSourceContainer.Add(tempDataView);

        //    xmlTabControl.Items.Add(generateTabItem(ref tempDataView, tabItemHeader));

        //    (xmlTabControl.Items[0] as TabItem).IsSelected = true;
        //    //xmlTabControl.SelectedIndex = 0;

        //}

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private TabItem generateTabItem(ref CollectionView contentView, String itemHeader)
        {
            TabItem tabbedItem = new TabItem();

            CustomListbox temp = new CustomListbox(ref contentView);

            tabbedItem.Content = new CustomListbox(ref contentView);
            tabbedItem.Header = createHeaderDataTemplate(itemHeader);

            return tabbedItem;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private StackPanel createHeaderDataTemplate(String name)
        {
            StackPanel headerStackpanel = new StackPanel();
            headerStackpanel.Orientation = Orientation.Horizontal;

            Button closeFileButton = new Button();
            closeFileButton.Content = "X";
            closeFileButton.Click += closeFileButtonClicked;
            closeFileButton.Margin = new Thickness(5.0, 0.0, 0.0, 0.0);

            TextBlock header = new TextBlock();
            header.Text = name;

            headerStackpanel.Children.Add(header);
            headerStackpanel.Children.Add(closeFileButton);

            return headerStackpanel;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void fileListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int tempIndex = FileListWindow.SelectedIndex;
            String fileName = "" + (FileListWindow.Items[tempIndex] as ioObject).name;
            
            if (tempIndex >= 0)
            {
                CollectionView tempDataView;

                contNewFileOpener(out tempDataView, tempIndex);

                xmlTabControl.Items.Add(generateTabItem(ref tempDataView, fileName));

                int lastItemIndex = xmlTabControl.Items.Count - 1;
                (xmlTabControl.Items[lastItemIndex] as TabItem).IsSelected = true;
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void closeFileButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void tempFunctionName()
        {

        }


        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void windowListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            //    xmlElem tempElement = null;
            //    contDataGetter(listBoxSelectedItems, ref itemEditorDataElement);
            //    itemEditorData = itemEditorDataElement;
            //    //listBoxItemViewer.Content = itemEditorData;
            //    debugWindow.Text = itemEditorData.ToString();
            //    //Binding myBinding = new Binding("itemEditorData");
            //    //myBinding.Mode = BindingMode.TwoWay;
            //    //myBinding.ElementName = "PrimaryWindow";
            //    //myBinding.Path = new PropertyPath(itemEditorData);
            //    //listBoxItemViewer.SetBinding(ScrollViewer.ContentProperty, myBinding);

            //    String temp = "";
            //    foreach (int index in listBoxSelectedItems)
            //    {
            //        temp += index.ToString() + ", ";
            //    }
            //    itemEditorHeading = temp;
            //}
            //catch { }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        //private void listBoxSourceUpdatedHandler(object sender, DataTransferEventArgs e)
        //{
        //    contErrorDisplayer("listBox source updated");
        //    //controllerReference.saveData();
        //    contFileSaver();
        //}

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void windowResized(object sender, SizeChangedEventArgs e)
        {
            String tempHeight = this.Height.ToString();
            String tempWidth = this.Width.ToString();

            contSettingHandler(heightSettingEnum, ref tempHeight);
            contSettingHandler(widthSettingEnum, ref tempWidth);
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void itemEditorResized(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                String tempItemHeight = this.itemEditorArea.Height.Value.ToString();
                contSettingHandler(editingItemSettingEnum, ref tempItemHeight);
            }
            catch
            {
                contErrorDisplayer("Exception in MainWindow.itemEditorResized()");
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void sidePanelResizedHandler(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                //String tempItemHeight = (sender as ColumnDefinition).Width.Value.ToString();
                String tempSidePanelWidth = this.sidePanelArea.Width.Value.ToString();
                contSettingHandler(sidePanelWidth, ref tempSidePanelWidth);
            }
            catch
            {
                //contErrorDisplayer(sender.ToString());
                //contErrorDisplayer(sidePanelArea.ToString());
                contErrorDisplayer("Exception in MainWindow.sidePanelResizedHandler()");
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void itemHeightSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //String temp = listItemHeightSlider.Value.ToString();
                //contSettingHandler(listItemSettingEnum, ref temp, true);
            }
            catch { }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void saveItem(object sender, RoutedEventArgs e)
        {
            //int tempIndex = windowListbox.SelectedIndex;

            //contItemEditor(this.itemEditorData, this.listBoxSelectedItems);

            //contBinder(ref oldDataViewSource);
            //oldDataView = oldDataViewSource;

            //windowListbox.SelectedIndex = tempIndex;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        //private void viewerItemTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    String tempObject = e.Source.ToString();

        //    String tempText = (e.Source as TextBox).Text;
        //}

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void saveFileButtonClicked(object sender, RoutedEventArgs e)
        {
            //contFileSaver();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void duplicateItemClick(object sender, RoutedEventArgs e)
        {
            //int listIndex= windowListbox.Items.Count;
            //try
            //{
            //    contItemAdder(new xmlElem(itemEditorData));
            //    //contBinder(ref dataViewSource);
            //    //dataView = dataViewSource;

            //    windowListbox.SelectedItem = windowListbox.Items[listIndex];
            //}
            //catch { }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void xmlTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //contErrorDisplayer("tabitem with index "+xmlTabControl.SelectedIndex.ToString()+" selected");

            int selectedTabIndex = xmlTabControl.SelectedIndex;

            if (xmlTabControl.Items.Count >= 2)
            {
                try
                {
                    CustomListbox tempBox = ((xmlTabControl.Items[selectedTabIndex] as TabItem).Content as CustomListbox);
                    //CustomListbox tempBox = (xmlTabControl.SelectedContent as CustomListbox);




                    ItemCollection tempList = ((xmlTabControl.Items[selectedTabIndex] as TabItem).Content as CustomListbox).tabItemListbox.Items;

                    //this.debugWindow.Text = "";

                    //foreach(object item in tempList)
                    //{
                    //    this.debugWindow.Text += (item as xmlElem).ToString();
                    //}





                    int[] tempIndeces = tempBox.listBoxSelectedItems;

                    //int[] tempIndeces = (xmlTabControl.SelectedContent as CustomListbox).listBoxSelectedItems;

                    contDataGetter(tempIndeces, ref itemEditorDataElement, selectedTabIndex);

                    //int[] tempIndeces = ((xmlTabControl.Items[0] as TabItem).Content as CustomListbox).listBoxSelectedItems;

                    itemEditorData = itemEditorDataElement;
                    listBoxItemViewer.Content = itemEditorData;
                    Binding myBinding = new Binding("itemEditorData");
                    myBinding.Mode = BindingMode.TwoWay;
                    myBinding.ElementName = "PrimaryWindow";
                    listBoxItemViewer.SetBinding(ScrollViewer.ContentProperty, myBinding);

                    //String temp = "";
                    //foreach (int index in listBoxSelectedItems)
                    //{
                    //    temp += index.ToString() + ", ";
                    //}
                    //itemEditorHeading = temp;
                }
                catch// (Exception et)
                {
                    //contErrorDisplayer(et.ToString());
                }

                return;
            }
            else if (xmlTabControl.Items.Count == 1)
            {
                TabItem tempItem = (xmlTabControl.Items[0] as TabItem);

                CustomListbox tempBox = (tempItem.Content as CustomListbox);

                try
                {
                    int[] tempIndeces = tempBox.listBoxSelectedItems;
                    contDataGetter(tempIndeces, ref itemEditorDataElement, 0);
                }
                catch
                {
                    contErrorDisplayer(tempBox.listBoxSelectedItems.ToString());
                    
                }

                //int[] tempIndeces = ((xmlTabControl.Items[0] as TabItem).Content as CustomListbox).listBoxSelectedItems;
                itemEditorData = itemEditorDataElement;
                listBoxItemViewer.Content = itemEditorData;
                Binding myBinding = new Binding("itemEditorData");
                myBinding.Mode = BindingMode.TwoWay;
                myBinding.ElementName = "PrimaryWindow";
                listBoxItemViewer.SetBinding(ScrollViewer.ContentProperty, myBinding);
            }
        }

        
    }
}
