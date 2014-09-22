using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Components
{
    using Kreyos.Classes.Utils;

    public class ActivityStatsHeader<T> : List<T>
    {
        /****************************************************************
         * Properties
         **/
        /// <summary>
        /// The delegate that is used to get the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /****************************************************************
         * Constructors
         **/
        public ActivityStatsHeader(uint p_unix)
        {
            DateTime epoch = KreyosUtils.ToDateTime((long)p_unix);
            this.Day = epoch.DayOfWeek.ToString();
            this.Date = KreyosUtils.Month(epoch.Month) + " " + epoch.Day;
            this.UnixTime = p_unix;
        }

        /****************************************************************
         * Getter | Setters
         **/
        public uint UnixTime { get; set; }
        public uint TotalSteps { get; private set; }
        public uint TotalDistance { get; private set; }
        public uint TotalCalories { get; private set; }
        public uint TotalHours { get; private set; }
        public uint TotalMinues { get; private set; }
        public uint TotalSeconds { get; private set; }

        // Text Display Getters
        public string TxtDay
        {
            get { return this.Day.Substring(0, 3).ToUpper(); }
        }

        public string TxtSteps
        {
            get { return this.TotalSteps + "k"; }
        }

        public string TxtDistance
        {
            get { return this.TotalDistance.ToString(); }
        }

        public string TxtCalories
        {
            get { return this.TotalCalories + "k"; }
        }

        /// <summary>
        /// DAY OF WEEK ( MON - SUN )
        /// </summary>
        [System.ComponentModel.DefaultValue("SUN")]
        public string Day { get; private set; }

        /// <summary>
        /// String of: MONTH<space>DAY_OF_MONTH
        /// </summary>
        [System.ComponentModel.DefaultValue("Jan 1")]
        public string Date { get; private set; }

        /****************************************************************
         * Public Methods
         **/
        /// <summary>
        /// Updates total Values.
        /// Sum of all of it's unit values
        /// </summary>
        public void UpdateTotalValues()
        {
            uint totalSteps = 0;
            uint totalDist = 0;
            uint totalCal = 0;

            foreach (T cell in this)
            {
                ActivityStatsData c = (ActivityStatsData)Convert.ChangeType(cell, typeof(ActivityStatsData));
                totalSteps += c.Steps;
                totalDist += c.Distance;
                totalCal += c.Calories;
            }

            // TODO: Compute total Hrs, Min & Sec here

            TotalSteps = totalSteps;
            TotalDistance = totalDist;
            TotalCalories = totalCal;
        }

        /// <summary>
        /// Returns true if one of it's elements contains an action type 'p_type'
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public bool Contains (EAct_Types p_type)
        {
            if (this.Count <= 0) { return false; }

            foreach (T cell in this)
            {
                ActivityStatsData c = (ActivityStatsData)Convert.ChangeType(cell, typeof(ActivityStatsData));
                if (c.ActType == p_type) { return true; }
            }

            return false;
        }

        /// <summary>
        /// Returns any element at random index of the same 'p_type'.
        /// Note: Unused 'ST' template, it was added just to fix compiler errors.
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public ActivityStatsData GetAny (EAct_Types p_type)
        {
            if (this.Count <= 0) { return null; }

            foreach (T cell in this)
            {
                ActivityStatsData c = (ActivityStatsData)Convert.ChangeType(cell, typeof(ActivityStatsData));
                if (c.ActType == p_type) { return c; }
            }

            return null;
        }

        /****************************************************************
         * Helpers
         **/
        public static List<ActivityStatsHeader<ActivityStatsData>> ListFrom(List<ActivityStatsData> p_activities)
        {
            List<ActivityStatsHeader<ActivityStatsData>> list = new List<ActivityStatsHeader<ActivityStatsData>>();
            // string = (DAY OF WEEK)_(DATE) | ActivityStatsHeader::DateString
            Dictionary<string, ActivityStatsHeader<ActivityStatsData>> mapData = new Dictionary<string, ActivityStatsHeader<ActivityStatsData>>();

            foreach (ActivityStatsData cellData in p_activities)
            {
                DateTime epoch = KreyosUtils.ToDateTime((long)cellData.EpochTime);
                string dateKey = KreyosUtils.DateString(epoch);

                if (!mapData.ContainsKey(dateKey))
                {
                    ActivityStatsHeader<ActivityStatsData> headerData = new ActivityStatsHeader<ActivityStatsData>((uint)cellData.EpochTime);
                    mapData.Add(dateKey, headerData);
                    list.Add(headerData);
                }

                mapData[dateKey].Add(cellData);
                mapData[dateKey].UpdateTotalValues();
                Debug.WriteLine("MainPage::ListFrom epoch:" + epoch + " dateKey:" + dateKey + " count:" + list.Count + " headCount:" + mapData[dateKey].Count + " unix:" + (long)cellData.EpochTime);
            }

            return list;
        }

        public static void FilterActivityStats (List<ActivityStatsHeader<ActivityStatsData>> p_list)
        {
            foreach (ActivityStatsHeader<ActivityStatsData> header in p_list)
            {
                ActivityStatsHeader<ActivityStatsData> headerHolder = ActivityStatsHeader<ActivityStatsData>.CopyData( header );
                header.Clear();
                foreach (ActivityStatsData cell in headerHolder)
                {
                    if (!header.Contains(cell.ActType))
                    {
                        header.Add(cell);
                    }
                    else
                    {
                        ActivityStatsData filtered = header.GetAny(cell.ActType);
                        filtered.Steps += cell.Steps;
                        filtered.Distance += cell.Distance;
                        filtered.Calories += cell.Calories;
                        filtered.Altitude += cell.Altitude;
                    }
                }

                // update data for sub slider
                foreach (ActivityStatsData cell in header)
                {
                    cell.UpdateData();
                }
            }
        }

        public static ActivityStatsHeader<T> CopyData (ActivityStatsHeader<T> p_list)
        {
            ActivityStatsHeader<T> dup = new ActivityStatsHeader<T>(p_list.UnixTime);

            foreach (T cell in p_list)
            {
                dup.Add(cell);
            }

            return dup;
        }
    }
}
