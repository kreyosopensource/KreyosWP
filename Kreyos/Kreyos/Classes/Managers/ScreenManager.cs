using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;  
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;

namespace Kreyos.Classes.Managers
{
    /****************************************************************
     * Constants
     **/
    public enum EScreens : int
    {
        ES_INVALID          = -1,
        ES_Login            = 0,

		ES_RegisterEmail,
        ES_RegisterProfile,

        // Main screens
        ES_HomeActivity,
        ES_ActivityStats,
        ES_SportsMode,
        ES_DailyTarget,
        ES_PersonalProfile,
        ES_Settings,

        // Sub screens ( Settings )
        ES_DateAndTime,
        ES_SilentArams,
        ES_FirmwareUpdate,
        ES_Bluetooth,

        ES_MAX,
        ES_Debug
    };

    /// <summary>
    /// Main Screen Pages
    /// </summary>
    public enum EPivotPage
    {
        TodaysActivity      = 0,
        OverallActivity,
        SportsMode,
        DailyTarget,
        PersonalProfile,
        Settings
    };
	
    public class ScreenManager
    {
        /****************************************************************
         * Singleton
         **/
        private static ScreenManager m_instance = null;

        public static ScreenManager Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new ScreenManager();
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Properties
         **/
        public static readonly string TODAYS_ACTIVITY_KEY               = "your_progress";
        public static readonly string OVERALL_ACTIVITY_KEY              = "activity_stats";
        public static readonly string SPORTS_MODE_KEY                   = "sports_mode";
        public static readonly string DAILY_TARGET_KEY                  = "daily_target";
        public static readonly string PROFILE_KEY                       = "profile";
        public static readonly string SETTINGS_KEY                      = "settings";
        public static readonly Dictionary<string, EPivotPage> PageMap   = new Dictionary<string, EPivotPage>()
        {
            { TODAYS_ACTIVITY_KEY,      EPivotPage.TodaysActivity     },
            { OVERALL_ACTIVITY_KEY,     EPivotPage.OverallActivity    },
            { SPORTS_MODE_KEY,          EPivotPage.SportsMode         },
            { DAILY_TARGET_KEY,         EPivotPage.DailyTarget        },
            { PROFILE_KEY,              EPivotPage.PersonalProfile    },
            { SETTINGS_KEY,             EPivotPage.Settings           }
        };

        private List<String> m_screens = null;

        /****************************************************************
         * Constructor
         **/
        private ScreenManager ()
        {
            m_screens = new List<String>();

            //~~~Main Screen
            m_screens.Insert((int)EScreens.ES_Login,            "/Classes/Screens/LoginScreen.xaml");
            m_screens.Insert((int)EScreens.ES_RegisterEmail,    "/Classes/Screens/CreateAccountEmail.xaml");
            m_screens.Insert((int)EScreens.ES_RegisterProfile,  "/Classes/Screens/CreateAccountProfile.xaml");

            //~~~Home Screen
            m_screens.Insert((int)EScreens.ES_HomeActivity,     "/Classes/Screens/MainScreen.xaml");
            m_screens.Insert((int)EScreens.ES_ActivityStats,    "/Classes/Screens/MainScreen.xaml?goto=" + (int)EPivotPage.OverallActivity);
            m_screens.Insert((int)EScreens.ES_SportsMode,       "/Classes/Screens/MainScreen.xaml?goto=" + (int)EPivotPage.SportsMode);
            m_screens.Insert((int)EScreens.ES_DailyTarget,      "/Classes/Screens/MainScreen.xaml?goto=" + (int)EPivotPage.DailyTarget);
            m_screens.Insert((int)EScreens.ES_PersonalProfile,  "/Classes/Screens/MainScreen.xaml?goto=" + (int)EPivotPage.PersonalProfile);
            m_screens.Insert((int)EScreens.ES_Settings,         "/Classes/Screens/MainScreen.xaml?goto=" + (int)EPivotPage.Settings);

            //~~~Sub screens
            m_screens.Insert((int)EScreens.ES_DateAndTime,      "/Classes/Screens/DateAndTimeScreen.xaml");
            m_screens.Insert((int)EScreens.ES_SilentArams,      "/Classes/Screens/SilentAlarmScreen.xaml");
            m_screens.Insert((int)EScreens.ES_FirmwareUpdate,   "/Classes/Screens/UpdateFirmwareScreen.xaml");
            m_screens.Insert((int)EScreens.ES_Bluetooth,        "/Classes/Screens/BluetoothScreen.xaml");
        }

        /****************************************************************
         * Helper Methods
         **/
        public void Switch (EScreens p_screen)
        {
            PhoneApplicationFrame frame;

            // debug
            if (p_screen == EScreens.ES_Debug)
            {
                frame = Application.Current.RootVisual as PhoneApplicationFrame;
                frame.Navigate(new Uri("/Classes/Screens/TestView.xaml", UriKind.Relative));
                return;
            }

            String screen = m_screens[(int)p_screen];
            frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri(screen, UriKind.Relative));

            // Do some stuff here
        }

        public void DebugSwitch (string p_string)
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri(p_string, UriKind.Relative));
        }
    }
}
