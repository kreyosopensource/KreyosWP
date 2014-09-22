using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Kreyos.SDK.Bluetooth
{
    public class SportsDataRow
    {
        public enum DataType
        {
            DATA_WORKOUT	    = 0,
		    DATA_SPEED 	        = 1,
		    DATA_HEARTRATE      = 2,
		    DATA_CALS		    = 3,
		    DATA_DISTANCE	    = 4,
		    DATA_SPEED_AVG      = 5,
		    DATA_ALTITUTE	    = 6,
		    DATA_TIME		    = 7,
		    DATA_SPEED_TOP      = 8,
		    DATA_CADENCE        = 9,
		    DATA_PACE           = 10,

		    DATA_HEARTRATE_AVG    = 11,
		    DATA_HEARTRATE_TOTAL  = 12,
		    DATA_ELEVATION_GAIN   = 13,
		    DATA_CURRENT_LAP      = 14,
		    DATA_BEST_LAP         = 15,
		    DATA_FLOORS           = 16,
		    DATA_STEPS            = 17,
		    DATA_PACE_AVG         = 18,
		    DATA_LAP_AVG          = 19,
		
		    SPORTS_MODE_NORMAL  = 0x00,
		    SPORTS_MODE_RUNNING = 0x01,
		    SPORTS_MODE_BIKING  = 0x02,
		    SPORTS_MODE_WALK    = 0x03,
		    SPORTS_MODE_PAUSING = 0x10
        }

        public int sports_mode;
        public int seconds_elapse;
        public Dictionary<DataType, double> data;
        // public List<double> data;


        public static SportsDataRow loadFromBuffer(byte[] buf)
        {
            SportsDataRow row = new SportsDataRow();

            row.data = new Dictionary<DataType, double>();

            int cursor = 0;
            int grid_num = ((int)buf[cursor]) & 0x000000ff; cursor++;

            // + ET 041514 : Commented because it limit the showing of values
            //if (grid_num > 5)
            //return null;

            int data_start_offset = cursor + grid_num;
            for (int i = 0; i < grid_num - 1; ++i)
            {
                //get the value
                int key = ((int)buf[cursor]) & 0x000000ff;
                int intvalue = Protocol.bytesToInt(buf, data_start_offset + i * 4);
                cursor++;

                switch (key)
                {
                    case (int)DataType.DATA_WORKOUT:
                        row.seconds_elapse = intvalue;
                        break;

                    case (int)DataType.DATA_SPEED:
                    case (int)DataType.DATA_SPEED_AVG:
                    case (int)DataType.DATA_SPEED_TOP:
                        double speedValue = (double)intvalue * 36 / 1000;
                        // row.data.Insert(key, Math.Round(speedValue * 100.0) / 100.0);
                        row.data.Add((DataType)key, Math.Round(speedValue * 100.0) / 100.0);
                        break;
                    
                    case (int)DataType.DATA_DISTANCE:
                        // row.data.Insert(key, (double)(intvalue) / 10);
                         row.data.Add((DataType)key, (double)(intvalue)/ 10);
                        break;
                    
                    default:
                        // row.data.Insert(key, (double)(intvalue));
                        row.data.Add((DataType)key, (double)(intvalue));
                        break;
                }
            }

            return row;
        }


    }
}
