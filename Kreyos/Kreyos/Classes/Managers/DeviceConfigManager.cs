using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Managers
{
    using Kreyos.Classes.Components;

    class DeviceConfigManager
    {
        /****************************************************************
        * Singleton
        **/
        private static DeviceConfigManager m_instance = null;

        public static DeviceConfigManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new DeviceConfigManager();
                    m_instance.GetDeviceConfig();
                }

                return m_instance;
            }
        }


        private DeviceConfiguration m_deviceConfig = null;

        public DeviceConfiguration GetDeviceConfig()
        {
            // Load device if not initialized
            if (m_deviceConfig == null) 
            {
                m_deviceConfig = new DeviceConfiguration();
                m_deviceConfig.Initialized();
            }

            return m_deviceConfig;
        }

        public void SetNewGoal(int p_goal) 
        {
            m_deviceConfig.Goals = new int[] 
            {
                p_goal,
                1000,
                1000
            };

            m_deviceConfig.Save();
            BluetoothManager.Instance.SyncWatch();
        }

        public void SetHeight(int p_height)
        {
            m_deviceConfig.Height = p_height;
        }

        public void SetWeight(int p_weight)
        {
            m_deviceConfig.Weight = p_weight;
        }
    }

}
