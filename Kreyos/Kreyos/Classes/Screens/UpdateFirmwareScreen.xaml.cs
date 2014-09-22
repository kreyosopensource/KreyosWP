using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Kreyos.Classes.Screens
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;

    public partial class UpdateFirmwareScreen : PhoneApplicationPage
    {
        /****************************************************************
         * Properties
         **/
        private List<DeviceData> m_devices;
        private LongListSelector m_scrollerDevices;
        private Button m_btnUpdate;
        private TextBlock m_txtNoDevicesFound;

        /****************************************************************
         * Constructors
         **/
        public UpdateFirmwareScreen ()
        {
            InitializeComponent();
            m_devices = new List<DeviceData>();
            m_scrollerDevices = this.lls_devices;
            m_btnUpdate = this.btn_update;
            m_txtNoDevicesFound = this.txt_no_device_found;
            m_txtNoDevicesFound.Visibility = Visibility.Collapsed;

            //~~~Setup delegates
            BluetoothObserver.Instance.OnReceivedEvent += new Delegate_HandleCommand(this.HandleCommand);

            //~~~Check Connected Devices
            if (BluetoothManager.Instance.IsConnected)
            {
                m_devices.Add(new DeviceData(BluetoothManager.Instance.DeviceName));
                this.UpdateDevices();
            }
            else
            {
                this.FetchDevices();
            }
        }

        /****************************************************************
         * Public Functionalities
         **/
        public void UpdateDevices ()
        {
            m_scrollerDevices.ItemsSource = null;
            m_scrollerDevices.UpdateLayout();
            m_scrollerDevices.ItemsSource = m_devices;
        }

        /// <summary>
        /// Clear status of all the currently searched devices
        /// </summary>
        public void ClearDevices ()
        {
            foreach (DeviceData device in m_devices)
            {
                device.UpdateStatus(EDevice.Status_Neutral);
            }
        }

        public void FetchDevices ()
        {
            BluetoothManager.Instance.GetKreyosDevices();
        }

        /****************************************************************
         * Bluetooth Delegate Methods
         **/
        private void HandleCommand (ObserverInfo p_command)
        {
            switch (p_command.Command)
            {
                case EBTEvent.BTE_OnFetchedDevices:
                {
                    this.OnFetchedKreyosDevices(p_command.Devices);
                }
                break;

                case EBTEvent.BTE_OnDeviceConnected:
                {
                    this.OnDeviceConnected(p_command.Device);
                }
                break;

                case EBTEvent.BTE_OnDeviceDisconnected:
                {
                    this.OnDeviceDisconnected(p_command.Error);
                }
                break;
            }
        }

        /// <summary>
        /// Getting the list of detected bluetooth devices
        /// </summary>
        /// <param name="p_devices"></param>
        private void OnFetchedKreyosDevices (List<string> p_devices)
        {
            m_devices.Clear();

            if (p_devices != null && p_devices.Count > 0)
            {
                m_devices = new List<DeviceData>();

                foreach (string name in p_devices)
                {
                    m_devices.Add(new DeviceData(name));
                }

                this.UpdateDevices();
            }
        }

        /// <summary>
        /// Connection to a bluetooth device is initiated.
        /// </summary>
        private void OnDeviceConnected (string p_device)
        {
            KreyosUtils.Log("BluetoothTest::OnDeviceConnected"," device:" + p_device);
            DeviceData selectedDevice = DeviceData.DeviceFromName(m_devices, p_device);
            selectedDevice.UpdateStatus(EDevice.Status_Connected);
            this.UpdateDevices();
        }

        /// <summary>
        /// Connection with the watch is disconnected.
        /// </summary>
        private void OnDeviceDisconnected (string p_error)
        {
            KreyosUtils.Log("BluetoothTest::OnDeviceDisconnected", "message:" + p_error);
            this.ClearDevices();
            this.UpdateDevices();
            BluetoothManager.Instance.GetKreyosDevices();
        }

        /****************************************************************
         * UI Callbacks
         **/
        /// <summary>
        /// Check of Updates button callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUpdateWatch (object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!BluetoothManager.Instance.IsConnected) 
            {
                KreyosUtils.Log("UpdateFirmwareScreen::OnUpdateWatch","Please connect your phone to your Kreyos watch.");
                return;
            }

            BluetoothManager.Instance.UpdateWatch();
        }

        /// <summary>
        /// LongListSelected Event that is triggered when an Item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSelected (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) { return; }

            DeviceData selectedDevice = (DeviceData)e.AddedItems[0];

            // check if device is already connected and it was the selected. disconnect the device
            if (selectedDevice.DeviceStatus == EDevice.Status_Connected)
            {
                // disconnect the device
                selectedDevice.UpdateStatus(EDevice.Status_Disconnected);
                BluetoothManager.Instance.DisconnectKreyosDevice();
                return;
            }

            // check if the app is currently connected to a watch
            if (BluetoothManager.Instance.IsConnected)
            {
                // disconnect the device
                BluetoothManager.Instance.DisconnectKreyosDevice();
            }

            selectedDevice.UpdateStatus(EDevice.Status_Connecting);

            // update device list
            this.UpdateDevices();

            KreyosUtils.Log("BluetoothTest::OnDeviceSelected", "cellData:" + selectedDevice);

            // Connected to Watch
            BluetoothManager.Instance.ConnectKreyosDevice(selectedDevice.Name);
        }
    }
}