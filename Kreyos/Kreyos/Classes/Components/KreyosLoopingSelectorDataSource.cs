using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Phone.Controls.Primitives;

namespace Kreyos.Classes.Components
{
    /// <summary>
    /// Custom single picker
    /// </summary>
    public abstract class KreyosLoopingSelectorDataSource : ILoopingSelectorDataSource
    {
        private object selectedItem;

        public abstract object GetNext (object relativeTo);

        public abstract object GetPrevious (object relativeTo);

        public object SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                // this will use the Equals method if it is overridden for the data source item class
                if (!object.Equals(this.selectedItem, value))
                {
                    // save the previously selected item so that we can use it 
                    // to construct the event arguments for the SelectionChanged event
                    object previousSelectedItem = this.selectedItem;
                    this.selectedItem = value;
                    // fire the SelectionChanged event
                    this.OnSelectionChanged(previousSelectedItem, this.selectedItem);
                }
            }
        }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        protected virtual void OnSelectionChanged(object oldSelectedItem, object newSelectedItem)
        {
            EventHandler<SelectionChangedEventArgs> handler = this.SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs(new object[] { oldSelectedItem }, new object[] { newSelectedItem }));
            }
        }
    }
}
