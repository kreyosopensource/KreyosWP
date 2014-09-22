using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;  
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Kreyos.Resources;

using Kreyos.Classes.Managers;
using Kreyos.Classes.Components;
using Kreyos.Classes.Utils;
using System.Windows.Markup;

namespace Kreyos
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Properties
        private DispatcherTimer m_switchTimer;

        /*
        private LongListSelector[] m_sports = new LongListSelector[3];
        private int m_gridIndex = 0;
        private List<SportsCellData> m_cellData = new List<SportsCellData>();
        private List<SportsCellData> m_dummyData = new List<SportsCellData>();
        private Image m_buttonAdd;
        //*/

        // Constructor
        public MainPage ()
        {
            InitializeComponent();

            // start the timer
            this.StartTimer();

            // Test create DataTemplate Programmatically
            //this.UpdateGrid();

            /*
            m_cellData.Add(new SportsCellData("steps", "", "0"));
            m_cellData.Add(new SportsCellData("distance", "meter", "0"));
            m_cellData.Add(new SportsCellData("altitude", "", "0"));
            m_cellData.Add(new SportsCellData("calories", "", "0"));

            m_dummyData.Add(new SportsCellData("steps", "", "0"));
            m_dummyData.Add(new SportsCellData("distance", "meter", "0"));
            m_dummyData.Add(new SportsCellData("altitude", "", "0"));
            m_dummyData.Add(new SportsCellData("calories", "", "0"));

            m_sports[(int)ESports.EGrid1x2] = this.SportsGrid1x2;
            m_sports[(int)ESports.EGrid3x1] = this.SportsGrid3x1;
            m_sports[(int)ESports.EGrid2x2] = this.SportsGrid2x2;

            m_sports[(int)ESports.EGrid1x2].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid3x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid2x2].Visibility = Visibility.Collapsed;

            m_buttonAdd = this.img_add;

            m_gridIndex = (int)ESports.EGrid2x2;
            this.ChangeGridCellSize((int)ESports.EGrid2x2, m_cellData);
            //*/
        }

        /*
        public void ChangeGridCellSize ( int p_count, List<SportsCellData> p_data )
        {
            m_sports[(int)ESports.EGrid1x2].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid3x1].Visibility = Visibility.Collapsed;
            m_sports[(int)ESports.EGrid2x2].Visibility = Visibility.Collapsed;

            switch( p_count )
            {
                case (int)ESports.EGrid1x2:
                {
                    SportsGrid1x2.GridCellSize = new Size(SportsGrid1x2.Width/2.0f, SportsGrid1x2.Height);
                    SportsGrid1x2.ItemsSource = p_data;
                    SportsGrid1x2.Visibility = Visibility.Visible;
                    m_buttonAdd.Visibility = Visibility.Visible;
                }
                break;

                case (int)ESports.EGrid3x1:
                {
                    SportsGrid3x1.GridCellSize = new Size(SportsGrid3x1.Width, SportsGrid3x1.Height/3.0f);
                    SportsGrid3x1.ItemsSource = p_data;
                    SportsGrid3x1.Visibility = Visibility.Visible;
                    m_buttonAdd.Visibility = Visibility.Visible;
                }
                break;

                case (int)ESports.EGrid2x2:
                {
                    SportsGrid2x2.GridCellSize = new Size(SportsGrid2x2.Width/2.0f, SportsGrid2x2.Height/2.0f);
                    SportsGrid2x2.ItemsSource = p_data;
                    SportsGrid2x2.Visibility = Visibility.Visible;
                    m_buttonAdd.Visibility = Visibility.Collapsed;
                }
                break;
            }
        }

        // Trash & Refresh callbacks
        private void onDelete(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            m_dummyData.RemoveAt(0);
            m_gridIndex--;
            this.ChangeGridCellSize(m_gridIndex, m_dummyData);
        }

        private void onRefresh(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
        }

        private void OnAdd(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // TODO: Add event handler implementation here.
            foreach (SportsCellData cell in m_cellData)
            {
                if (!m_dummyData.Contains(cell))
                {
                    m_dummyData.Add(cell);
                    m_gridIndex++;
                    this.ChangeGridCellSize(m_gridIndex, m_dummyData);
                    break;
                }
            }
        }
        //*/

        private void StartTimer()
        {
            // creating timer instance
            m_switchTimer = new DispatcherTimer();
            // timer interval specified as 0.1 second
            m_switchTimer.Interval = TimeSpan.FromSeconds(0.1f);
            // Sub-routine OnTimerTick will be called at every 1 second
            m_switchTimer.Tick += SwitchScreen;
            // starting the timer
            m_switchTimer.Start();
        }

        private void SwitchScreen ( Object p_sender, EventArgs p_args )
        {
            // stop the timer
            m_switchTimer.Stop();
            // remove callback reference
            m_switchTimer.Tick -= SwitchScreen;
            // delete timer
            m_switchTimer = null;

            // Switch screen
            ScreenManager.Instance.Switch( EScreens.ES_Login );
        }

    }

    public class TestData
    {
        public TestData (string p_title, string p_value)
        {
            this.Title = p_title;
            this.Value = p_value;
        }

        public string Title { get; set; }
        public string Value { get; set; }
    }
}