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
        private string connectionString = "Data Source=DESKTOP-8J6N5LS\\SQLEXPRESS;Initial Catalog=XTECTWifi_Database;Persist Security Info=True;User ID=sa;Password=72154;Encrypt=True;TrustServerCertificate=True";
        private static WlanClient _client = new WlanClient();
        private WifiScanner wifiScanner = new WifiScanner(_client);
        private List<WifiSSID_Model> networks = new List<WifiSSID_Model>();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @" select
	                        [Name_SSID] as 'Имя сети',
	                        [Wifi_Status] as 'Уровень сигнала сети',
	                        [Date_Add] as 'Дата сохранения сети'
                        from Wifi_SSID";

                using (var command = new SqlCommand(query, connection))
                {
                    var dataTable = new DataTable();
                    new SqlDataAdapter(command).Fill(dataTable);
                    dtWiFi.ItemsSource = dataTable.DefaultView;
                }
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            List<WifiSSID_Model> networks = wifiScanner.GetAvailableWifiSSID_Models();
            dtWiFi.ItemsSource = networks;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход из приложения", MessageBoxButton.YesNo)){
                case MessageBoxResult.Yes:
                    Close();
                    break;
            };
        }
    }
}
