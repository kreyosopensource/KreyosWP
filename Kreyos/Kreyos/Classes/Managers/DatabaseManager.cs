using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kreyos.Classes.Managers
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;
    using SQLite;
    
    public sealed class DatabaseManager
    {
        /****************************************************************
         * Static Instance and Lock Object
         **/
        private static volatile DatabaseManager m_instance      = null;
        private static object m_lockObject                      = new Object();

        /****************************************************************
         * Singleton
         **/
        public static DatabaseManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    lock (m_lockObject)
                    {
                        if (m_instance == null) { m_instance = new DatabaseManager(); }
                    }
                }

                return m_instance;
            }
        }

        /****************************************************************
         * Constants
         **/
        public static readonly string DB_NAME                   = "kreyos_user_db.sqlite";
        /// <summary>
        /// The following keys are the actual table name
        /// </summary>
        public static readonly string KREYOS_USER_ACTIVITIES    = "Kreyos_User_Activities";
        public static readonly string KREYOS_USER_PROFILE       = "Kreyos_User_Profile";

        /****************************************************************
         * Instance Properties
         **/
        /// <summary>
        /// The sqlite connection.
        /// </summary>
        private SQLiteConnection m_dbConnection;

        /****************************************************************
         * Constructors
         **/
        private DatabaseManager ()
        {
        }

        public void Init (string p_user)
        {
            this.KreyosDBPath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, p_user + "_" + DB_NAME));
            this.KreyosDBName = p_user + "_" + DB_NAME;
            this.OnCreate(this.KreyosDBPath, KreyosDBName);
        }

        /****************************************************************
         * Getters | Setters
         **/
        public string KreyosDBPath { get; private set; }
        public string KreyosDBName { get; private set; }

        /****************************************************************
         * Public Functionalities
         **/
        public void SaveUserAct(List<Kreyos_User_Activities> p_acts)
        {
            //~~~save data ti db
        }

        /****************************************************************
         * Private Functionalities
         **/
        private async Task<bool> OnCreate (string p_dbPath, string p_dbName)
        {
            KreyosUtils.Log("DatabaseManager::OnCreate", "creating db at path:" + p_dbPath);

            try
            {
                bool dbExisted = this.CheckFileExists(p_dbName).Result;

                KreyosUtils.Log("DatabaseManager::OnCreate", "db:" + p_dbName + " exists? " + dbExisted);

                if (!dbExisted)
                {
                    KreyosUtils.Log("DatabaseManager::OnCreate", "a..");
                    using (m_dbConnection = new SQLiteConnection(p_dbPath))
                    {
                        KreyosUtils.Log("DatabaseManager::OnCreate", "b..");
                        m_dbConnection.CreateTable<Kreyos_User_Activities>();
                        KreyosUtils.Log("DatabaseManager::OnCreate", "New db..");
                    }
                }
                else
                {
                    KreyosUtils.Log("DatabaseManager::OnCreate", "Existing db..");
                }

                return true;
            }
            catch (FileNotFoundException e)
            {
                KreyosUtils.Log("DatabaseManager::OnCreate", "No db.. name:" + e.FileName + " error:" + e.Message + " " + e.StackTrace);
                return false;
            } 
        }

        private async Task<bool> CheckFileExists (string p_fileName)
        {
            try
            {
                var store = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(p_fileName);
                return true;
            }
            catch
            {
                return false;
            }
        } 

        //~~retrieve activity from db
        public Kreyos_User_Activities ReadActivity (uint p_epoch)
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                var existingconact = dbConn.Query<Kreyos_User_Activities>("select * from Kreyos_User_Activities where CreatedTime =" + p_epoch).FirstOrDefault();
                return existingconact;
            }

            return null;
        }

        //~~~retrieve all activities fromd b 
        public ObservableCollection<Kreyos_User_Activities> ReadActivities ()
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                List<Kreyos_User_Activities> myCollection = dbConn.Table<Kreyos_User_Activities>().ToList<Kreyos_User_Activities>();
                ObservableCollection<Kreyos_User_Activities> ContactsList = new ObservableCollection<Kreyos_User_Activities>(myCollection);
                return ContactsList;
            }

            return null;
        } 

        //~~~update existing activity
        public void UpdateActivity (Kreyos_User_Activities p_activity)
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                var existingActivity = dbConn.Query<Kreyos_User_Activities>("select * from Kreyos_User_Activities where CreatedTime =" + p_activity.CreatedTime).FirstOrDefault();
                if (existingActivity != null)
                {
                    existingActivity.Update(p_activity);                             

                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Update(existingActivity);
                    });
                }
            }
        }

        //~~~insert new activity
        public void InsertActivity (Kreyos_User_Activities p_activity)
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                dbConn.RunInTransaction(() =>
                {
                    dbConn.Insert(p_activity);
                });
            }
        } 

        //~~~delete activity
        public void DeleteActivity (int p_epoch)
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                var existingActivity = dbConn.Query<Kreyos_User_Activities>("select * from Kreyos_User_Activities where CreatedTime =" + p_epoch).FirstOrDefault();
                if (existingActivity != null)
                {
                    dbConn.RunInTransaction(() =>
                    {
                        dbConn.Delete(existingActivity);
                    });
                }
            }
        }

        //~~~delete all activities
        public void DeleteActivities ()
        {
            using (var dbConn = new SQLiteConnection(this.KreyosDBPath))
            {
                //dbConn.RunInTransaction(() => 
                //   { 
                dbConn.DropTable<Kreyos_User_Activities>();
                dbConn.CreateTable<Kreyos_User_Activities>();
                dbConn.Dispose();
                dbConn.Close();
                //}); 
            }
        } 
    }
}
