using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Managers
{
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Components;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;

    public sealed class PrefsManager
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile PrefsManager m_instance = null;
        private static object m_lockObject = new Object();

        /****************************************************************
         * Singleton
         **/
        public static PrefsManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new PrefsManager(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Constants
         **/
        public static readonly string KREYOS_USER_PREFS = "kreyos_user_prefs";
        public static readonly string KREYOS_ACTIVITIES = "User_Activities";
        public static readonly string KREYOS_PROFILE    = "User_Profile";

        /****************************************************************
         * Instance Properties
         **/
        private IsolatedStorageSettings m_prefs = IsolatedStorageSettings.ApplicationSettings;
        private List<string> m_userActivityDates;

        /****************************************************************
         * Constructors
         **/
        private PrefsManager ()
        {
        }

        public void Init (string p_user)
        {
            this.KreyosPrefsKey = p_user + "_" + KREYOS_USER_PREFS;
            
            //~~~TODO:
            //      Reload the following data. (m_userActivityDates, etc)
            m_userActivityDates = this.Reload(KREYOS_ACTIVITIES + "_" + this.KreyosPrefsKey);
        }

        /****************************************************************
         * Getters | Setters
         **/
        public string KreyosPrefsKey { get; private set; }
        public List<Kreyos_User_Activities> Activities(ulong p_epoch)
        {
            DateTime epoch = KreyosUtils.ToDateTime((long)p_epoch);
            string dateKey = this.KreyosPrefsKey + KreyosUtils.DateString(epoch);
            List<Kreyos_User_Activities> activities = null;

            try
            {
                activities = (List<Kreyos_User_Activities>)m_prefs[dateKey];
            }
            catch (KeyNotFoundException ex)
            {
                return null;
            }

            return activities;
        }

        public List<Kreyos_User_Activities> OverallActivities()
        {
            List<Kreyos_User_Activities> activities = new List<Kreyos_User_Activities>();

            foreach (string epoch in m_userActivityDates)
            {
                List<Kreyos_User_Activities> acts;

                try
                {
                    acts = (List<Kreyos_User_Activities>)m_prefs[epoch];
                }
                catch (KeyNotFoundException ex)
                {
                    acts = null;
                }

                if (acts == null) { continue; }

                activities.AddRange(acts);
            }

            //~~~sort
            activities = activities.OrderBy(act => act.CreatedTime).ToList();

            return activities;
        }

        /****************************************************************
         * Public Functionalities
         *      Assumes that p_acts contains 'today's data' only.
         **/
        public void SaveUserAct(List<Kreyos_User_Activities> p_acts)
        {
            if (p_acts == null || p_acts.Count < 1) { return; }

            uint epochTime = p_acts[0].CreatedTime;
            DateTime epoch = KreyosUtils.ToDateTime((long)epochTime);
            string dateKey = this.KreyosPrefsKey + KreyosUtils.DateString(epoch);

            List<Kreyos_User_Activities> activities = null;

            try
            {
                //~~~Old Items
                activities = (List<Kreyos_User_Activities>)m_prefs[dateKey];
            }
            catch (KeyNotFoundException ex)
            {
                //~~~New Items
                //      sort activities
                p_acts = p_acts.OrderBy(act => act.CreatedTime).ToList();
                m_prefs[dateKey] = (List<Kreyos_User_Activities>)p_acts;
                m_userActivityDates.Add(dateKey);
                this.Save();
                return;
            }

            if (activities == null)
            {
                activities = new List<Kreyos_User_Activities>();
            }

            //~~~insert the new items
            activities.AddRange(p_acts);

            //~~~sort
            activities = activities.OrderBy(act => act.CreatedTime).ToList();
            m_prefs[dateKey] = (List<Kreyos_User_Activities>)activities;

            //~~~temp save
            //      this shouldn't be called here
            this.Save();
        }

        public void SaveUserProfile (Kreyos_User_Profile p_profile)
        {
            string profilePrefix = KREYOS_PROFILE + "_" + this.KreyosPrefsKey;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.ID]] = p_profile.ID;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Email]] = p_profile.Email;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.FacebookID]] = p_profile.FacebookID;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.TwitterID]] = p_profile.TwitterID;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.GoogleID]] = p_profile.GoogleID;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Password]] = p_profile.Password;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Name]] = p_profile.Name;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Gender]] = p_profile.Gender;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Height]] = p_profile.Height;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Weight]] = p_profile.Weight;
            m_prefs[profilePrefix + SaveManager.UserProfileKeys[UserProfile.Birthday]] = p_profile.Birthday;

            //~~~temp save
            //      this shouldn't be called here
            this.Save();
        }

        public void Save()
        {
            m_prefs.Add(KREYOS_ACTIVITIES + "_" + this.KreyosPrefsKey, m_userActivityDates);
            //m_prefs.Save();
        }

        /****************************************************************
         * Private Functionalities
         **/
        private List<string> Reload (string p_key)
        {
            List<string> con = null;

            try
            {
                //~~~Old Items
                con = (List<string>)m_prefs[p_key];
            }
            catch (KeyNotFoundException ex)
            {
                //~~~New Items
                con = new List<string>();
            }

            return con;
        }
    }
}
