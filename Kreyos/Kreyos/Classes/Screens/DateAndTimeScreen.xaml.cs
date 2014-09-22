using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;


using Kreyos.Classes.Utils;
using Kreyos.Classes.Managers;

namespace Kreyos.Classes.Screens
{
    public partial class DateAndTimeScreen : PhoneApplicationPage
    {
        /****************************************************************
         * Constants
         **/
        public static readonly string BUTTON_ON     = "On";
        public static readonly string BUTTON_OFF    = "Off";

        /****************************************************************
         * Properties
         **/
        private TextBlock m_txtSwitch;
        private ToggleSwitch m_toggleButton;
        private Grid m_dateAndTime;
        private DatePicker m_datePicker;
        private TimePicker m_timePicker;
        private bool m_bIsOn;

        /****************************************************************
         * Constructors
         **/
        public DateAndTimeScreen()
        {
            InitializeComponent();

            // Temp Value
            m_bIsOn = true;

            m_dateAndTime = this.grou_date_and_time;
            m_txtSwitch = this.txt_switch;
            m_toggleButton = this.ts_toggle;
            m_datePicker = this.dp_picker_date;
            m_datePicker.DataContext = this;
            m_timePicker = this.tp_picler_time;
            m_timePicker.DataContext = this;

            // set date and time to on
            this.UpdateSwitch();
        }


        /****************************************************************
         * Main Functionalities
         **/
        private void UpdateSwitch ()
        {
            switch (m_bIsOn)
            {
                case true:
                {
                    m_txtSwitch.Text = BUTTON_ON;
                    m_dateAndTime.Visibility = Visibility.Collapsed;
                    m_toggleButton.IsChecked = m_bIsOn;
                }
                break;

                case false:
                {
                    m_txtSwitch.Text = BUTTON_OFF;
                    m_dateAndTime.Visibility = Visibility.Visible;
                    m_toggleButton.IsChecked = m_bIsOn;
                }
                break;
            }

            m_datePicker.UpdateLayout();
            m_timePicker.UpdateLayout();
        }

        /****************************************************************
         * UI Getters. For Data Binding
         **/
        public string CurrentDate
        {
            get
            {
                DateTime now = DateTime.Now;
                return "" + now.Month + "/" + now.Day + "/" + now.Year + "";
            }
        }

        public string CurrentTime
        {
            get
            {
                DateTime now = DateTime.Now;
                return "" + now.Hour + ":" + now.Minute + "" + now.ToString("tt").ToUpper();
            }
        }

        /****************************************************************
         * UI Events
         **/
        private void OnToggle(object sender, System.Windows.Input.GestureEventArgs e)
        {
            m_bIsOn = !m_bIsOn;
            this.UpdateSwitch();
        }

        private void OnDateChanged(object sender, Microsoft.Phone.Controls.DateTimeValueChangedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            if (e.NewDateTime == null || e.OldDateTime == null)
            {
                return;
            }
           
            DateTime dateNow = DateTime.Now;
            DateTime dateTime = (DateTime)e.NewDateTime;
            dateTime = dateTime.AddHours(dateNow.Hour);
            dateTime = dateTime.AddMinutes(dateNow.Minute);
            BluetoothManager.Instance.SetDateTime(dateTime);
            // KreyosUtils.Log("DateTime", "Set Time");
        }

        private void OnTimeChanged(object sender, Microsoft.Phone.Controls.DateTimeValueChangedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            if (e.NewDateTime == null || e.OldDateTime == null)
            {
                return;
            }

            DateTime dateTime = (DateTime)e.NewDateTime;
            BluetoothManager.Instance.SetDateTime(dateTime);
            // KreyosUtils.Log("DateTime", "Set Time");
        }
    }
}