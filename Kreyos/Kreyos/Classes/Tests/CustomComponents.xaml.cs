using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Kreyos.Classes.Tests
{
    using Kreyos.Classes.Components;

    public partial class CustomComponents : PhoneApplicationPage
    {
        public CustomComponents()
        {
            InitializeComponent();
            //this.kts_switch.Style = (Style)this.Resources["KreyosToggleSwitchStyle"];
            this.kts_switch.UpdateStyle(this);
        }
    }
}