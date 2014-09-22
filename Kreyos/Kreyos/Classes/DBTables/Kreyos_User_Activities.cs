using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.DBTables
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Utils;
    using Kreyos.SDK.Bluetooth;

    public class Kreyos_User_Activities
    {
        //~~~epoch
        public uint CreatedTime { get; set; }
        public uint ActivityBestLap { get; set; }
        public uint ActivityAvgLap { get; set; }
        public uint ActivityCurrentLap { get; set; }
        public uint ActivityAvgPace { get; set; }
        public uint ActivityPace { get; set; }
        public uint ActivityTopSpeed { get; set; }
        public uint ActivityAvgSpeed { get; set; }
        public uint ActivitySpeed { get; set; }
        public uint ActivityElevation { get; set; }
        public uint ActivityAltitude { get; set; }
        public uint ActivityMaxHeart { get; set; }
        public uint AvgActivityHeart { get; set; }
        public uint ActivityHeart { get; set; }
        public uint Activity_ID { get; set; }
        public uint Sport_ID { get; set; }
        public uint KreyosUserID { get; set; }
        public uint ActivityCalories { get; set; }
        public uint ActivitySteps { get; set; }
        public uint Coordinates { get; set; }
        public uint ActivityDistance { get; set; }

        //~~~helper query
        //      e.g. Jan, 1 2014
        public string CreatedDate { get; set; }

        public Kreyos_User_Activities ()
        {
            this.CreatedTime = 0;
            this.ActivityBestLap = 0;
            this.ActivityAvgLap = 0;
            this.ActivityCurrentLap = 0;
            this.ActivityAvgPace = 0;
            this.ActivityPace = 0;
            this.ActivityTopSpeed = 0;
            this.ActivityAvgSpeed = 0;
            this.ActivitySpeed = 0;
            this.ActivityElevation = 0;
            this.ActivityAltitude = 0;
            this.ActivityMaxHeart = 0;
            this.AvgActivityHeart = 0;
            this.ActivityHeart = 0;
            this.Activity_ID = 0;
            this.Sport_ID = 0;
            this.KreyosUserID = 0;
            this.ActivityCalories = 0;
            this.ActivitySteps = 0;
            this.Coordinates = 0;
            this.ActivityDistance = 0;
            this.CreatedDate = string.Empty;
        }

        public void Update (Kreyos_User_Activities p_activity)
        {
            this.ActivityBestLap = p_activity.ActivityBestLap;
            this.ActivityAvgLap = p_activity.ActivityAvgLap;
            this.ActivityCurrentLap = p_activity.ActivityCurrentLap;
            this.ActivityAvgPace = p_activity.ActivityAvgPace;
            this.ActivityPace = p_activity.ActivityPace;
            this.ActivityTopSpeed = p_activity.ActivityTopSpeed;
            this.ActivityAvgSpeed = p_activity.ActivityAvgSpeed;
            this.ActivitySpeed = p_activity.ActivitySpeed;
            this.ActivityElevation = p_activity.ActivityElevation;
            this.ActivityAltitude = p_activity.ActivityAltitude;
            this.ActivityMaxHeart = p_activity.ActivityMaxHeart;
            this.AvgActivityHeart = p_activity.AvgActivityHeart;
            this.ActivityHeart = p_activity.ActivityHeart;
            this.Activity_ID = p_activity.Activity_ID;
            this.Sport_ID = p_activity.Sport_ID;
            this.KreyosUserID = p_activity.KreyosUserID;
            this.ActivityCalories = p_activity.ActivityCalories;
            this.ActivitySteps = p_activity.ActivitySteps;
            this.Coordinates = p_activity.Coordinates;
            this.ActivityDistance = p_activity.ActivityDistance;   
        }

        public void UpdateFromRow (ActivityDataRow p_row)
        {
            Dictionary<ActivityDataRow.DataType, double> data = (Dictionary<ActivityDataRow.DataType, double>)p_row.data;
            this.Sport_ID           = (uint)p_row.mode;
            this.ActivitySteps      = (uint)data[ActivityDataRow.DataType.DATA_COL_STEP];
            this.ActivityDistance   = (uint)data[ActivityDataRow.DataType.DATA_COL_DIST];
            this.ActivityCalories   = (uint)data[ActivityDataRow.DataType.DATA_COL_CALS];
            this.ActivityHeart      = (uint)data[ActivityDataRow.DataType.DATA_COL_HR];
            
            //~~~update time
            DateTime today          = KreyosUtils.NowWith(p_row.hour, p_row.minute);
            this.CreatedTime        = KreyosUtils.EpochFrom(today);
            this.CreatedDate        = KreyosUtils.DateStringWithYear(today);
        }

        public ActivityStatsData ToActivityView()
        {
            ActivityStatsData viewData = new ActivityStatsData(
                (EAct_Types)this.Sport_ID,
                (ulong)this.CreatedTime,
                (uint)this.ActivitySteps,
                (uint)this.ActivityDistance,
                (uint)this.ActivityCalories,
                (uint)this.ActivityHeart);

            return viewData;
        }
    }
}
