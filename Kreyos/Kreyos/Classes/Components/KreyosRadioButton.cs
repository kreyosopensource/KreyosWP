using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Kreyos.Classes.Components
{
    public class KreyosRadioButton : RadioButton
    {
        public static readonly SolidColorBrush BLACK = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 255)); 

        public void ChangeColor ( SolidColorBrush p_color )
        {
            this.Foreground = p_color;
            this.BorderBrush = p_color;
        }
    }
}
