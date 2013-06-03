using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private controllerOpenFile contFileOpener;
        private controllerBind contBinder;
        private controllerShowError contErrorDisplayer;
        private controllerSave contFileSaver;
        private controllerClose contProgramCloser;
        private controllerGetSelectedData contDataGetter;
        private controllerManipulateSetting contSettingHandler;

        private xmlElement editingItem;

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        //public MainWindow(EditorController owner)
        //{
        //    InitializeComponent();

        //    if (owner == null)
        //    {
        //        throw new NullReferenceException("MainWindow Constructor cannot have a null reference to program controller");
        //    }
        //    controllerReference = owner;

        //}

        public MainWindow(controllerDelegateContainer editorFunctions)
        {
            InitializeComponent();

            if (editorFunctions == null)
            {
                throw new NullReferenceException("MainWindow Constructor cannot have a null reference to editorFunctions");
            }
            editorFunctions.retrieveDelegates(out contErrorDisplayer, out contFileOpener, out contBinder, out contDataGetter, out contSettingHandler, out contFileSaver, out contProgramCloser);

        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
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
        private void closingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            contProgramCloser();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void openFileButtonClick(object sender, RoutedEventArgs e)
        {
            String theFile;

            String fullPath;

            contFileOpener(out theFile, out fullPath);
            
            contBinder(ref windowListbox);

            //currentFileLabel.Text = theFile;
            //currentFileFullPathLabel.Text = fullPath;
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void windowListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                contDataGetter(listBoxSelectedItems, ref editingItem);
            }
            catch
            {
                //currentFileLabel.Text = "Error";
            }
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void listBoxSourceUpdatedHandler(object sender, DataTransferEventArgs e)
        {
            //controllerReference.saveData();
            contFileSaver();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public String currentFile()
        {
            return currentFileFullPathLabel.Text;
        }

        

    }
}
