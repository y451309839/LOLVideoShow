using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Threading;
using System.Windows.Threading;

namespace LOLVideoShow.Class
{
    public class Toast
    {
        private PhoneApplicationFrame _frame;
        private PhoneApplicationPage _page;
        private Grid _grid;
        private Border _toast;
        private TextBlock _block;
        private int show_time;
        public Boolean is_showing = false;

        public Toast()
        {
            _frame = Application.Current.RootVisual as PhoneApplicationFrame;
            _page = _frame.Content as PhoneApplicationPage;
            _grid = VisualTreeHelper.GetChild(_page, 0) as Grid;

            _toast = new Border();
            _toast.Padding = new Thickness(30, 20, 30, 20);
            _toast.Margin = new Thickness(0, 0, 0, 100);
            _toast.BorderThickness = new Thickness(1);
            _toast.CornerRadius = new CornerRadius(5);
            Color bd = Colors.White;
            bd.A = 40;
            _toast.BorderBrush = new SolidColorBrush(bd);
            Color bg = Colors.Black;
            bg.A = 240;
            _toast.Background = new SolidColorBrush(bg);
            _toast.HorizontalAlignment = HorizontalAlignment.Center;
            _toast.VerticalAlignment = VerticalAlignment.Bottom;

            _block = new TextBlock();
            _block.FontSize = 18;
            _block.Foreground = new SolidColorBrush(Colors.White);
            _block.TextAlignment = TextAlignment.Center;
            _block.HorizontalAlignment = HorizontalAlignment.Center;
            _block.VerticalAlignment = VerticalAlignment.Center;

            _toast.Child = _block;
        }

        public void show(string content, int time = 3)
        {
            show_time = time;
            _block.Text = content;

            if (_grid.Children.Count > 0)
            {
                Grid.SetRowSpan(_toast, _grid.Children.Count);
            }
            _grid.Children.Add(_toast);
            is_showing = true;
            Thread t = new Thread(new ThreadStart(hide));
            t.Start();
        }

        private void hide()
        {
            if (_toast != null)
            {
                Thread.Sleep(show_time * 1000);
                _page.Dispatcher.BeginInvoke(() =>
                {
                    _grid.Children.Remove(_toast);
                });
                is_showing = false;
            }
        }
    }

}
