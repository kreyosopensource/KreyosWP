using System;
using System.Diagnostics;
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

    public partial class BluetoothScreen : PhoneApplicationPage
    {
        /****************************************************************
         * Properties
         **/
        private List<DeviceData> m_devices;
        private LongListSelector m_scrollerDevices;
        private Button m_btnSearch;


        /****************************************************************
         * Constructors
         **/
        public BluetoothScreen ()
        {
            InitializeComponent();
            m_devices = new List<DeviceData>();
            m_scrollerDevices = this.lls_devices;
            m_btnSearch = this.btn_search;

            //~~~Setup delegate
            BluetoothObserver.Instance.OnReceivedEvent += new Delegate_HandleCommand(this.HandleCommand);

            this.CheckConnectedDevices();
        }

        /****************************************************************
         * Public Functions
         **/
        public void UpdateDevices ()
        {
            List<DeviceData> test = new List<DeviceData>();
            foreach (DeviceData i in m_devices)
            {
                DeviceData device = new DeviceData(i.Name);
                device.UpdateStatus(i.DeviceStatus);
                test.Add(device);
            }
            m_scrollerDevices.ItemsSource = test;
            m_scrollerDevices.UpdateLayout();
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

        public void CheckConnectedDevices ()
        {
            BluetoothManager.Instance.GetKreyosDevices();
        }

        /****************************************************************
         * Bluetooth Delegate Methods
         **/
        private void HandleCommand(ObserverInfo p_command)
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
                    DeviceData device = new DeviceData(name);
                    device.Index = m_devices.Count;
                    m_devices.Add(device);
                }

                this.UpdateDevices();
            }
        }

        /// <summary>
        /// Connection to a bluetooth device is initiated.
        /// </summary>
        private void OnDeviceConnected (string p_device)
        {
            Debug.WriteLine("BluetoothTest::OnDeviceConnected device:" + p_device);
            DeviceData deviceSelected = DeviceData.DeviceFromName(m_devices, p_device);
            deviceSelected.UpdateStatus(EDevice.Status_Connected);
            this.UpdateDevices();
            // BluetoothManager.Instance.AppMainScreen.InitActivityStatsData(); // Temp D:
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
         * UI Events
         **/
        private void OnFetchDevices (object sender, System.Windows.Input.GestureEventArgs e)
        {
            BluetoothManager.Instance.GetKreyosDevices();
        }

        /// <summary>
        /// LongListSelected Event that is triggered when an Item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSelected (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            if (e.AddedItems.Count <= 0) { return; }

            DeviceData selectedDevice = (DeviceData)(e.AddedItems[0]);
            selectedDevice = DeviceData.DeviceFromName(m_devices, selectedDevice.Name);

            selectedDevice.UpdateStatus(EDevice.Status_Connecting);
            
            //~~~check if device is already connected
            if (selectedDevice.DeviceStatus == EDevice.Status_Connected)
            {
                // disconnect the device
                selectedDevice.UpdateStatus(EDevice.Status_Disconnected);
                BluetoothManager.Instance.DisconnectKreyosDevice();
                return;
            }

            //~~~check if the app is currently connected to a watch
            if (BluetoothManager.Instance.IsConnected)
            {
                //~~~disconnect the device
                BluetoothManager.Instance.DisconnectKreyosDevice();
            }

            selectedDevice.UpdateStatus(EDevice.Status_Connecting);

            //~~~Update device list
            this.UpdateDevices();

            KreyosUtils.Log("BluetoothTest::OnDeviceSelected", "cellData:" + selectedDevice);

            //~~~Connected to Watch
            BluetoothManager.Instance.ConnectKreyosDevice(selectedDevice.Name);
        }
    }
}