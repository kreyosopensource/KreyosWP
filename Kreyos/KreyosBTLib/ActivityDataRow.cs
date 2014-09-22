using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.SDK.Bluetooth
{
    public class ActivityDataRow
    {
        public enum DataType
        {
            DATA_COL_INVALID = 0,
            DATA_COL_STEP = 1,
            DATA_COL_CADN,
            DATA_COL_HR,
            DATA_COL_DIST,
            DATA_COL_CALS,
        }

        public int hour { get; set; }
        public int minute { get; set; }
        public int mode { get; set; }

        public IDictionary<DataType, double> data { get; set; }

        public ActivityDataRow ()
        {
            this.data = new Dictionary<DataType, double>();
        }
    }
}
