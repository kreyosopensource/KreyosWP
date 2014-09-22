using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.SDK.Bluetooth
{
    public class ActivityDataDoc
    {
        public int version { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }

        public IList<ActivityDataRow> data { get; set; }

        public ActivityDataDoc()
        {
            this.data = new List<ActivityDataRow>();
        }
    }
}
