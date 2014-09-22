using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Utils
{
    using Kreyos.SDK.Bluetooth;

    public enum Gender
    {
        Male = 0,
        Female
    };

    public enum EAct_Types : uint
    {
        EWalking = 0,
        ERunning,
        ECycling
    };

    public enum ESports : int
    {
        EGrid2x1 = 0,
        EGrid3x1,
        EGrid2x2
    };

    /****************************************************************
     * Default Values:
     * Cycling
     *  Speed
     *  Distance
     *  Altitude
     *  
     * Running
     *  Speed
     *  Distance
     * 
     **/
    public enum ECell
    {
        EC_Speed,
        EC_Distance,
        EC_Steps,
        EC_HeartRate,
        EC_AvgHeartRate,
        EC_MaxHeartRate,
        EC_Calories,
        EC_Altitude,
    };

    public class KreyosConstants
    {
        public static bool IS_DEV_MODE = false;
        public static bool IS_LIVE_ON = true;
        public static string LOCAL_IP = "http://192.168.1.115:3000";
        public static string LIVE_URL = IS_LIVE_ON ? "https://members.kreyos.com/" : "https://kreyos-members.herokuapp.com/";
        public static string URL_CREATE_ACCOUNT = IS_DEV_MODE ? LOCAL_IP + "/api/users" : LIVE_URL + "api/users";
        public static string URL_CHECK_MAIL = IS_DEV_MODE ? LOCAL_IP + "/api/users/check_email" : LIVE_URL + "api/users/check_email";
        public static string URL_LOGIN_CHECK = IS_DEV_MODE ? LOCAL_IP + "/api/sessions" : LIVE_URL + "api/sessions";
        public static string URL_SESSION_KEY = IS_DEV_MODE ? LOCAL_IP + "/api/persistence" : LIVE_URL + "api/persistence";
        public static string URL_USER_UPDATE = IS_DEV_MODE ? LOCAL_IP + "/api/users/update" : LIVE_URL + "users/update";
        public static string URL_USER_ACTIVITIES = IS_DEV_MODE ? LOCAL_IP + "/api/activities" : LIVE_URL + "api/activities";
        public static string URL_FIRMWARE = IS_DEV_MODE ? LOCAL_IP + "/api/firmwares/latest_firmware" : LIVE_URL + "api/firmwares/latest_firmware";
        public static string URL_DELETE_SESSION = IS_DEV_MODE ? LOCAL_IP + "/api/logout" : LIVE_URL + "api/logout";
        public static string URL_FACEBOOK_LOGIN = IS_DEV_MODE ? LOCAL_IP + "/api/login_via_facebook" : LIVE_URL + "api/login_via_facebook";
        public static string URL_GET_USER_ACTIVITIES = IS_DEV_MODE ? LOCAL_IP + "/api/activities" : LIVE_URL + "api/activities";

        public static readonly Dictionary<Gender, string> UserGender = new Dictionary<Gender, string>()
        {
            { Gender.Male,      "Male" },                 
            { Gender.Female,    "Female" }
        };

        public static readonly float SCREEN_INTERVAL = 0.01f;
        public static readonly float SPORTS_INTERVAL = 1.00f;
    }
}
