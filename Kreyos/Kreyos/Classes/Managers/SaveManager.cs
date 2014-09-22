using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Managers
{
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Managers;
    using Kreyos.SDK.Bluetooth;

    /// <summary>
    /// User Activity Columns
    /// </summary>
    public enum UserAct
    {
        ActivityBestLap = 0,
        ActivityAvgLap,
        ActivityCurrentLap,
        ActivityAvgPace,
        ActivityPace,
        ActivityTopSpeed,
        ActivityAvgSpeed,
        ActivitySpeed,
        ActivityElevation,
        ActivityAltitude,
        ActivityMaxHeart,
        AvgActivityHeart,
        ActivityHeart,
        Activity_ID,
        Sport_ID,
        KreyosUserID,
        ActivityCalories,
        ActivitySteps,
        Coordinates,
        ActivityDistance,
        CreatedTime
    };

    /// <summary>
    /// User Profile Columns
    /// </summary>
    public enum UserProfile
    {
        ID,
        Email,
        FacebookID,
        TwitterID,
        GoogleID,
        Password,
        Name,
        Gender,
        Height,
        Weight,
        Birthday,
    };

    public sealed class SaveManager
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile SaveManager m_instance      = null;
        private static object m_lockObject                  = new Object();

        /****************************************************************
         * Singleton
         **/
        public static SaveManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new SaveManager(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Constants
         **/
        public static readonly Dictionary<UserAct, string> UserActivityKeys = new Dictionary<UserAct, string>()
        {
            { UserAct.ActivityBestLap,			"ActivityBestLap" },
            { UserAct.ActivityAvgLap,			"ActivityAvgLap" },									
            { UserAct.ActivityCurrentLap,		"ActivityCurrentLap" },								
            { UserAct.ActivityAvgPace,		    "ActivityAvgPace" },																		
            { UserAct.ActivityPace,				"ActivityPace" },						
            { UserAct.ActivityTopSpeed,			"ActivityTopSpeed" },							
            { UserAct.ActivityAvgSpeed,			"ActivityAvgSpeed" },							
            { UserAct.ActivitySpeed,			"ActivitySpeed" },						
            { UserAct.ActivityElevation,		"ActivityElevation" },							
            { UserAct.ActivityAltitude,			"ActivityAltitude" },							
            { UserAct.ActivityMaxHeart,			"ActivityMaxHeart" },															
            { UserAct.AvgActivityHeart,			"AvgActivityHeart" },															
            { UserAct.ActivityHeart,			"ActivityHeart" },						
            { UserAct.Activity_ID,				"Activity_ID" },						
            { UserAct.Sport_ID,					"Sport_ID" },					
            { UserAct.KreyosUserID,				"KreyosUserID" },				
            { UserAct.ActivityCalories,			"ActivityCalories" },					
            { UserAct.ActivitySteps,			"ActivitySteps" },				
            { UserAct.Coordinates,				"Coordinates" },				
            { UserAct.ActivityDistance,			"ActivityDistance" },					
            { UserAct.CreatedTime,				"CreatedTime" }									
        };

        public static readonly Dictionary<UserProfile, string> UserProfileKeys = new Dictionary<UserProfile, string>()
        {
            { UserProfile.ID,			        "ID" },                 
            { UserProfile.Email,		        "Email" },                 
            { UserProfile.FacebookID,	        "FacebookID" },                 
            { UserProfile.TwitterID,	        "TwitterID" },                 
            { UserProfile.GoogleID,		        "GoogleID" },                 
            { UserProfile.Password,		        "Password" },                 
            { UserProfile.Name,			        "Name" },                 
            { UserProfile.Gender,		        "Gender" },                 
            { UserProfile.Height,		        "Height" },                 
            { UserProfile.Weight,		        "Weight" },                 
            { UserProfile.Birthday,		        "Birthday" } 
        };

        /****************************************************************
         * Instance Properties
         **/
        private List<Kreyos_User_Activities> m_userActivities;

        /****************************************************************
         * Constructors
         **/
        private SaveManager ()
        {
            m_userActivities = new List<Kreyos_User_Activities>();
        }

        public void Init (string p_user)
        {
            this.KreyosUser = p_user;
            DatabaseManager.Instance.Init(p_user);
            PrefsManager.Instance.Init(p_user);
        }

        /****************************************************************
         * Getters | Setters
         **/
        public string KreyosUser { get; private set; }

        /****************************************************************
         * Public Functionalities
         **/
        public void AddActivities (ActivityDataDoc p_data)
        {
            if (p_data == null) { return; }
            if (p_data.data == null) { return; }
            if (p_data.data.Count == 0) { return; }
            List<ActivityDataRow> unitData = (List<ActivityDataRow>)p_data.data;

            foreach (ActivityDataRow row in unitData)
            {
                Kreyos_User_Activities act = new Kreyos_User_Activities();
                act.UpdateFromRow(row);
                this.AddActivity(act);
            }

            this.Save();
        }

        public void AddActivity(Kreyos_User_Activities p_act)
        {
            m_userActivities.Add(p_act);
        }

        public void AddProfile (Kreyos_User_Profile p_profile)
        {
            //~~~save data to prefs
            PrefsManager.Instance.SaveUserProfile(p_profile);
        }

        public void Save ()
        {
            //~~~save  data to db
            DatabaseManager.Instance.SaveUserAct(m_userActivities);
            //~~~save data to prefs
            PrefsManager.Instance.SaveUserAct(m_userActivities);
            //~~~save data to web
            //~~~reset data
            m_userActivities.Clear();
        }
    }
}
