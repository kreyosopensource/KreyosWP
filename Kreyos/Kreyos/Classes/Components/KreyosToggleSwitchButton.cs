using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls.Primitives;

namespace Kreyos.Classes.Components
{
    public class KreyosToggleSwitchButton : ToggleSwitchButton
    {
        public static readonly string KREYOS_TOGGLE_SWITCH_BUTTON_STYLE = "KreyosToggleSwitchButtonStyle";

        public void UpdateStyle(PhoneApplicationPage p_mainScreen)
        {
            this.Style = (Style)p_mainScreen.Resources[KREYOS_TOGGLE_SWITCH_BUTTON_STYLE];
            this.UpdateLayout();
        }
    }
}
