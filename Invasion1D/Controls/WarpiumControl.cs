using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Controls
{
	internal class WarpiumControl : Frame
	{
		private const int Proportion = 2;

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			var proportionalWidth = height * Proportion;
			WidthRequest = proportionalWidth;
			CornerRadius = (float)height / 2;
		}
	}
}