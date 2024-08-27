using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invasion1D.Controls
{
    internal class PopUpMenu : Frame
    {
        Label title;
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(PopUpMenu), default(string));
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                if (title != null)
                {
                    title.Text = value;
                }
            }
        }

        public static readonly BindableProperty ForegroundProperty = BindableProperty.Create(nameof(Foreground), typeof(Color), typeof(PopUpMenu), default(Color));
        public Color Foreground
        {
            get => (Color)GetValue(ForegroundProperty);
            set
            {
                SetValue(ForegroundProperty, value);
                if (title != null)
                {
                    title.TextColor = value;
                }
            }
        }

        new public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof(VerticalStackLayout), typeof(PopUpMenu), default(VerticalStackLayout));
        new public VerticalStackLayout Content
        {
            get => (VerticalStackLayout)GetValue(ContentProperty);
            set
            {
                SetValue(ContentProperty, value);
                base.Content = value;
            }
        }

        new public static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(PopUpMenu), default(Color));
        new public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set
            {
                SetValue(BackgroundColorProperty, value);
                base.BackgroundColor = value;
            }
        }



        new public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(PopUpMenu), default(Color));
        new public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set
            {
                SetValue(BorderColorProperty, value);
                base.BorderColor = value;
            }
        }

        public PopUpMenu()
        {
            title = new() { TextColor = Foreground };
            title.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

            base.BackgroundColor = BackgroundColor;
            base.BorderColor = BorderColor;

            Content = [title];
            base.Content = Content;
       }
    }
}
