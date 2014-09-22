using System;
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
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;


    public partial class CreateAccountEmail : PhoneApplicationPage
    {
        /****************************************************************
         * Properties
         **/
		private TextBox m_txtEmail;
        private PasswordBox m_txtPassword;
        private PasswordBox m_txtConfirmPassword;

        /****************************************************************
         * Constructors
         **/
        public CreateAccountEmail()
        {
            Kreyos_User_Profile.Instance.Clear();

            InitializeComponent();
            m_txtEmail = this.txt_email;
            m_txtPassword = this.txt_password;
            m_txtConfirmPassword = this.txt_password_confirm;
        }

        /****************************************************************
         * Private Functions
         **/
        private bool IsDataValid ()
        {
            // check similar password
            if( m_txtPassword.Password == null
            ||  m_txtEmail.Text == null
            ||  m_txtPassword.Password.Length < KreyosUtils.MIN_CHARS
            ||  m_txtEmail.Text.Length < KreyosUtils.MIN_CHARS
            ||  m_txtPassword.Password.Equals(KreyosUtils.CONFIRM_PASSWORD_DEFAULT)
            ||  m_txtEmail.Text.Equals(KreyosUtils.EMAIL_DEFAULT)
            ||  !m_txtPassword.Password.Equals(m_txtConfirmPassword.Password)
            ) {
                // missed match password
                KreyosUtils.ClearField(this.txt_password_dummy, m_txtPassword.Password.Length, KreyosUtils.PASSWORD_DEFAULT);
                KreyosUtils.ClearField(this.txt_password_confirm_dummy, m_txtConfirmPassword.Password.Length, KreyosUtils.CONFIRM_PASSWORD_DEFAULT);
                return false;
            }

            return true;
        }

        /****************************************************************
         * Callback Functions
         **/
        private void EmailOnLoadFocus (object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            KreyosUtils.ClearField(this.txt_email_dummy, m_txtEmail.Text.Length, KreyosUtils.EMAIL_DEFAULT);
        }

        private void PasswordOnLoadFocus (object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            KreyosUtils.ClearField(this.txt_password_dummy, m_txtPassword.Password.Length, KreyosUtils.PASSWORD_DEFAULT);
        }

        private void ConfirmPasswordOnLostFocus (object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            KreyosUtils.ClearField(this.txt_password_confirm_dummy, m_txtConfirmPassword.Password.Length, KreyosUtils.CONFIRM_PASSWORD_DEFAULT);
        }

        private void OnBack (object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_Login);
        }

        private void OnTapNext (object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            //~~~sanity check
            //      Comment for debuggin
            //if (!this.IsDataValid())
            //{
            //    return;
            //}

            Kreyos_User_Profile.Instance.ID         = m_txtEmail.Text;
            Kreyos_User_Profile.Instance.Email      = m_txtEmail.Text;
            Kreyos_User_Profile.Instance.Password   = m_txtPassword.Password;

            //~~~print
            Kreyos_User_Profile.Instance.Print();

            //~~~move to next scene
			ScreenManager.Instance.Switch(EScreens.ES_RegisterProfile);
        }
    }
}