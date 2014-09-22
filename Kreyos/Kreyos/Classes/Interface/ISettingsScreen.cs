using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Interface
{
    interface ISettingsScreen
    {
        void OnDateAndTime(object sender, System.Windows.Input.GestureEventArgs e);
        void OnSilentAlarms(object sender, System.Windows.Input.GestureEventArgs e);
        void OnUpdateFirmware(object sender, System.Windows.Input.GestureEventArgs e);
        void OnBluetooth(object sender, System.Windows.Input.GestureEventArgs e);
    }
}
