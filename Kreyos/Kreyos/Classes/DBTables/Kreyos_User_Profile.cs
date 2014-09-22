using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.DBTables
{
    using Kreyos.Classes.Utils;

    public class Kreyos_User_Profile
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile Kreyos_User_Profile m_instance = null;
        private static object m_lockObject = new Object();

        /****************************************************************
         * Singleton
         **/
        public static Kreyos_User_Profile Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new Kreyos_User_Profile(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Constructors
         **/
        private Kreyos_User_Profile ()
        {
            this.Clear();
        }

        /****************************************************************
         * Getters | Setters
         **/
        /// <summary>
        /// Kreyos User ID
        /// </summary>
        public string ID { get; set; }
        public string Email { get; set; }
        public string FacebookID { get; set; }
        public string TwitterID { get; set; }
        public string GoogleID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        /// <summary>
        /// cm
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// lbs
        /// </summary>
        public int Weight { get; set; }
        public string Birthday { get; set; }

        /// <summary>
        /// Flag on whenever there is a profile already created.
        /// </summary>
        public bool IsValid { get; set; }

        /****************************************************************
         * Public Functionalities
         **/
        public void Clear ()
        {
            this.ID = string.Empty;
            this.Email = string.Empty;
            this.FacebookID = string.Empty;
            this.TwitterID = string.Empty;
            this.GoogleID = string.Empty;
            this.Password = string.Empty;
            this.Name = string.Empty;
            this.LastName = string.Empty;
            this.Gender = -1;
            this.Height = -1;
            this.Weight = -1;
            this.Birthday = string.Empty;
        }

        /// <summary>
        /// Debug print the User Profile data
        /// </summary>
        public void Print ()
        {
            KreyosUtils.Log("KreyosUserProfile::Print", "ID              " + this.ID);        
            KreyosUtils.Log("KreyosUserProfile::Print", "Email           " + this.Email);     
            KreyosUtils.Log("KreyosUserProfile::Print", "FacebookID      " + this.FacebookID);
            KreyosUtils.Log("KreyosUserProfile::Print", "TwitterID       " + this.TwitterID); 
            KreyosUtils.Log("KreyosUserProfile::Print", "GoogleID        " + this.GoogleID);  
            KreyosUtils.Log("KreyosUserProfile::Print", "Password        " + this.Password);
            KreyosUtils.Log("KreyosUserProfile::Print", "Name            " + this.Name);
            KreyosUtils.Log("KreyosUserProfile::Print", "LastName        " + this.LastName);  
            KreyosUtils.Log("KreyosUserProfile::Print", "Birthday        " + this.Birthday);  
            KreyosUtils.Log("KreyosUserProfile::Print", "Gender          " + this.Gender);    
            KreyosUtils.Log("KreyosUserProfile::Print", "Height          " + this.Height);
            KreyosUtils.Log("KreyosUserProfile::Print", "Weight          " + this.Weight);    
        }
    }
}
