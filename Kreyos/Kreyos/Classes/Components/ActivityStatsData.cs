using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kreyos.Classes.Utils;

namespace Kreyos.Classes.Components
{
    public class ActivityStatsData
    {
        /****************************************************************
         * Constants
         **/
        public static readonly string[] ACTIVITYIES = { "walking", "running", "cycling" };
        public static readonly string IMG_WALKING = "/Assets/Icons/activity-stats-walking.png";
        public static readonly string IMG_RUNNING = "/Assets/Icons/activity-stats-running.png";
        public static readonly string IMG_CYCLING = "/Assets/Icons/activity-stats-cycling.png";
        public static readonly string[] IMG_ACTIVITIES = { IMG_WALKING, IMG_RUNNING, IMG_CYCLING };

        /****************************************************************
         * Getters | Setters
         **/
        /// <summary>
        /// Activity Type: Waling:0 Running:1 Cycling:2
        /// </summary>
        public EAct_Types ActType { get; private set; }
        [System.ComponentModel.DefaultValue((long)(0))]
        public ulong EpochTime { get; private set; }
        [System.ComponentModel.DefaultValue((int)(0))]
        public uint Steps { get; set; }
        [System.ComponentModel.DefaultValue((int)(0))]
        public uint Distance { get; set; }
        [System.ComponentModel.DefaultValue((int)(0))]
        public uint Calories { get; set; }
        [System.ComponentModel.DefaultValue((int)(0))]
        public uint Altitude { get; set; }
        //~~~Extra getters
        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }

        // Text Display Getters
        public string TxtTime { get; private set; }
        public string TxtActImage { get; private set; }
        public string TxtActTitle { get; private set; }
        public string TxtItemTitle { get; private set; } // 3rd column title *altitude or *calories
        public string TxtItemValue { get; private set; } // 3rd column value.
        public List<StatsData> Stats { get; private set; } // Sub scroller data

        /****************************************************************
         * Constructors
         **/
        public ActivityStatsData(
            EAct_Types p_type,
            ulong p_epoch = 0, // seconds passed since 1/1/1970
            uint p_steps = 0,
            uint p_distance = 0,
            uint p_calories = 0,
            uint p_altitude = 0
        )
        {
            this.EpochTime = p_epoch;
            this.Steps = p_steps;
            this.Distance = p_distance;
            this.Calories = p_calories;
            this.Altitude = p_altitude;
            this.ActType = p_type;

            // Test print
            DateTime date = KreyosUtils.ToDateTime((long)p_epoch);
            KreyosUtils.Log("ActivityStatsData::Constructor", "epoch:" + p_epoch + " date:" + date + " epochDate:" + date.ToShortDateString());// date.ToShortDateString()

            // Add display values
            this.TxtTime = date.Hour + ":" + date.Minute + date.ToString("tt").ToLower().Substring(0,1); // Add am or pm after the minute value
            this.TxtActImage = ActivityStatsData.ActImage(p_type);
            this.TxtActTitle = ActivityStatsData.ActToString(p_type);

            this.Hour = date.Hour;
            this.Minute = date.Minute;
            this.Second = date.Second;

            // display altitude values for cycling.
            // Hard coded D:
            if (this.ActType == EAct_Types.ECycling)
            {
                this.TxtItemTitle = "altitude";
                this.TxtItemValue = p_altitude.ToString();
            }
            else
            {
                this.TxtItemTitle = "calories";
                this.TxtItemValue = p_calories.ToString();
            }

            // Create sub scroller data
            this.Stats = new List<StatsData>();
            this.UpdateData();
        }

        /****************************************************************
         * Helpers
         **/
        public static string ActToString (EAct_Types p_act)
        {
            return ACTIVITYIES[(int)p_act];
        }

        public static string ActImage (EAct_Types p_act)
        {
            return IMG_ACTIVITIES[(int)p_act];
        }

        public static StatsData CreateStat (string p_title, string p_value)
        {
            StatsData stats = new StatsData();
            stats.TxtTitle = p_title;
            stats.TxtValue = p_value;
            return stats;
        }

        /// <summary>
        /// Clears and Readd stat values for Sub Slider.
        /// </summary>
        public void UpdateData () 
        {
            this.Stats.Clear();
            this.Stats.Add(ActivityStatsData.CreateStat("steps", this.Steps.ToString()));
            this.Stats.Add(ActivityStatsData.CreateStat("distance", this.Distance.ToString()));
            this.Stats.Add(ActivityStatsData.CreateStat("calories", this.Calories.ToString()));
            this.Stats.Add(ActivityStatsData.CreateStat("altitude", this.Altitude.ToString()));
        }
    }

    /****************************************************************
     * Sub scroller data
     **/
    public class StatsData
    {
        public string TxtTitle { get; set; }
        public string TxtValue { get; set; }
    }
}
