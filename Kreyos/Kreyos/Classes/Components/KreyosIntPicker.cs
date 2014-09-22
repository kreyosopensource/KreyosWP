using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Components
{
    public class KreyosIntPicker : KreyosLoopingSelectorDataSource
    {
        private int minValue;
		private int maxValue;
		private int increment;

        public KreyosIntPicker ()
		{
			this.MaxValue = 10;
			this.MinValue = 0;
			this.Increment = 1;
			this.SelectedItem = 0;
		}

		public int MinValue
		{
			get { return this.minValue; }
			set
			{
				if (value >= this.MaxValue)
				{
					throw new ArgumentOutOfRangeException("MinValue", "MinValue cannot be equal or greater than MaxValue");
				}
				this.minValue = value;
			}
		}

		public int MaxValue
		{
			get { return this.maxValue; }
			set
			{
				if (value <= this.MinValue)
				{
					throw new ArgumentOutOfRangeException("MaxValue", "MaxValue cannot be equal or lower than MinValue");
				}
				this.maxValue = value;
			}
		}

		public int Increment
		{
			get { return this.increment; }
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("Increment", "Increment cannot be less than or equal to zero");
				}
				this.increment = value;
			}
		}

		public override object GetNext(object relativeTo)
		{
			int nextValue = (int)relativeTo + this.Increment;
			if (nextValue > this.MaxValue)
			{
				nextValue = this.MinValue;
			}
			return nextValue;
		}

		public override object GetPrevious(object relativeTo)
		{
			int prevValue = (int)relativeTo - this.Increment;
			if (prevValue < this.MinValue)
			{
				prevValue = this.MaxValue;
			}
			return prevValue;
		}
    }
}
