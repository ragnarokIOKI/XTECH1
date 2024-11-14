using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using SimpleWifi.Win32;
using SimpleWifi.Win32.Interop;

namespace XTECHLavrinova1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScanCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.Scan();
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.Save();
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as MainViewModel;
            viewModel.Exit();
            Close();
        }
    }
}
