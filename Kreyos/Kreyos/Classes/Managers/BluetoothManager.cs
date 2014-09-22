using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Windows.Storage.Streams;

namespace Kreyos.Classes.Managers
{
    using Kreyos.SDK.Bluetooth;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Screens;
    using Kreyos.Classes.Utils;
    using Kreyos.Classes.Components;
    
    public sealed class BluetoothManager
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile BluetoothManager m_instance = null;
        private static object m_lockObject                  = new Object();

        /****************************************************************
         * Singleton
         **/
        public static BluetoothManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new BluetoothManager(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Constants
         **/
        public static readonly string UPDATE_URL = "http://freebsd.cloudapp.net/~howardsu/upgrade2.bin";

        /****************************************************************
         * Instance Properties
         **/

        /****************************************************************
         * Constructors
         **/
        private BluetoothManager ()
        {
        }

        /****************************************************************
         * Getters | Setters
         **/
        public bool IsConnected { get { return BluetoothAgent.Instance.Connected; } }
        public string DeviceName { get { return BluetoothAgent.Instance.PeeredDevice.DisplayName; } }


        /****************************************************************
         * Bluetooth Functionalities
         *  Note: p_delegate should never be null
         **/
        public async void GetKreyosDevices ()
        {
            // Get List of devices from BluetoothAgent
            var devices = await BluetoothAgent.GetPairedDevices();
            List<string> kreyosDevices = new List<string>();

            foreach (var device in devices)
            {
                kreyosDevices.Add((string)device);
            }

            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnFetchedDevices;
            info.Devices = kreyosDevices;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnFetchedDevices, info);
        }

        public async void ConnectKreyosDevice (string p_device)
        {
            if (string.IsNullOrWhiteSpace(p_device))
            {
                ObserverInfo info = new ObserverInfo();
                info.Command = EBTEvent.BTE_OnDeviceDisconnected;
                info.Error = "Error! Invalid device name!";
                BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnDeviceDisconnected, info);
                return;
            }

            var succ = await BluetoothAgent.Instance.Start(p_device);

