using SimpleWifi.Win32.Interop;
using SimpleWifi.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XTECHLavrinova1;

public class WifiScanner
{
    private readonly WlanClient _client;

    public WifiScanner(WlanClient client)
    {
        _client = client;
    }

    public List<WifiSSID_Model> GetAvailableWifiSSID_Models()
    {
        List<WifiSSID_Model> WifiSSID_Models = new List<WifiSSID_Model>();

        if (_client.NoWifiAvailable)
            return WifiSSID_Models;

        foreach (WlanInterface wlanIface in _client.Interfaces)
        {
            WlanAvailableNetwork[] rawNetworks = wlanIface.GetAvailableNetworkList(0);

            List<WlanAvailableNetwork> networks = new List<WlanAvailableNetwork>();

            foreach (WlanAvailableNetwork network in rawNetworks)
            {
                bool hasProfileName = !string.IsNullOrEmpty(network.profileName);
                bool anotherInstanceWithProfileExists = rawNetworks
                    .Where(n => n.Equals(network) && !string.IsNullOrEmpty(n.profileName))
                    .Any();

                if (!anotherInstanceWithProfileExists || hasProfileName)
                    networks.Add(network);
            }

            foreach (WlanAvailableNetwork network in networks)
            {
                var WifiSSID_Model = new WifiSSID_Model
                {
                    Name_SSID = Encoding.ASCII.GetString(network.dot11Ssid.SSID, 0, (int)network.dot11Ssid.SSIDLength),
                    Wifi_Status = (int)network.wlanSignalQuality
                };
                WifiSSID_Models.Add(WifiSSID_Model);
            }
        }

        return WifiSSID_Models;
    }
}
