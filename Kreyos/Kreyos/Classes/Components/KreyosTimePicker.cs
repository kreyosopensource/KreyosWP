using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Kreyos.Classes.Components
{
    public class KreyosTimePicker : TimePicker
    {
        /****************************************************************
         * Public Methods
         **/
        /// <summary>
        /// Trigger Show Picker
        /// </summary>
        public void ClickTemplateButton()
        {
            Button button = (GetTemplateChild("DateTimeButton") as Button);
            ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
            IInvokeProvider provider = (peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider);
            provider.Invoke();
        }
    }
}
