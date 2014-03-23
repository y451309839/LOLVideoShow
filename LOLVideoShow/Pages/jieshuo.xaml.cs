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
using LOLVideoShow.Data;
using System.Collections.ObjectModel;
using LOLVideoShow.Class;

namespace LOLVideoShow.Pages
{
    public partial class jieshuo : PhoneApplicationPage
    {
        private WebData _web = new WebData();
        private ObservableCollection<JieshuoInfo> Jieshuos = new ObservableCollection<JieshuoInfo>();

        public jieshuo()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(jieshuo_Loaded);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Gridbox.Children.Count == 0)
            {
                Jieshuos = DataCache.GetCache<ObservableCollection<JieshuoInfo>>("Jieshuos", !App.isNetworkEnabled);
                if (Jieshuos != null)
                {
                    GridJieshuos();
                }
                else
                {
                    _web.Load(App.HOST + "/api/get_list/jieshuo?t=" + new Random().Next(100000), jieshuoLoadedCallback);    //随机数防止URL缓存
                }
            }
        }

        private void jieshuoLoadedCallback(object sender, OpenReadCompletedEventArgs e)
        {
            Jieshuos = _web.JsonToObject<ObservableCollection<JieshuoInfo>>(e);
            if (Jieshuos != null)
            {
                DataCache.SaveCache("Jieshuos", Jieshuos);
                GridJieshuos();
            }
        }

        private void GridJieshuos()
        {
            Gridbox.Children.Clear();
            Gridbox.RowDefinitions.Clear();
            int num = 0;
            int row_index = 0;
            Boolean _Left = false;
            Boolean _Right = false;
            foreach (var item in Jieshuos)
            {
                int x = num % 2;
                Boolean _Big = item.name.Length > 6;

                ///控件创建
                Button mButton = new Button();
                mButton.Template = (ControlTemplate)Application.Current.Resources["TileBtnTemlate"];
                mButton.Content = item.name;
                mButton.FontSize = 24;
                mButton.Tag = item;
                mButton.Click += new RoutedEventHandler(mButton_Click);
                TiltEffect.SetIsTiltEnabled(mButton, true);

                if (_Left && _Right)
                {
                    _Left = false;
                    _Right = false;
                    row_index++;
                    addNewRow(Gridbox);
                }
                else if (_Right && x == 1)
                {
                    _Right = false;
                    num++;
                    row_index++;
                    x = num % 2;
                    addNewRow(Gridbox);
                }
                else if (_Left && x == 0)
                {
                    _Left = false;
                    num++;
                    x = num % 2;
                    addNewRow(Gridbox);
                }
                else if (x == 0)
                {
                    addNewRow(Gridbox);
                }
                Grid.SetColumn(mButton, x);
                Grid.SetRow(mButton, row_index);
                if (_Big)
                {
                    Grid.SetRowSpan(mButton, 2);
                    if (x == 0)
                        _Left = true;
                    else
                        _Right = true;
                }

                num++;
                if (x == 1) row_index++;
                Gridbox.Children.Add(mButton);
            }
            //addNewRow(Gridbox);
        }

        private void addNewRow(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(80, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row);
        }

        private void mButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button b = sender as Button;
                JieshuoInfo jieshuo = b.Tag as JieshuoInfo;
                NavigationService.Navigate(new Uri("/VideoList/jieshuo/" + jieshuo.id, UriKind.Relative));
            }
            catch
            {
                return;
            }
        }
    }
}