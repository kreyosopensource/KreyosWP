using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;

namespace Kreyos.Classes.Components
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Screens;
    
    public class AlarmView
    {
        /****************************************************************
         * Properties
         **/
        private DateTime m_alarmData = DateTime.Now;
        private int m_index; // It's value will be either 0-1-2


        /****************************************************************
         * Constructors
         **/
        public AlarmView (SilentAlarmScreen p_context, int p_index)
            : this(p_context, p_index, false)
        {
        }

        public AlarmView (SilentAlarmScreen p_context, int p_index, bool p_bIsOn)
        {
            switch (p_index)
            {
                case 0:
                {
                    this.AlarmTime = p_context.txt_alarm_time0;
                    this.AlarmStatus = p_context.txt_alarm_status0;
                    this.ToggleSwitch = p_context.tsb_toggle0;
                }
                break;

                case 1:
                {
                    this.AlarmTime = p_context.txt_alarm_time1;
                    this.AlarmStatus = p_context.txt_alarm_status1;
                    this.ToggleSwitch = p_context.tsb_toggle1;
                }
                break;

                case 2:
                {
                    this.AlarmTime = p_context.txt_alarm_time2;
                    this.AlarmStatus = p_context.txt_alarm_status2;
                    this.ToggleSwitch = p_context.tsb_toggle2;
                }
                break;
            }
            
            m_index = p_index;
            this.UpdateView(p_bIsOn);
        }

        /****************************************************************
         * Public function. ( Main Functionalities )
         **/
        public void UpdateView (bool p_bIsOn)
        {
            this.IsOn = p_bIsOn;

            if (this.IsOn)
            {
                this.AlarmTime.Text = "" + m_alarmData.Hour + ":" + m_alarmData.Minute + "" + m_alarmData.ToString("tt").ToUpper();
                this.AlarmStatus.Text = "On";
            }
            else
            {
                this.AlarmTime.Text = "ALARM " + (m_index + 1);
                this.AlarmStatus.Text = "Off";
            }
        }

        /****************************************************************
         * Getter | Setter
         **/
        public void SetAlarm (DateTime p_alarm)
        {
            m_alarmData = p_alarm;
        }

        public bool IsOn { get; private set; }
        public TextBlock AlarmTime { get; private set; }
        public TextBlock AlarmStatus { get; private set; }
        public KreyosToggleSwitchButton ToggleSwitch { get; private set; }
        public DateTime AlarmData { get { return m_alarmData; } }

        /****************************************************************
         * Helpers
         **/
        /// <summary>
        /// Returns the Index by trimmings
        /// </summary>
        /// <param name="p_name">
        /// The p_name is the name of the ToggleSwitchButtons which follows this pattern.
        /// <buttonName>0
        /// <buttonName>1
        /// <buttonName>2
        /// </param>
        /// <returns></returns>
        public static int GetIndex(string p_name)
        {
            string strIndex = p_name.Substring(p_name.Length - 1, 1);
            int index;
            bool bIsNumber = int.TryParse(strIndex, out index);
            if (bIsNumber) { return index; }
            // Invalid
            return -1;
        }
    }
}
