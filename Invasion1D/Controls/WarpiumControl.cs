namespace Invasion1D.Controls;

internal class WarpiumControl : Frame
{
	private const float Proportion = 0.5f;

	protected override void OnSizeAllocated(double width, double height)
	{
		base.OnSizeAllocated(width, height);
		double proportionalWidth = height * Proportion;
		WidthRequest = proportionalWidth;
		CornerRadius = (float)height / 4;
	}
}