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
                var query = @" select * from Wifi_SSID";

                using (var command = new SqlCommand(query, connection))
                {
                    var dataTable = new DataTable();
                    new SqlDataAdapter(command).Fill(dataTable);
                    dtWiFi.ItemsSource = dataTable.DefaultView;
                }
            }
        }

        private void ExecuteNonQuery(string query, Action<SqlCommand> addParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    addParameters(command);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                networks = wifiScanner.GetAvailableWifiSSID_Models();
                dtWiFi.ItemsSource = networks;
                lbBestSSID.Content = networks.OrderByDescending(n => n.Wifi_Status).FirstOrDefault().Name_SSID;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message.ToString(), "Сканирование сетей");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (networks.Count != 0)
            {
                var query = @"
                        INSERT INTO Wifi_SSID
                        (Name_SSID, Wifi_Status)
                        VALUES
                        (@Name_SSID, @Wifi_Status)";

                try
                { 
                    foreach (var network in networks)
                    {
                        ExecuteNonQuery(query, command =>
                        {
                            command.Parameters.AddWithValue("@Name_SSID", network.Name_SSID);
                            command.Parameters.AddWithValue("@Wifi_Status", network.Wifi_Status);
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "Сканирование сетей");
                }

                MessageBox.Show("Данные успешно сохранены!", "Сканирование сетей");
            }
            else
            {
                MessageBox.Show("Нет данных для сохранения!", "Сканирование сетей");
            }
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
