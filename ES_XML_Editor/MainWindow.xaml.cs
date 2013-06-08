using System;
using System.Collections.Generic;
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
        private controllerBind contBinder;
        private controllerShowError contErrorDisplayer;
        private controllerSave contFileSaver;
        private controllerClose contProgramCloser;
        private controllerGetSelectedData contDataGetter;
        private controllerManipulateSetting contSettingHandler;
        private controllerAddItem contItemAdder;
        private controllerEditItem contItemEditor;

        #endregion

        #region enumSettingInstaces

        private const EditorController.EditorSettings heightSettingEnum = EditorController.EditorSettings.editingWindowHeight;
        private const EditorController.EditorSettings widthSettingEnum = EditorController.EditorSettings.editingWindowWidth;
        private const EditorController.EditorSettings editingItemSettingEnum = EditorController.EditorSettings.itemEditorHeight;
        private const EditorController.EditorSettings listItemSettingEnum = EditorController.EditorSettings.listItemHeight;

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
                //if (value != null)
                //{
                //    itemEditorDataElement = value;
                //    OnPropertyChanged("itemEditorData");
                //}
            }
            get
            {
                return itemEditorDataElement;
            }
        }

        private CollectionView dataViewSource;

        public CollectionView dataView
        {
            set
            {
                if (value != null)
                {
                    dataViewSource = value;
                    OnPropertyChanged("dataView");
                }
            }
            get
            {
                return dataViewSource;
            }
        }



        //list of indeces for all select items in Listbox
        private int[] listBoxSelectedItems
        {
            get
            {
                try
                {
                    int[] someArray = new int[windowListbox.SelectedItems.Count];
                    for (int i = 0; i < windowListbox.SelectedItems.Count; i++)
                    {
                        someArray[i] = windowListbox.Items.IndexOf(windowListbox.SelectedItems[i]);
                    }
                    return someArray;
                }
                catch
                {
                    return null;
                }
            }
        }


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
            editorDataFunctions.retrieveDelegates(out contBinder, out contDataGetter, out contItemAdder, out contItemEditor);
            editorMiscFunctions.retrieveDelegates(out contFileOpener, out contSettingHandler, out contFileSaver);

            //windowListbox.ItemsSource = dataView;

            itemEditorDataElement = new xmlElem();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public void optionalInitialSetup()
        {
            try
            {
                String itemHeight = null;
                contSettingHandler(editingItemSettingEnum, ref itemHeight);
                itemEditorAreaResizer.Height = new GridLength(Convert.ToDouble(itemHeight));
            }
            catch
            {
                itemEditorAreaResizer.Height = new GridLength(1.0, GridUnitType.Auto);
            }

            try
            {
                String listItemHeight = null;
                contSettingHandler(listItemSettingEnum, ref listItemHeight);
                listItemHeightSlider.Value = Convert.ToDouble(listItemHeight);
            }
            catch
            {
                listItemHeightSlider.Value = listItemHeightSlider.Minimum;
            }

            contBinder(ref dataViewSource);
            dataView = dataViewSource;
            

            //itemEditorText2.Text = "None";
            //try
            //{
            //    contBinder(ref windowListbox);
            //    itemEditorText1.Text = "Selected Item Indeces: ";
            //    itemEditorText2.Text = "None";
            //}
            //catch
            //{
            //    contErrorDisplayer("Error trying to bind to list in constructor.");
            //}
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
            contFileOpener();

            contBinder(ref dataViewSource);
            dataView = dataViewSource;

            itemEditorText1.Text = "Selected Item Indeces: ";
            //itemEditorText2.Text = "None";
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void windowListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                xmlElem tempElement = null;
                contDataGetter(listBoxSelectedItems, ref itemEditorDataElement);
                itemEditorData = itemEditorDataElement;
                //listBoxItemViewer.Content = itemEditorData;
                debugWindow.Text = itemEditorData.ToString();
                //Binding myBinding = new Binding("itemEditorData");
                //myBinding.Mode = BindingMode.TwoWay;
                //myBinding.ElementName = "PrimaryWindow";
                //myBinding.Path = new PropertyPath(itemEditorData);
                //listBoxItemViewer.SetBinding(ScrollViewer.ContentProperty, myBinding);

                String temp = "";
                foreach (int index in listBoxSelectedItems)
                {
                    temp += index.ToString() + ", ";
                }
                itemEditorHeading = temp;
            }
            catch
            {
                //throw ex;
                //contErrorDisplayer("Error!");
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void listBoxSourceUpdatedHandler(object sender, DataTransferEventArgs e)
        {
            contErrorDisplayer("listBox source updated");
            //controllerReference.saveData();
            contFileSaver();
        }

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
        //public String currentFile()
        //{
        //    return currentFileFullPathLabel.Text;
        //}

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void itemEditorResized(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            try
            {
                String tempItemHeight = this.itemEditorAreaResizer.Height.Value.ToString();

                contSettingHandler(editingItemSettingEnum, ref tempItemHeight);
            }
            catch
            {
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void itemHeightSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                String temp = listItemHeightSlider.Value.ToString();
                contSettingHandler(listItemSettingEnum, ref temp, true);
            }
            catch
            {
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void saveItem(object sender, RoutedEventArgs e)
        {
            contItemEditor(this.itemEditorData, this.listBoxSelectedItems);

            contBinder(ref dataViewSource);
            dataView = dataViewSource;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void viewerItemTextChanged(object sender, TextChangedEventArgs e)
        {
            String tempObject = e.Source.ToString();

            String tempText = (e.Source as TextBox).Text;
        }

    }
}
