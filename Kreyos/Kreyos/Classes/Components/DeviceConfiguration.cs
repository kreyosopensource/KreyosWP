using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace Kreyos.Classes.Components
{
    class DeviceConfiguration
    {

        private IsolatedStorageSettings m_userStorage = IsolatedStorageSettings.ApplicationSettings;

        private string m_KeyGoals = "goals";
        private string m_KeyHeight = "height";
        private string m_KeyWeight = "weight";

        public string[] WorldClockTable { set; get; }
        public int[] WorldClockOffset { set; get; }
        public bool IsDigitalClock { set; get; }
        public int AnalogClock { set; get; }
        public int DigitalClock { set; get; }
        public int SportsGridType { set; get; }
        public int[] SportsGridValues { set; get; }
        public int[] Goals { set; get; }
        public int Height { set; get; }
        public int Weight { set; get; }
        public bool IsEnableGesture { set; get; }
        public bool IsLeftHandGesture { set; get; }
        public int[] Actionstable { set; get; }
        public bool IsUkUnit { set; get; }

        public void Initialized()
        {
            // Initialized Variables
            WorldClockTable = new string[] 
            {
                "TZone-1",
                "TZone-2",
                "TZone-3",
                "TZone-4",
                "TZone-5",
                "TZone-6"
            };

            WorldClockOffset = new int[]
            {
                0,
                1, 
                2,
                3,
                4,
                5
            };

            IsDigitalClock  = true;
            AnalogClock     = 1;
            DigitalClock    = 2;
            SportsGridType  = 0;
 
            SportsGridValues = new int[]
            {
                0,
                1,
                2,
                3,
                4,
                5
            };
            
            Goals = new int[] 
            {
                5000,
                5000,
                5000,
            };

            Height = 67;
            Weight = 110;

            IsEnableGesture     = true;
            IsLeftHandGesture   = true;

            Actionstable = new int[] 
            {
                0,
                1,
                2,
                3
            };

            IsUkUnit = true;

            if (!IsAlreadyHaveOldProfile())
            {
                CreateDefaultProfile();
                return;
            }

            Load();
        }

        private bool IsAlreadyHaveOldProfile()
        {
            if (!m_userStorage.Contains(m_KeyGoals)
            || !m_userStorage.Contains(m_KeyHeight)
            || !m_userStorage.Contains(m_KeyWeight))
            {
                return false;
            }
            return true;
        }

        private void CreateDefaultProfile()
        {
            m_userStorage.Add(m_KeyGoals, Goals);
            m_userStorage.Add(m_KeyHeight, Height);
            m_userStorage.Add(m_KeyWeight, Weight);
            m_userStorage.Save();
        }

        private void Load()
        {
            Goals     = (int[])m_userStorage[m_KeyGoals];
            Height    = (int)m_userStorage[m_KeyHeight];
            Weight    = (int)m_userStorage[m_KeyWeight];
        }

        public void Save()
        { 
            m_userStorage[m_KeyGoals]   = Goals;
            m_userStorage[m_KeyHeight]  = Height;
            m_userStorage[m_KeyWeight]  = Weight;
        }
    }
}
