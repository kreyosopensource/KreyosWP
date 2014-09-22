using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Managers
{
    using Kreyos.SDK.Bluetooth;

    /****************************************************************
     * Event Enum
     **/
    public enum EBTEvent
    {
        BTE_Invalid = -1,
        BTE_OnFetchedDevices,
        BTE_OnDeviceConnected,
        BTE_OnDeviceDisconnected,
        BTE_OnTodaysActivity,
        BTE_OnOverallActivity,
        BTE_OnReadySportsMode, 
        BTE_OnStartSportsMode,
        BTE_OnFinishSportsMode,
    };

    public class ObserverInfo
    {
        public EBTEvent Command                 = EBTEvent.BTE_Invalid;
        public List<string> Devices             = null;
        public string Device                    = null;
        public string Error                     = null;
        public TodayActivity TodaysData         = null;
        public ActivityDataDoc OverallData      = null;
        public SportsDataRow SportsData         = null;
    };

    /****************************************************************
     * Delegates
     **/
    public delegate void Delegate_HandleCommand (ObserverInfo p_info);

    /****************************************************************
     * Observer Class
     **/
    public sealed class BluetoothObserver
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile BluetoothObserver m_instance = null;
        private static object m_lockObject = new Object();

        /****************************************************************
         * Singleton
         **/
        public static BluetoothObserver Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new BluetoothObserver(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Bluetooth Events
         **/
        public event Delegate_HandleCommand OnReceivedEvent;

        /****************************************************************
         * Constructors
         **/
        private BluetoothObserver ()
        {
        }

        /****************************************************************
         * Public Functionalities
         **/
        public void Trigger (EBTEvent p_event, ObserverInfo p_info = null)
        {
            switch (p_event)
            {
                case EBTEvent.BTE_OnFetchedDevices:
                case EBTEvent.BTE_OnDeviceConnected:
                case EBTEvent.BTE_OnDeviceDisconnected:
                case EBTEvent.BTE_OnTodaysActivity:
                case EBTEvent.BTE_OnOverallActivity:
                case EBTEvent.BTE_OnReadySportsMode:
                case EBTEvent.BTE_OnStartSportsMode:
                case EBTEvent.BTE_OnFinishSportsMode:
                {
                    //~~~Do something you need to handle before triggering the event
                }
                break;
            }

            //~~~trigger event
            if (this.OnReceivedEvent == null) { return; }
            this.OnReceivedEvent(p_info);
        }
    }
}
