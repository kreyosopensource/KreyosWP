using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Kreyos.Classes.Managers;
using System.Diagnostics;
using Kreyos.Classes.Utils;

namespace Kreyos
{
    public partial class LoginScreen : PhoneApplicationPage
    {
        /****************************************************************
         * Properties
         **/
        private TextBox m_txtUsername;
        private PasswordBox m_txtPassword;
        private Button m_buttonLogin;

        /****************************************************************
         * Constructors
         **/
        public LoginScreen ()
        {
            InitializeComponent();
            m_txtUsername = this.txt_username;
            m_txtPassword = this.txt_password;
            m_buttonLogin = this.btn_login;
        }

        /****************************************************************
         * UI Callbacks
         **/
        private void OnLogin(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_HomeActivity);
            //ScreenManager.Instance.Switch(EScreens.ES_SportsMode);
            //ScreenManager.Instance.Switch(EScreens.ES_Bluetooth);
            //ScreenManager.Instance.Switch(EScreens.ES_Debug);
            //ScreenManager.Instance.DebugSwitch("/Classes/Screens/DateAndTimeScreen.xaml");
            //ScreenManager.Instance.DebugSwitch("/Classes/Screens/SilentAlarmScreen.xaml");
            //ScreenManager.Instance.DebugSwitch("/Classes/Tests/CustomComponents.xaml");
            //ScreenManager.Instance.DebugSwitch("/Classes/Tests/GridTest.xaml");
        }

        private void UsernameGotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.txt_username_dummy.Text = KreyosUtils.EMPTY_DEFAULT;
        }

        private void PasswordGotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.txt_password_dummy.Text = KreyosUtils.EMPTY_DEFAULT;
        }
		
        private void UsernameLostFocus ( object sender, System.Windows.RoutedEventArgs e )
        {
            KreyosUtils.ClearField(this.txt_username_dummy, m_txtUsername.Text.Length, KreyosUtils.USERNAME_DEFAULT);
        }

        private void PasswordLostFocus ( object sender, System.Windows.RoutedEventArgs e )
        {
            KreyosUtils.ClearField(this.txt_password_dummy, m_txtPassword.Password.Length, KreyosUtils.PASSWORD_DEFAULT);
        }
		
        private void OnCreateAccount (object sender, System.Windows.Input.GestureEventArgs e)
        {
            ScreenManager.Instance.Switch(EScreens.ES_RegisterEmail);
        }
    }
}