namespace Invasion1D.Controls;

public partial class InvertedProgressBar : Frame
{
    readonly BoxView progressBarControl;

    public static readonly BindableProperty ProgressBarColorProperty = BindableProperty.Create(nameof(ProgressBarColor), typeof(Color), typeof(InvertedProgressBar), default(Color));
    public Color ProgressBarColor
    {
        get => (Color)GetValue(ProgressBarColorProperty);
        set
        {
            SetValue(ProgressBarColorProperty, value);
            progressBarControl.BackgroundColor = value;
        }
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(nameof(Progress), typeof(float), typeof(InvertedProgressBar), default(float));
    public float Progress
    {
        get => (float)GetValue(ProgressProperty);
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            else if (value > 1)
            {
                value = 1;
            }

            SetValue(ProgressProperty, value);

            if (Width == -1)
            {
                throw new Exception();
            }

            ProgressBarWidthRequest = Width * value;
        }
    }

    public static readonly BindableProperty ProgressBarWidthRequestProperty = BindableProperty.Create(nameof(ProgressBarWidthRequest), typeof(double), typeof(InvertedProgressBar), default(double));
    public double ProgressBarWidthRequest
    {
        get => (double)GetValue(ProgressBarWidthRequestProperty);
        private set => SetValue(ProgressBarWidthRequestProperty, value);
    }

    public InvertedProgressBar()
    {
        progressBarControl = new BoxView();
        progressBarControl.SetBinding(BackgroundColorProperty, new Binding(nameof(ProgressBarColor), source: this));
        progressBarControl.SetBinding(WidthRequestProperty, new Binding(nameof(ProgressBarWidthRequest), source: this));

        Content = progressBarControl;
    }
}