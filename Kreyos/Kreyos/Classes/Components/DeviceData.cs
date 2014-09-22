using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Components
{
    using Kreyos.Classes.Managers;

    public enum EDevice
    {
        Status_Neutral,
        Status_Connecting,
        Status_Connected,
        Status_Disconnected,
    };

    public class DeviceData
    {
        /****************************************************************
         * Constants
         **/
        /// <summary>
        /// Device Label Font Weight STATUS. the data is binded to the Template of Bluetooth screen
        /// </summary>
        public static readonly string FONT_NORMAL           = "Normal";
        public static readonly string FONT_BOLD             = "Bold";

        /// <summary>
        /// Device status data. the data is binded to the Template of Bluetooth screen
        /// </summary>
        public static readonly string STATUS_NEUTRAL        = "tap to pair";
        public static readonly string STATUS_CONNECTING     = "connecting...";
        public static readonly string STATUS_CONNECTED      = "connected";
        public static readonly string STATUS_DISCONNECTED   = "disconnected";

        /****************************************************************
         * Constructors
         **/
        public DeviceData (string p_name)
        {
            this.Name = p_name;

            if (BluetoothManager.Instance.IsConnected && BluetoothManager.Instance.DeviceName.Equals(p_name))
            {
                this.UpdateStatus(EDevice.Status_Connected);
            }
            else
            {
                this.UpdateStatus(EDevice.Status_Neutral);
            }
        }

        /****************************************************************
         * Public Functions
         **/
        public void UpdateStatus (EDevice p_status)
        {
            this.DeviceStatus = p_status;

            switch (p_status)
            {
                case EDevice.Status_Neutral:
                {
                    this.Status = STATUS_NEUTRAL;
                    this.FontWeight = FONT_NORMAL;
                }
                break;
                case EDevice.Status_Connecting:
                {
                    this.Status = STATUS_CONNECTING;
                    this.FontWeight = FONT_NORMAL;
                }
                break;
                case EDevice.Status_Connected:
                {
                    this.Status = STATUS_CONNECTED;
                    this.FontWeight = FONT_BOLD;
                }
                break;
                case EDevice.Status_Disconnected:
                {
                    this.Status = STATUS_DISCONNECTED;
                    this.FontWeight = FONT_NORMAL;
                }
                break;
            }
        }

        /****************************************************************
         * Getter | Setters
         **/
        public EDevice DeviceStatus { get; private set; }
        public int Index { get; set; }

        /// <summary>
        /// These methods are binded to UI of BluetoothScreen template.
        /// </summary>
        public string Name { get; private set; }
        public string Status { get; private set; }
        public string FontWeight { get; private set; }

        /****************************************************************
         * Helpers
         **/
        /*
        DeviceData selectedDevice = (DeviceData)(e.AddedItems[0]);
            selectedDevice = m_devices.Find(delegate(DeviceData device)
            {
                return device.Name == selectedDevice.Name;
            });
        //*/
        public static DeviceData DeviceFromName (List<DeviceData> p_list, string p_name)
        {
            if (p_list == null || p_list.Count == 0) { return null; }

            DeviceData selectedDevice = p_list.Find(delegate(DeviceData device)
            {
                return device.Name == p_name;
            });

            return selectedDevice;
        }

        public static DeviceData DeviceFromIndex (List<DeviceData> p_list, int p_index)
        {
            if (p_list == null || p_list.Count == 0) { return null; }

            DeviceData selectedDevice = p_list.Find(delegate(DeviceData device)
            {
                return device.Index == p_index;
            });

            return selectedDevice;
        }
    }
}