            if (succ)
            {
                BluetoothAgent.Instance.InnerProtocol.OnElementParsed               += OnElementParsed;
                BluetoothAgent.Instance.InnerProtocol.OnFileReceived                += OnFileReceived;
                BluetoothAgent.Instance.InnerProtocol.OnDeviceIDRead                += OnDeviceIDRead;
                BluetoothAgent.Instance.InnerProtocol.OnActivityDataRead            += OnActivityDataRead;
                BluetoothAgent.Instance.InnerProtocol.OnSportsGridRead              += OnSportsGridRead;
                BluetoothAgent.Instance.InnerProtocol.OnActivityDataEnd             += OnActivityDataEnd;
                BluetoothAgent.Instance.InnerProtocol.OnActivityDataPrepared        += OnActivityDataPrepared;
                BluetoothAgent.Instance.InnerProtocol.OnActivityDataSync            += OnActivityDataSync;
                BluetoothAgent.Instance.InnerProtocol.OnFirmwareVersionRequest      += OnFirmwareVersionRequest;

                BluetoothAgent.Instance.InnerProtocol.OnOverallActivitiesReceived   += OnOverallActivitiesReceived;
                BluetoothAgent.Instance.InnerProtocol.OnTodayActivityReceived       += OnTodayActivityReceived;
                BluetoothAgent.Instance.InnerProtocol.OnSportsDataReceived          += OnSportsDataReceived;

                ObserverInfo info = new ObserverInfo();
                info.Command = EBTEvent.BTE_OnDeviceConnected;
                info.Device = p_device;
                BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnDeviceConnected, info);
                return;
            }
            else
            {
                switch (BluetoothAgent.Instance.ConnectInfo)
                {
                    case EBTConnect.BluetoothIsOff:
                    {
                        var result = MessageBox.Show("Bluetooth is turned off. To see the current Bluetooth settings tap 'ok'", "Bluetooth Off", MessageBoxButton.OKCancel);
                        if (result == MessageBoxResult.OK)
                        {
                            // TODO: Show the bluetooth control panel here
                            ConnectionSettingsTask connectionSettingsTask = new ConnectionSettingsTask();
                            connectionSettingsTask.ConnectionSettingsType = ConnectionSettingsType.Bluetooth;
                            connectionSettingsTask.Show();
                        }
                    }
                    break;

                    case EBTConnect.CapPermission:
                    {
                        MessageBox.Show("To run this app, you must have ID_CAP_PROXIMITY enabled in WMAppManifest.xaml");
                    }
                    break;
                }

                ObserverInfo info = new ObserverInfo();
                info.Command = EBTEvent.BTE_OnDeviceDisconnected;
                info.Error = BluetoothAgent.Instance.ErrorMessage;
                BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnDeviceDisconnected, info);
            }
        }

        public void DisconnectKreyosDevice ()
        {
            BluetoothAgent.Instance.Stop();
        }

        public void UpdateWatch ()
        {
            var client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(this.OpenReadCompleted);
            client.OpenReadAsync(new Uri(UPDATE_URL));
        }

        public void GetActivityData ()
        {
            BluetoothAgent.Instance.InnerProtocol.getActivityData();
        }

        public void UnlockWatch() 
        {
            BluetoothAgent.Instance.InnerProtocol.unlockWatch();
        }

        public void SyncWatch() 
        {
            if (!IsConnected) 
            {
                return;
            }
            DeviceConfiguration config = DeviceConfigManager.Instance.GetDeviceConfig();
            BluetoothAgent.Instance.InnerProtocol.syncWatchConfig(config.WorldClockTable,
                                                                    config.WorldClockOffset,
                                                                    config.IsDigitalClock,
                                                                    config.DigitalClock,
                                                                    config.AnalogClock,
                                                                    config.SportsGridType,
                                                                    config.SportsGridValues,
                                                                    config.Goals,
                                                                    config.Weight,
                                                                    config.Height,
                                                                    config.IsEnableGesture,
                                                                    config.IsLeftHandGesture,
                                                                    config.Actionstable,
                                                                    config.IsUkUnit);

            KreyosUtils.Log("Protocol", "Sync Watch");
        }

        public void SetAlarm(int p_index, bool p_bIsEnable, int p_hour, int p_minute)
        {
            if (!IsConnected)
            {
                return;
            }

            // int index 
            // int mode
            // int monthday
            // int weekday
            // int hour
            // int minute
            // syncWatchAlarmConf(index, mEnableAlarm, mEnableVibrate, 0, 0, valueHour, valueMinute);

            int mode = p_bIsEnable ? 0x04 : 0x01;
            BluetoothAgent.Instance.InnerProtocol.setWatchAlarm(p_index, mode, 0, 0, p_hour, p_minute);
            KreyosUtils.Log("Protocol", "Set Alarm");
        }

        public void SetDateTime(DateTime p_dateTime)
        {
            if (!IsConnected)
            {
                return;
            }

            BluetoothAgent.Instance.InnerProtocol.syncTimeFromInput(p_dateTime);
            KreyosUtils.Log("Protocol", "Set Date and Time");
        }

        public void SyncDateTime() 
        {
            if (!IsConnected)
            {
                return;
            }

            BluetoothAgent.Instance.InnerProtocol.syncTime();
            KreyosUtils.Log("Protocol", "Sync Date and Time");
        }

        public void GetOverallActivities () 
        {
            if (!IsConnected)
            {
                return;
            }

            BluetoothAgent.Instance.InnerProtocol.getActivityData();
            KreyosUtils.Log("Protocol", "Get Overall Activity Data");
        }

        public void GetTodayActivity()
        {
            if (!IsConnected)
            {
                return;
            }

            BluetoothAgent.Instance.InnerProtocol.sendDailyActivityRequest();
            KreyosUtils.Log("Protocol", "Get Today Activity Data");
        }

        /****************************************************************
         * Delegates
         **/
        private void OnElementParsed (ElementDesc p_element)
        {
            KreyosUtils.Log("BluetoothManager::OnElementParsed", "Type:" + p_element.ElementType + " Data:" + string.Join(" ", p_element.Data));
        }

        private void OnFileReceived(string fileName)
        {
            KreyosUtils.Log("BluetoothManager::OnFileReceived", "fileName:" + fileName);
        }

        private void OnDeviceIDRead(string fileName)
        {
            KreyosUtils.Log("BluetoothManager::OnDeviceIDRead", "fileName:" + fileName);
        }

        private void OnActivityDataRead(byte[] data)
        {
            KreyosUtils.Log("BluetoothManager::OnActivityDataRead", "data:" + data);
        }

        private void OnSportsGridRead(byte[] data)
        {
            KreyosUtils.Log("BluetoothManager::OnSportsGridRead", "data:" + data);
        }

        /// <summary>
        /// Switch to Sports Mode
        /// </summary>
        /// <param name="data"></param>
        private void OnActivityDataPrepared(byte[] data)
        {
            KreyosUtils.Log("BluetoothManager::OnActivityDataPrepared", "data:" + data);
            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnReadySportsMode;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnReadySportsMode, info);
        }

        /// <summary>
        /// Sports Mode is done.
        /// </summary>
        /// <param name="data"></param>
        private void OnActivityDataEnd(byte[] data)
        {
            KreyosUtils.Log("BluetoothManager::OnActivityDataEnd", "data:" + data);
            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnFinishSportsMode;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnFinishSportsMode, info);
        }

        private void OnActivityDataSync(byte[] data)
        {
            KreyosUtils.Log("BluetoothManager::OnActivityDataSync", "data:" + data);
        }

        private void OnFirmwareVersionRequest(string fileName)
        {
            KreyosUtils.Log("BluetoothManager::OnFirmwareVersionRequest", "fileName:" + fileName);
        }

        private void OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                BluetoothAgent.Instance.InnerProtocol.sendStream("firmware", e.Result);
            }
            catch (WebException exc)
            {
                KreyosUtils.Log("BluetoothManager::OpenReadCompleted", "Error: " + exc.Message );
            }
        }

        private void OnOverallActivitiesReceived (ActivityDataDoc doc)
        {
            KreyosUtils.Log("BluetoothManager::OnOverallActivitiesReceived", "");
            
            //~~~Display overall data
            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnOverallActivity;
            info.OverallData = doc;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnOverallActivity, info);
        }

        private void OnTodayActivityReceived (TodayActivity ta)
        {
            KreyosUtils.Log("BluetoothManager::OnTodayActivityReceived", "");
            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnTodaysActivity;
            info.TodaysData = ta;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnTodaysActivity, info);
        }

        private void OnSportsDataReceived(SportsDataRow sport)
        {
            KreyosUtils.Log("BluetoothManager::OnSportsDataReceived", "");
            //* Logs
            KreyosUtils.Log("Mode:", "" + sport.sports_mode);
            KreyosUtils.Log("Time:", "" + sport.seconds_elapse);
            foreach (KeyValuePair<SportsDataRow.DataType, double> pair in sport.data)
            {
                KreyosUtils.Log("Data", "" + pair.Key + ":" + pair.Value);
            }
            //*/

            ObserverInfo info = new ObserverInfo();
            info.Command = EBTEvent.BTE_OnStartSportsMode;
            info.SportsData = sport;
            BluetoothObserver.Instance.Trigger(EBTEvent.BTE_OnStartSportsMode, info);
        }
    }

    /****************************************************************
     * Bluetooth Events
     **/
}
