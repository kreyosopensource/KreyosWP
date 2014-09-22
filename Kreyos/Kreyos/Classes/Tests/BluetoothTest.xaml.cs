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

namespace Kreyos.Classes.Tests
{
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;

    public partial class BluetoothTest : PhoneApplicationPage
    {
        private TextBlock m_txtBTStatus;

        public BluetoothTest()
        {
            InitializeComponent();
            m_txtBTStatus = this.txt_connection_status;
            m_txtBTStatus.Text = "Not Connected";

            //~~~Setup delegates
            BluetoothObserver.Instance.OnReceivedEvent  += new Delegate_HandleCommand(this.HandleCommand);
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
            if (p_devices != null && p_devices.Count > 0)
            {
                List<BTCellData> listData = new List<BTCellData>();

                foreach (string name in p_devices)
                {
                    listData.Add(new BTCellData(name));
                }

                this.lls_devices.ItemsSource = listData;
            }
        }

        /// <summary>
        /// Connection to a bluetooth device is initiated.
        /// </summary>
        private void OnDeviceConnected (string p_device)
        {
            Debug.WriteLine("BluetoothTest::OnDeviceConnected device:" + p_device);
            m_txtBTStatus.Text = "Connected: " + p_device + "";
        }

        /// <summary>
        /// Connection with the watch is disconnected.
        /// </summary>
        private void OnDeviceDisconnected (string p_error)
        {
            KreyosUtils.Log("BluetoothTest::OnDeviceDisconnected", "message:" + p_error);
            m_txtBTStatus.Text = "Disconnected";
        }

        /****************************************************************
         * UI Events
         **/
        private void OnFetchDevices(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BluetoothManager.Instance.GetKreyosDevices();
        }

        /// <summary>
        /// Button Events. ( TAP )
        /// Don't use this, It will be difficult the get the data of a selected Item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectDevice(object sender, System.Windows.Input.GestureEventArgs e)
        {
            KreyosUtils.Log("BluetoothTest::OnSelectDevice", "sender:" + sender);
        }

        /// <summary>
        /// LongListSelected Event that is triggered when an Item is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDeviceSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) { return; }

            BTCellData cellData = (BTCellData)e.AddedItems[0];

            KreyosUtils.Log("BluetoothTest::OnDeviceSelected", "cellData:" + cellData);

            // Connected to Watch
            BluetoothManager.Instance.ConnectKreyosDevice(cellData.Name);
        }

        private void OnFetchActivityStats(object sender, System.Windows.Input.GestureEventArgs e)
        {
            BluetoothManager.Instance.GetActivityData();
        }
    }

    public class BTCellData
    {
        public BTCellData(string p_name)
        {
            this.Name = p_name;
        }

        public string Name { get; private set; }
    }
}