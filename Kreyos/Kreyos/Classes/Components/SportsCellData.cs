using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Components
{
    using Kreyos.Classes.Utils;

    public class SportsCellData
    {
        /****************************************************************
         * Constants
         **/
        public static readonly string TITLE_SPEED = "SPEED";
        public static readonly string TITLE_DISTANCE = "DISTANCE";
        public static readonly string TITLE_STEPS = "STEPS";
        public static readonly string TITLE_HEART_RATE = "HEART RATE";
        public static readonly string TITLE_AVG_HEART_RATE = "AVG HEART RATE";
        public static readonly string TITLE_MAX_HEART_RATE = "MAX HEART RATE";
        public static readonly string TITLE_CALORIES = "CALORIES";
        public static readonly string TITLE_ALTITUDE = "ALTITUDE";

        public static readonly string METRIC_SPEED = "KPH";
        public static readonly string METRIC_DISTANCE = "MTR";
        public static readonly string METRIC_STEPS = "STEPS";
        public static readonly string METRIC_HEART_RATE = "BPM";
        public static readonly string METRIC_AVG_HEART_RATE = "BPM";
        public static readonly string METRIC_MAX_HEART_RATE = "BPM";
        public static readonly string METRIC_CALORIES = "CAL";
        public static readonly string METRIC_ALTITUDE = "FT";

        public static readonly string CURSOR_INVALID = string.Empty;
        public static readonly string CURSOR_0 = "0";
        public static readonly string CURSOR_1 = "1";
        public static readonly string CURSOR_2 = "2";
        public static readonly string CURSOR_3 = "3";

        private static readonly Dictionary<ECell, string> TITLE_MAP = new Dictionary<ECell, string>()
        {
            { ECell.EC_Speed,           TITLE_SPEED },
            { ECell.EC_Distance,        TITLE_DISTANCE },
            { ECell.EC_Steps,           TITLE_STEPS },
            { ECell.EC_HeartRate,       TITLE_HEART_RATE },
            { ECell.EC_AvgHeartRate,    TITLE_AVG_HEART_RATE },
            { ECell.EC_MaxHeartRate,    TITLE_MAX_HEART_RATE },
            { ECell.EC_Calories,        TITLE_CALORIES },
            { ECell.EC_Altitude,        TITLE_ALTITUDE },
        };

        private static readonly Dictionary<ECell, string> METRIC_MAP = new Dictionary<ECell, string>()
        {
            { ECell.EC_Speed,           METRIC_SPEED },
            { ECell.EC_Distance,        METRIC_DISTANCE },
            { ECell.EC_Steps,           METRIC_STEPS },
            { ECell.EC_HeartRate,       METRIC_HEART_RATE },
            { ECell.EC_AvgHeartRate,    METRIC_AVG_HEART_RATE },
            { ECell.EC_MaxHeartRate,    METRIC_MAX_HEART_RATE },
            { ECell.EC_Calories,        METRIC_CALORIES },
            { ECell.EC_Altitude,        METRIC_ALTITUDE },
        };

        private static readonly List<string> CURSOR_LIST = new List<string>()
        {
            CURSOR_0, CURSOR_1, CURSOR_2, CURSOR_3
        };

        /****************************************************************
         * Properties
         **/
        private ECell m_type;
        private float m_value;


        /****************************************************************
         * Constructors
         **/
        public SportsCellData (string p_title, string p_unit, string p_value)
        {
            this.Title = p_title;
            this.Unit = p_unit;
            this.Value = p_value;
            this.Cursor = CURSOR_INVALID;
        }

        public SportsCellData (ECell p_type) :
            this(p_type, 0.0f)
        {
        }

        public SportsCellData (ECell p_type, float p_value)
        {
            m_type = p_type;
            this.Title = TITLE_MAP[p_type];
            this.Unit = METRIC_MAP[p_type];
            this.Value = p_value.ToString();
            this.Cursor = CURSOR_INVALID;
        }

        /****************************************************************
         * Getter | Setteres
         **/
        public string Title { get; private set; }
        public string Unit { get; private set; }
        public string Value { get; private set; }
        public string Cursor { get; set; }


        /****************************************************************
         * Helpers
         **/
        public static string TitleFrom (ECell p_cell)
        {
            return TITLE_MAP[p_cell];
        }

        public static string MetricFrom (ECell p_cell)
        {
            return METRIC_MAP[p_cell];
        }

        public static string CursorAt (int p_index)
        {
            if (p_index < 0 || p_index > CURSOR_LIST.Count - 1) { return null; }
            return CURSOR_LIST[p_index];
        }
    }
}
