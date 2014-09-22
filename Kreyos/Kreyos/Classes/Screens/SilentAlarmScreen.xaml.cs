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

namespace Kreyos.Classes.Screens
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Utils;
    using Kreyos.Classes.Managers;

    public partial class SilentAlarmScreen : PhoneApplicationPage
    {
        /****************************************************************
         * Properties
         **/
        private AlarmView[] m_alarmViews;
        private int m_selectedIndex;
        private KreyosTimePicker m_ctpContainer;

        /****************************************************************
         * Constructor
         **/
        public SilentAlarmScreen()
        {
            InitializeComponent();

            m_alarmViews = new AlarmView[3];
            m_alarmViews[0] = new AlarmView(this, 0);
            m_alarmViews[1] = new AlarmView(this, 1);
            m_alarmViews[2] = new AlarmView(this, 2);

            // this must always be invisible and disabled
            m_ctpContainer = this.ctp_time_picker;
        }

        /****************************************************************
         * UI Callbacks
         **/
        /// <summary>
        /// Toggle Switch Button Callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnEnabledAlarm(object sender, System.Windows.Input.GestureEventArgs e)
        {
            KreyosToggleSwitchButton button = sender as KreyosToggleSwitchButton;
            KreyosUtils.Log("SimpleAlarmScreen::OnEnabledAlarm", "Tap SelectedItem:" + e + " IsChecked:" + button.IsChecked);

            int index = AlarmView.GetIndex(button.Name);

            if (index >= 0 && index <= m_alarmViews.Length-1)
            {
                AlarmView view = m_alarmViews[index];
                view.UpdateView((bool)button.IsChecked);
                m_selectedIndex = index;

                // show picker
                if ((bool)button.IsChecked)
                {
                    m_ctpContainer.Value = (DateTime?)view.AlarmData;
                    m_ctpContainer.ClickTemplateButton();
                }
            }
        }

        private void OnUpdateWatch(object sender, System.Windows.Input.GestureEventArgs e)
        {
            KreyosUtils.Log("SimpleAlarmScreen::OnUpdateWatch", "Update..");

            for (int i = 0; i < m_alarmViews.Length; i++)
            {
                /*
                AlarmView view = m_alarmViews[0];
                KreyosUtils.Log("Alarm Status", "" + view.ToggleSwitch.IsChecked);
                KreyosUtils.Log("Alarm Time", "" + view.AlarmData.Hour);
                */

                AlarmView alarm = m_alarmViews[i];
                if (!(bool)alarm.ToggleSwitch.IsChecked) 
                {
                    continue;
                }

                BluetoothManager.Instance.SetAlarm(i, true, alarm.AlarmData.Hour, alarm.AlarmData.Minute);
            }
            
           
           
        }

        /// <summary>
        /// On Picker Date Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, Microsoft.Phone.Controls.DateTimeValueChangedEventArgs e)
        {
            KreyosUtils.Log("SimpleAlarmScreen::OnValueChanged", "Value Changed..");
            KreyosTimePicker timePicker = sender as KreyosTimePicker;
            DateTime? timeSet = timePicker.Value;
            AlarmView view = m_alarmViews[m_selectedIndex];
            view.SetAlarm((DateTime)timeSet);
            view.UpdateView((bool)view.ToggleSwitch.IsChecked);
        }
    }
}