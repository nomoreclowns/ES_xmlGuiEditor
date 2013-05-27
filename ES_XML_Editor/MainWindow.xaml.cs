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
        EditorController controllerReference;

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        public MainWindow(EditorController owner)
        {
            InitializeComponent();

            if (owner == null)
            {
                throw new NullReferenceException("MainWindow Constructor cannot have a null reference to program controller");
            }
            controllerReference = owner;
            
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void closingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            controllerReference.closeProgram();
        }

        /* ************************************************************************************************************************
         *************************************************************************************************************************/
        private void openFileButtonClick(object sender, RoutedEventArgs e)
        {
            String theFile;

            controllerReference.openFile(out theFile);
        }

    }
}
