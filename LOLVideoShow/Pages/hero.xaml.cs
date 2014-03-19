using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using LOLVideoShow.Data;
using LOLVideoShow.Class;
using System.Windows.Media.Imaging;

namespace LOLVideoShow.Pages
{
    public partial class hero : PhoneApplicationPage
    {
        private WebData _web = new WebData();
        private ObservableCollection<HeroInfo> Heros = new ObservableCollection<HeroInfo>();
        public hero()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Gridbox.Children.Count == 0)
            {
                Heros = DataCache.GetCache<ObservableCollection<HeroInfo>>("Heros", !App.isNetworkEnabled);
                if (Heros != null)
                {
                    GridHeros();
                }
                else
                {
                    _web.Load(App.HOST + "/api/get_list/hero?t=" + new Random().Next(100000), heroLoadedCallback);    //随机数防止URL缓存
                }
            }
        }

        private void heroLoadedCallback(object sender, OpenReadCompletedEventArgs e)
        {
            Heros = _web.JsonToObject<ObservableCollection<HeroInfo>>(e);
            if (Heros != null)
            {
                DataCache.SaveCache("Heros", Heros);
                GridHeros();
            }
        }

        private void GridHeros()
        {
            if (Heros == null || Heros.Count == 0) return;
            Gridbox.Children.Clear();
            Gridbox.RowDefinitions.Clear();
            int col = 0;
            int row = 0;
            foreach (HeroInfo hero in Heros)
            {
                StackPanel stack = new StackPanel();
                Image img = new Image();
                TextBlock text = new TextBlock();
                stack.Children.Add(img);
                stack.Children.Add(text);

                stack.Tag = hero;
                stack.Margin = new Thickness(5);
                stack.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(stack_Tap);

                img.Source = new BitmapImage(new Uri("/LOLVideoShow;component/" + hero.pic, UriKind.Relative));
                img.Height = 100;
                img.Width = 100;
                img.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                img.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                text.Text = hero.name;
                text.FontSize = 14;
                text.TextWrapping = TextWrapping.Wrap;
                text.TextAlignment = TextAlignment.Center;
                text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                Grid.SetColumn(stack, col);
                Grid.SetRow(stack, row);
                Gridbox.Children.Add(stack);
                col++;
                if (col == 4)
                {
                    col = 0;
                    row++;
                    addNewRow(Gridbox);
                }
            }
        }

        private void stack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                StackPanel s = sender as StackPanel;
                HeroInfo hero = s.Tag as HeroInfo;
                NavigationService.Navigate(new Uri("/VideoList/hero/" + hero.id, UriKind.Relative));
            }
            catch
            {
                return;
            }
        }

        private void addNewRow(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(150, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row);
        }
    }
}