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

namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for CustomListbox.xaml
    /// </summary>
    public partial class CustomListbox : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler changedHandler = PropertyChanged;

            if (changedHandler != null)
            {
                changedHandler(this, new PropertyChangedEventArgs(name));
            }
        }

        private int[] listBoxSelectedItems
        {
            get
            {
                try
                {
                    int[] someArray = new int[tabItemListbox.SelectedItems.Count];
                    for (int i = 0; i < tabItemListbox.SelectedItems.Count; i++)
                    {
                        someArray[i] = tabItemListbox.Items.IndexOf(tabItemListbox.SelectedItems[i]);
                    }
                    return someArray;
                }
                catch
                {
                    return null;
                }
            }
        }

        private CollectionView pBindableView;

        public CollectionView dataView
        {
            get
            {
                return pBindableView;
            }
        }

        //public CustomListbox()
        //{
        //    InitializeComponent();
        //}

        public CustomListbox(ref CollectionView iBindableView)
        {
            InitializeComponent();

            pBindableView = iBindableView;
            //tabItemListbox.ItemsSource = dataView;
            OnPropertyChanged("dataView");
        }

        private void listboxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        
    }
}
