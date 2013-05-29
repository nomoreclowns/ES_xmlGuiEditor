using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ES_XML_Editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void textChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void textChanged(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox theSender = sender as TextBox;

            if (theSender.Text.Contains('<') || theSender.Text.Contains('>'))
            {
                e.Handled = true;
                theSender.Undo();
            }

        }
    }
}
