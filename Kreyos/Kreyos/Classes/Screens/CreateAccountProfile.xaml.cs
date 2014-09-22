using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;
using Microsoft.Phone.Shell;
using Kreyos.Classes.Managers;

namespace Kreyos.Classes.Screens
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Utils;

    public partial class CreateAccountProfile : PhoneApplicationPage
    {
        /****************************************************************
         * Constants
         **/
        private List<int> WEIGHT_VALUES = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        private List<int> HEIGHT_VALUES = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

        /****************************************************************
         * Properties
         **/
        private Button m_btnCreateAccount;
        private TextBox m_txtFirstName;
        private TextBox m_txtLastName;
        private TextBox m_txtBirthdate;
        private ListPicker m_lpWeight;
        private ListPicker m_lpHeight;
        private KreyosRadioButton m_krbMale;
        private KreyosRadioButton m_krbFemale;
        private Gender m_gender;

        //~~~Test
        private LoopingSelector m_lsTest;
        
        /****************************************************************
         * Constructors
         **/
        public CreateAccountProfile()
        {
            InitializeComponent();

            //~~~init properties
            m_btnCreateAccount  = this.btn_next;
            m_txtFirstName = this.txt_firstname;
            m_txtLastName = this.txt_lastname;
            m_txtBirthdate = this.txt_birthday;
            m_krbMale = this.krb_male;
            m_krbFemale = this.krb_female;
            //m_lpWeight = this.lp_weight;
            //m_lpHeight = this.lp_height;

            //~~~set defaults
            m_gender = Gender.Male;
            //m_lpWeight.ItemsSource = WEIGHT_VALUES;
            //m_lpHeight.ItemsSource = HEIGHT_VALUES;

            //~~~test looping selector
            //m_lsTest = this.ls_test;
            //m_lsTest.DataSource = new KreyosIntPicker()
            //{
            //    MinValue = 1,
            //    MaxValue = 10,
            //    SelectedItem = 1
            //};
        }

        /****************************************************************
         * Private Functions
         **/
        private bool IsFieldValid (TextBox p_txt)
        {
            // check similar password
            if (p_txt.Text == null || p_txt.Text.Length == 0) 
            {
                return false;
            }

            return true;
        }

        /****************************************************************
         * UI Callbacks
         **/
        private void OnBack(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_Login);
        }

        private void OnTapCreateAccount(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            //~~~sanity check fields
            if (!this.IsFieldValid(m_txtFirstName)
            ||  !this.IsFieldValid(m_txtLastName)
            )
            {
                //~~~please fill all the fields!
                return;
            }

            //~~~fill the data in Kreyos Utils
            Kreyos_User_Profile.Instance.Name = m_txtFirstName.Text;
            Kreyos_User_Profile.Instance.LastName = m_txtLastName.Text;
            //~~~Canadian format. dd/mm/yyyy
            Kreyos_User_Profile.Instance.Birthday = m_txtBirthdate.Text;
            Kreyos_User_Profile.Instance.Gender = (int)m_gender;
            //Kreyos_User_Profile.Instance.Weight = int.Parse(m_txtWeight.Text);
            //Kreyos_User_Profile.Instance.Height = int.Parse(m_txtHeight.Text);

            //~~~print data
            Kreyos_User_Profile.Instance.Print();
        }
    }
}