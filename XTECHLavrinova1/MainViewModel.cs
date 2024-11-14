using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SimpleWifi.Win32;

namespace XTECHLavrinova1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string connectionString = "Data Source=DESKTOP-8J6N5LS\\SQLEXPRESS;Initial Catalog=XTECTWifi_Database;Persist Security Info=True;User ID=sa;Password=72154;Encrypt=True;TrustServerCertificate=True";
        private static WlanClient _client = new WlanClient();
        private WifiScanner wifiScanner = new WifiScanner(_client);

        public ObservableCollection<WifiSSID_Model> Networks { get; set; }
        public static RoutedCommand ScanCommand { get; } = new RoutedCommand("ScanCommand", typeof(MainViewModel));
        public static RoutedCommand SaveCommand { get; } = new RoutedCommand("SaveCommand", typeof(MainViewModel));
        public static RoutedCommand ExitCommand { get; } = new RoutedCommand("ExitCommand", typeof(MainViewModel));

        private string _bestSSID;
        public string BestSSID
        {
            get => _bestSSID;
            set
            {
                _bestSSID = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Networks = new ObservableCollection<WifiSSID_Model>();
            LoadData();
        }

        private void LoadData()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = @"select * from Wifi_SSID";

                using (var command = new SqlCommand(query, connection))
                {
                    var dataTable = new DataTable();
                    new SqlDataAdapter(command).Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        Networks.Add(new WifiSSID_Model
                        {
                            Name_SSID = row["Name_SSID"].ToString(),
                            Wifi_Status = Convert.ToInt32(row["Wifi_Status"])
                        });
                    }
                }
            }
        }

        public void Scan()
        {
            try
            {
                var networks = wifiScanner.GetAvailableWifiSSID_Models();
                Networks.Clear();
                foreach (var network in networks)
                {
                    Networks.Add(network);
                }
                BestSSID = networks.OrderByDescending(n => n.Wifi_Status).FirstOrDefault()?.Name_SSID;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Сканирование сетей");
            }
        }

        public void Save()
        {
            if (Networks.Count != 0)
            {
                var query = @"
                        INSERT INTO Wifi_SSID
                        (Name_SSID, Wifi_Status)
                        VALUES
                        (@Name_SSID, @Wifi_Status)";

                try
                {
                    foreach (var network in Networks)
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

        public void Exit()
        {
            switch (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход из приложения", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    Application.Current.Shutdown();
                    break;
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
