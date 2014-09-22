using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.SDK.Bluetooth
{
    public class TodayActivity
    {
        public int steps;
        public double distance;
        public double calories;
        public int time;

        public TodayActivity()
        {
            steps = 0;
            distance = 0;
            calories = 0;
            time = 0;
        }
    }
}
