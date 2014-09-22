using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Kreyos.Classes.Screens
{
    using Kreyos.Classes.Components;
    using Kreyos.Classes.DBTables;
    using Kreyos.Classes.Interface;
    using Kreyos.Classes.Managers;
    using Kreyos.Classes.Utils;
    using Kreyos.SDK.Bluetooth;
    using System.Windows.Media;
    

    public partial class HomeScreen : PhoneApplicationPage
    {
        private List<ObserverInfo> m_commands = new List<ObserverInfo>();
        private DispatcherTimer m_commandTimer;

        private Pivot m_mainPivot;
        private bool[] m_loadedScreen = new bool[]
        {
            false,
            false,
            false,
            false,
            false,
            false
        };

        public HomeScreen ()
        {
            InitializeComponent();

            //~~~init properties
            m_mainPivot = this.MainPivot;

            //~~~init listeners
            BluetoothObserver.Instance.OnReceivedEvent += new Delegate_HandleCommand(this.HandleCommand);

            //~~~init command timer
            m_commandTimer = new DispatcherTimer();
            m_commandTimer.Interval = TimeSpan.FromSeconds(KreyosConstants.SCREEN_INTERVAL);
            m_commandTimer.Tick += UpdateCommands;
            m_commandTimer.Start();
        }

        public void AddCommand (ObserverInfo p_info)
        {
            m_commands.Add(p_info);
        }

        private void UpdateCommands (Object p_sender, EventArgs p_args)
        {
            if (m_commands.Count == 0) { return; }

            ObserverInfo info = m_commands[0] as ObserverInfo;

            switch (info.Command)
            {
                case EBTEvent.BTE_OnTodaysActivity:
                {
                    this.UpdateTodaysScreen(info.TodaysData);
                }
                break;

                case EBTEvent.BTE_OnOverallActivity:
                {
                    this.UpdateOverallScreen(info.OverallData);
                }
                break;

                case EBTEvent.BTE_OnReadySportsMode:
                {
                    this.MoveToPage(EPivotPage.SportsMode);
                }
                break;

                case EBTEvent.BTE_OnStartSportsMode:
                {
                    this.OnStartSportsTimer(info.SportsData);
                }
                break;

                case EBTEvent.BTE_OnFinishSportsMode:
                {
                    this.OnFinishSports();
                }
                break;
            }

            //~~~remove the first command
            m_commands.RemoveAt(0);
        }

        /// <summary>
        /// Disable "Back Key"
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress (System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// Handle commands
        /// </summary>
        private void HandleCommand (ObserverInfo p_command)
        {
            this.AddCommand (p_command);
        }

        /// <summary>
        /// Override navigate event to change the Pivot's currect item.
        /// Call this when you're not in MainScreen.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            string itemIndex;
            if (NavigationContext.QueryString.TryGetValue("goto", out itemIndex))
            {
                EPivotPage page = (EPivotPage)Convert.ToInt32(itemIndex);
                this.MoveToPage(page);
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Call this when you're in MainScreen
        /// </summary>
        /// <param name="p_page"></param>
        public void MoveToPage (EPivotPage p_page)
        {
            EPivotPage page = (EPivotPage)p_page;
            this.MainPivot.SelectedItem = this.MainPivot.Items[(Int32)page];
        }

        /// <summary>
        /// Handle the change pivot view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPivotChangedPage (object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            KreyosUtils.Log("MainScreen::OnPivotChangedPage", "update, do something here..");
            Pivot page = (Pivot)sender;
            PivotItem item = (PivotItem)page.SelectedItem;
            string pageName = item.Name;
            EPivotPage pageToLoad = ScreenManager.PageMap[pageName];

            //~~~check if screen is already loaded
            if (m_loadedScreen[(int)pageToLoad] == true) 
            {
                this.FetchDataForPage(pageToLoad);
                return; 
            }

            //~~~set the flag to load
            m_loadedScreen[(int)pageToLoad] = true;

            switch (pageToLoad)
            {
                case EPivotPage.TodaysActivity:
                {
                    this.InitTodaysActivity();
                    item.UpdateLayout();

                }
                break;

                case EPivotPage.OverallActivity:
                {
                    this.InitActivityStats();
                    item.UpdateLayout();
                }
                break;

                case EPivotPage.SportsMode:
                {
                    this.InitSportsMode();
                    item.UpdateLayout();
                }
                break;

                case EPivotPage.DailyTarget:
                {
                    this.InitDailyTarget();
                    item.UpdateLayout();
                }
                break;

                case EPivotPage.PersonalProfile:
                {
                    this.InitProfile();
                    item.UpdateLayout();
                }
                break;

                case EPivotPage.Settings:
                {
                }
                break;
            }
            
            KreyosUtils.Log("MainScreen::OnPivotChangedPage", "Loaded Page:"+pageName);
        }

        /// <summary>
        /// Fetch data on specfici swcreen
        /// </summary>
        /// <param name="p_page"></param>
        private void FetchDataForPage (EPivotPage p_page)
        {
            switch (p_page)
            {
                case EPivotPage.TodaysActivity:
                {
                    this.FetchTodaysData();
                }
                break;

                case EPivotPage.OverallActivity:
                {
                    this.FetchOverallData();
                }
                break;

                case EPivotPage.SportsMode:
                {
                }
                break;

                case EPivotPage.DailyTarget:
                {
                }
                break;

                case EPivotPage.PersonalProfile:
                {
                }
                break;

                case EPivotPage.Settings:
                {
                }
                break;
            }
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, IHomeScreen
    {
        // green baord texts
        private TextBlock m_hours;
        private TextBlock m_minutes;
        private TextBlock m_seconds;

        public void InitTodaysActivity ()
        {
            // Initialize green board labels ( Total Hours, Mins and Seconds Values )
            m_hours = txt_hrs;
            m_minutes = txt_min;
            m_seconds = txt_sec;

            m_hours.Text = "0";
            m_minutes.Text = "0";
            m_seconds.Text = "0";
        }

        public void UpdateTodaysScreen (TodayActivity p_todaysData)
        {
            TodayActivity todaysData = p_todaysData;

            // TODO: Create radial meter here. for temporary, display values with the single activity data
            List<ActivityStatsData> activities = new List<ActivityStatsData>();
            ActivityStatsData stats = new ActivityStatsData(EAct_Types.EWalking, (ulong)KreyosUtils.EpochFromNow(), (uint)todaysData.steps, (uint)todaysData.distance, (uint)todaysData.calories, (uint)0);
            activities.Add(stats);
            this.DisplayActivities(activities);

            //~~~display time.
            //      Note: On todays data, time is displayed as 0, so.. use the EpochFromNow instead
            m_hours.Text = "" + stats.Hour;
            m_minutes.Text = "" + stats.Minute;
            m_seconds.Text = "" + stats.Second;

            //~~~update selected page's layout
            PivotItem item = (PivotItem)this.MainPivot.SelectedItem;
            item.UpdateLayout();
        }

        private void DisplayActivities (List<ActivityStatsData> p_act)
        {
            // Generate Header Data
            List<ActivityStatsHeader<ActivityStatsData>> cells = ActivityStatsHeader<ActivityStatsData>.ListFrom(p_act);

            // Filter Data
            ActivityStatsHeader<ActivityStatsData>.FilterActivityStats(cells);

            // Set data to your ListView
            TodaysActivityScroller.ItemsSource = cells;
        }

        public void FetchTodaysData ()
        {
            BluetoothManager.Instance.GetTodayActivity();
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, IActivityStatsScreen
    {
        public void InitActivityStats ()
        {
            //this.InitActivityStatsScroller();

            //~~~init default values
            SaveManager.Instance.Init("Mitortol");

            //~~~display data from local
            this.DisplayDataFromLocal();
        }

        private void UpdateOverallScreen (ActivityDataDoc p_overallData)
        {
            //~~~add activities to local
            SaveManager.Instance.AddActivities(p_overallData);

            //~~~display data from local
            this.DisplayDataFromLocal();
        }

        private void DisplayDataFromLocal ()
        {
            //~~~get activities to local
            List<Kreyos_User_Activities> userAct = PrefsManager.Instance.OverallActivities();
            List<ActivityStatsData> activities = new List<ActivityStatsData>();

            //~~~generate data from prefs
            foreach (Kreyos_User_Activities act in userAct)
            {
                activities.Add(act.ToActivityView());
            }

            //~~~display overall data
            this.DisplayOverallActivities(activities);
        }

        public void DisplayOverallActivities (List<ActivityStatsData> p_activities)
        {
            // Generate Header Data
            List<ActivityStatsHeader<ActivityStatsData>> cells = ActivityStatsHeader<ActivityStatsData>.ListFrom(p_activities);

            // Filter Data
            ActivityStatsHeader<ActivityStatsData>.FilterActivityStats(cells);

            // Set data to your ListView
            ActivityStatsScroller.ItemsSource = cells;
        }
        
        public void FetchOverallData ()
        {
            BluetoothManager.Instance.GetOverallActivities();
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, ISportsScreen
    {
        private bool m_sportsIsInitialized = false;

        private LongListSelector[] m_sports = new LongListSelector[3];
        private List<LongListSelector> m_sportGrids = new List<LongListSelector>();
        private DispatcherTimer m_gridUpdate;
        private ESports m_gridIndex;
        private List<SportsCellData> m_cellData = new List<SportsCellData>();
        private Image m_buttonAdd;

        private TextBlock m_txtRunning;
        private TextBlock m_txtCycling;
        private TextBlock m_txtTime;

        private int m_sportsTime;
        private EAct_Types m_sportsMode;
        private DispatcherTimer m_sportsTimer;
        private SportsDataRow m_sportsData;
        
        //~~~label brush
        private readonly SolidColorBrush ENABLED_BRUSH  = new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0x26, 0x26));
        private readonly SolidColorBrush DISABLED_BRUSH = new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x99, 0x99));

        //~~~temp cell data
        private Dictionary<ECell, SportsCellData> m_cachedCells = new Dictionary<ECell, SportsCellData>()
        {
            { ECell.EC_Speed,           new SportsCellData(ECell.EC_Speed) },           
            { ECell.EC_Distance,        new SportsCellData(ECell.EC_Distance) },
            { ECell.EC_Steps,           new SportsCellData(ECell.EC_Steps) },
            { ECell.EC_HeartRate,       new SportsCellData(ECell.EC_HeartRate) },
            { ECell.EC_AvgHeartRate,    new SportsCellData(ECell.EC_AvgHeartRate) },
            { ECell.EC_MaxHeartRate,    new SportsCellData(ECell.EC_MaxHeartRate) },
            { ECell.EC_Calories,        new SportsCellData(ECell.EC_Calories) },
            { ECell.EC_Altitude,        new SportsCellData(ECell.EC_Altitude) },
        };

        private ECell m_cachedIndex = ECell.EC_Speed;

        public void InitSportsMode ()
        {
            m_sportsIsInitialized = true;

            this.AddCell(m_cachedCells[ECell.EC_Steps]);
            this.AddCell(m_cachedCells[ECell.EC_Distance]);
            this.AddCell(m_cachedCells[ECell.EC_Altitude]);
            this.AddCell(m_cachedCells[ECell.EC_Calories]);

            m_sports[(int)ESports.EGrid2x1] = this.SportsGrid1x2;
            m_sports[(int)ESports.EGrid3x1] = this.SportsGrid3x1;
            m_sports[(int)ESports.EGrid2x2] = this.SportsGrid2x2;

            m_sports[(int)ESports.EGrid2x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid3x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid2x2].Visibility = Visibility.Collapsed;

            m_buttonAdd     = this.img_add;
            m_txtRunning    = this.txt_sportsRunning;
            m_txtCycling    = this.txt_sportsCycling;
            m_txtTime       = this.txt_sportsTimer;

            //~~~update default data to 3x1
            m_gridIndex = ESports.EGrid3x1;
            m_cellData.RemoveAt(m_cellData.Count - 1);

            this.ChangeGridCellSize(m_gridIndex);
            this.StopState();

            //~~~init timers
            m_gridUpdate = new DispatcherTimer();
            m_gridUpdate.Interval = TimeSpan.FromSeconds(KreyosConstants.SCREEN_INTERVAL);
            m_gridUpdate.Tick += this.UpdateSportsGrid;
            m_gridUpdate.Start();

            m_sportsTimer = new DispatcherTimer();
            m_sportsTimer.Interval = TimeSpan.FromSeconds(KreyosConstants.SPORTS_INTERVAL);
            m_sportsTimer.Tick += this.UpdateSportTimer;
        }

        private void UpdateSportsState ()
        {
            //~~~update label color
            switch (m_sportsMode)
            {
                case EAct_Types.ERunning:
                {
                    m_txtRunning.Foreground = ENABLED_BRUSH;
                    m_txtCycling.Foreground = DISABLED_BRUSH;
                }
                break;

                case EAct_Types.ECycling:
                {
                    m_txtRunning.Foreground = DISABLED_BRUSH;
                    m_txtCycling.Foreground = ENABLED_BRUSH;
                }
                break;
            }

            //~~~update grid
            foreach (KeyValuePair<SportsDataRow.DataType, double> pair in m_sportsData.data)
            {
                KreyosUtils.Log("SportsView::UpdateSportsState", "Data " + pair.Key + ":" + pair.Value);
            }
        }

        private void StopState ()
        {
            m_txtRunning.Foreground = DISABLED_BRUSH;
            m_txtCycling.Foreground = DISABLED_BRUSH;
        }

        public void OnStartSportsTimer (SportsDataRow p_sportsData)
        {
            if (!m_sportsIsInitialized) { return; }

            m_sportsData = p_sportsData;
            m_sportsTime = m_sportsData.seconds_elapse;
            m_sportsMode = (EAct_Types)m_sportsData.sports_mode;

            this.UpdateSportsState();
            
            if (m_sportsTimer.IsEnabled) { return; }
            m_sportsTimer.Start();
        }

        public void OnFinishSports ()
        {
            if (!m_sportsIsInitialized) { return; }

            this.StopState();
            m_sportsTimer.Stop();
        }

        private void UpdateSportsGrid (Object p_sender, EventArgs p_args)
        {
            if (m_sportGrids.Count == 0) { return; }

            m_sportGrids[0].ItemsSource = m_cellData;
            m_sportGrids[0].Visibility = Visibility.Visible;
            m_sportGrids[0].UpdateLayout();
            m_sportGrids.RemoveAt(0);
        }

        private void UpdateSportTimer (Object p_sender, EventArgs p_args)
        {
            int secods = m_sportsTime;
            int minutes = (m_sportsTime / 60);
            int hours = minutes / 60;

            secods -= minutes * 60;
            minutes -= hours * 60;

            string strTime = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + secods.ToString("D2");
            m_txtTime.Text = strTime;

            m_sportsTime++;
        }

        public void ChangeGridCellSize (ESports p_count)
        {
            m_sports[(int)ESports.EGrid2x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid3x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid2x2].Visibility = Visibility.Collapsed;

            switch( p_count )
            {
                case ESports.EGrid2x1:
                {
                    SportsGrid1x2.GridCellSize = new Size(SportsGrid1x2.Width, SportsGrid1x2.Height/2.0f);
                    SportsGrid1x2.ItemsSource = m_cellData;
                    m_buttonAdd.Visibility = Visibility.Visible;
                    m_sportGrids.Add(SportsGrid1x2);
                }
                break;

                case ESports.EGrid3x1:
                {
                    SportsGrid3x1.GridCellSize = new Size(SportsGrid3x1.Width, SportsGrid3x1.Height/3.0f);
                    SportsGrid3x1.ItemsSource = m_cellData;
                    m_buttonAdd.Visibility = Visibility.Visible;
                    m_sportGrids.Add(SportsGrid3x1);
                }
                break;

                case ESports.EGrid2x2:
                {
                    SportsGrid2x2.GridCellSize = new Size(SportsGrid2x2.Width/2.0f, SportsGrid2x2.Height/2.0f);
                    SportsGrid2x2.ItemsSource = m_cellData;
                    m_buttonAdd.Visibility = Visibility.Collapsed;
                    m_sportGrids.Add(SportsGrid2x2);
                }
                break;
            }

            //~~~update the grid
            m_gridIndex = (ESports)p_count;
        }

        // Trash & Refresh callbacks
        private void onDelete (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Image deleteButton = (Image)sender;
            int index = int.Parse(deleteButton.Tag.ToString());
            KreyosUtils.Log("SportsView::onDelete", "delete the fucking buttons. TAG:" + index);

            this.RemoveCell(index);
            m_gridIndex--;
            this.ChangeGridCellSize(m_gridIndex);
        }

        private void onRefresh (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            Image refreshButton = (Image)sender;
            int index = int.Parse(refreshButton.Tag.ToString());
            KreyosUtils.Log("SportsView::onRefresh", "refresh the fucking buttons. TAG:"+index);

            while (true)
            {
                SportsCellData newCell = m_cachedCells[m_cachedIndex];

                //~~~Replace the not displayed cell
                if (newCell.Cursor.Equals(SportsCellData.CURSOR_INVALID))
                {
                    KreyosUtils.Log("SportsView::OnRefresh", "CachedIndex:" + m_cachedIndex);

                    this.RemoveCell(index);
                    this.AddCell(newCell, index);
                    this.ChangeGridCellSize(m_gridIndex);
                    break;
                }

                //~~~Increment cached index
                m_cachedIndex++;
                if (m_cachedIndex > ECell.EC_Altitude)
                {
                    m_cachedIndex = ECell.EC_Speed;
                }
            }
        }

        private void OnAdd (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            foreach (KeyValuePair<ECell, SportsCellData> pair in m_cachedCells)
            {
                if (!m_cellData.Contains(pair.Value))
                {
                    this.AddCell(pair.Value);
                    m_gridIndex++;
                    this.ChangeGridCellSize(m_gridIndex);
                    break;
                }
            }
        }

        private void AddCell (SportsCellData p_cell)
        {
            p_cell.Cursor = SportsCellData.CursorAt(m_cellData.Count);
            m_cellData.Add(p_cell);
        }

        private void AddCell (SportsCellData p_cell, int p_index)
        {
            p_cell.Cursor = SportsCellData.CursorAt(p_index);
            m_cellData.Insert(p_index, p_cell);
        }

        private void RemoveCell (int p_index)
        {
            m_cellData[p_index].Cursor = SportsCellData.CURSOR_INVALID;
            m_cellData.RemoveAt(p_index);

            //~~~update cursors
            for (int i = 0; i < m_cellData.Count; i++)
            {
                m_cellData[i].Cursor = SportsCellData.CursorAt(i);
            }
        }

        private void RemoveCell (SportsCellData p_cell)
        {
            p_cell.Cursor = SportsCellData.CURSOR_INVALID;
            m_cellData.Remove(p_cell);

            //~~~update cursors
            for (int i = 0; i < m_cellData.Count; i++)
            {
                m_cellData[i].Cursor = SportsCellData.CursorAt(i);
            }
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, ISettingsScreen
    {
        public void OnDateAndTime (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_DateAndTime);
        }

        public void OnSilentAlarms (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_SilentArams);
        }

        public void OnUpdateFirmware (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_FirmwareUpdate);
        }

        public void OnBluetooth (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            ScreenManager.Instance.Switch(EScreens.ES_Bluetooth);
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, IDailyTargetScreen
    {
        private double[] m_rangeValues = new double[5] { 4000, 6000, 8000, 10000, 12000 };
        private TextBlock[] m_rangeTexts = new TextBlock[5];
        private Button m_btnUpdateWatch;
        private Slider m_sliderSetTarget;

        public void InitDailyTarget ()
        {
            m_rangeTexts[0] = this.txt_snap_0;
            m_rangeTexts[1] = this.txt_snap_1;
            m_rangeTexts[2] = this.txt_snap_2;
            m_rangeTexts[3] = this.txt_snap_3;
            m_rangeTexts[4] = this.txt_snap_4;

            m_btnUpdateWatch = this.btn_update_watch;

            for (int i = 0; i < m_rangeTexts.Length; i++)
            {
                string strValue =  m_rangeValues[i].ToString();
                m_rangeTexts[i].Text = strValue.Substring(0, strValue.Length - 3);
            }

            m_sliderSetTarget = this.slider_set_target;
            m_sliderSetTarget.Minimum = m_rangeValues[0];
            m_sliderSetTarget.Maximum = m_rangeValues[m_rangeValues.Length-1];
        }

        /// <summary>
        /// Handles the OnComplete event of Slider.
        /// Snapping is applied through checking the range values.
        /// To modify the the snapping points, just adjust the 'rangeValues' array.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSliderCompleted (object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            double currentValue = m_sliderSetTarget.Value;

            for (int i = 1; i < m_rangeValues.Length; i++)
            {
                if (currentValue >= m_rangeValues[i])
                {
                    continue;
                }

                double smallRange = Math.Abs(currentValue - m_rangeValues[(i - 1)]);
                double bigRange = Math.Abs(currentValue - m_rangeValues[i]);

                if (smallRange <= bigRange)
                {
                    currentValue = m_rangeValues[(i - 1)];
                }
                else
                {
                    currentValue = m_rangeValues[i];
                }

                m_sliderSetTarget.Value = currentValue;
                txt_test_display_value.Text = string.Format("debug :{0}", currentValue.ToString());
                break;
            }
        }

        private void OnUpdateWatch (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // DeviceConfigManager.Instance.SetNewGoal((int)slider_set_target.Value);
            BluetoothManager.Instance.GetTodayActivity();

            //~~~Sample move to Sports Mode
            //this.OnSportsMode();
        }
    }

    public partial class HomeScreen : PhoneApplicationPage, IProfileScreen
    {
        private Dictionary<TextBox, TextBlock> m_staticLabels;
        private Dictionary<TextBox, string> m_labelValues;
        private RadioButton m_radioMale;
        private RadioButton m_radioFemale;
        private Button m_btnFacebook;
        private Button m_btnUpdate;
        private Image m_imgFacebookPicture;

        public void InitProfile ()
        {
            m_staticLabels = new Dictionary<TextBox, TextBlock>();
            m_staticLabels.Add(this.txt_first_name, this.txt_static_first_name);
            m_staticLabels.Add(this.txt_last_name, this.txt_static_last_name);
            m_staticLabels.Add(this.txt_birthday, this.txt_static_birthday);
            m_staticLabels.Add(this.txt_location, this.txt_static_location);
            m_staticLabels.Add(this.txt_weight, this.txt_static_weight);
            m_staticLabels.Add(this.txt_height, this.txt_static_height);

            m_labelValues = new Dictionary<TextBox, string>();
            m_labelValues.Add(this.txt_first_name, "First Name");
            m_labelValues.Add(this.txt_last_name, "Last Name");
            m_labelValues.Add(this.txt_birthday, "1/01/1970");
            m_labelValues.Add(this.txt_location, "Use my current location");
            m_labelValues.Add(this.txt_weight, "Set one now");
            m_labelValues.Add(this.txt_height, "Set one now");

            // remove display labels
            foreach (KeyValuePair<TextBox, TextBlock> kvp in m_staticLabels)
            {
                kvp.Key.Text = "";
            }

            m_radioMale = this.rb_male;
            m_radioFemale = this.rb_female;
            m_btnFacebook = this.btn_import_facebook;
            m_btnUpdate = this.btn_update_profile;
            m_imgFacebookPicture = this.img_facebook_picture;
        }

        private void OnLabelGotFocus (object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox targetLabel = (TextBox)sender;
            TextBlock label = m_staticLabels[targetLabel];

            if (label != null)
            {
                label.Text = "";
            }
        }

        private void OnLabelLostFocus (object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox targetLabel = (TextBox)sender;
            TextBlock label = m_staticLabels[targetLabel];

            if (label != null)
            {
                if (targetLabel.Text.Length < 1)
                {
                    label.Text = m_labelValues[targetLabel];
                    targetLabel.Text = "";
                }
            }
        }

        private void OnLabelChanged (object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            TextBox targetLabel = (TextBox)sender;
            string labelText = targetLabel.Text;
            if (labelText.Contains(KreyosUtils.EMPTY_DEFAULT))
            {
                targetLabel.Text = labelText.Trim();
            }
        }

        private void OnRadioChecked (object sender, System.Windows.RoutedEventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
        }

        private void OnChangePhoto (object sender, System.Windows.Input.GestureEventArgs e)
        {
            // load facebook image here
        }
    }
}