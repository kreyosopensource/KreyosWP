using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Kreyos.Classes.Screens
{
    public partial class TestView : PhoneApplicationPage
    {
        private double[] rangeValues = new double[5] { 4000, 6000, 8000, 10000, 12000 };

        public TestView()
        {
            InitializeComponent();

            /*
            List<string> chufuls = new List<string>();
            chufuls.Add("a");
            chufuls.Add("b");
            chufuls.Add("c");
            chufuls.Add("d");

            //SportsGrid.i
            SportsGrid.ItemsSource = chufuls;
            //*/

            slider_chufs.Value = 6000;
        }

        private void OnSliderUpdate(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
        	// TODO: Add event handler implementation here.
            //slider_chufs.Value = Math.Round(e.NewValue);
            //double dNewValue = e.NewValue;
            //int iNewValue = (int)Math.Round(dNewValue);
            //slider_chufs.Value = Math.Round(dNewValue);
            //txt_chufs.Text = String.Format("slider:{0}", e.NewValue.ToString());
            /*
            double newValue = Math.Round(e.NewValue);
            if (slider_chufs.Value != newValue)
            {
                slider_chufs.Value = newValue;
            }
            //*/

        }

        private void OnSliderCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            double currentValue = slider_chufs.Value;

            for (int i = 1; i < rangeValues.Length; i++)
            {
                if (currentValue >= rangeValues[i])
                {
                    continue;
                }

                double smallRange = Math.Abs(currentValue - rangeValues[(i - 1)]);
                double bigRange = Math.Abs(currentValue - rangeValues[i]);

                if (smallRange <= bigRange)
                {
                    currentValue = rangeValues[(i - 1)];
                }
                else
                {
                    currentValue = rangeValues[i];
                }

                slider_chufs.Value = currentValue;
                break;
            }
        }

        /*
        private void onDelete(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }

        private void onRefresh(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
        }
        //*/
    }
}