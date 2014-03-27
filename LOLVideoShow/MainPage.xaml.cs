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
using System.Windows.Media.Imaging;
using LOLVideoShow.Class;
using System.Threading;
using LOLVideoShow.Data;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;
using Microsoft.Phone.Tasks;

namespace LOLVideoShow
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Toast toast;
        private WebData _web;
        private ObservableCollection<VideoInfo> newTui;
        private ObservableCollection<VideoInfo> history;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();
            MainPanorama.Background = App.Getbackground;
            MainPanorama.Background.Opacity = 0.5;
            loadAdMSN();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            toast = new Toast();
            _web = new WebData();
            _web.SetBusyEvent(onWebBusy, unWebBusy);
            if (newTui == null)
            {
                newTui = DataCache.GetCache<ObservableCollection<VideoInfo>>("newTui", !App.isNetworkEnabled);
                if (newTui == null)
                    _web.Load(App.HOST + "/api", LoadedIndexDataCallback);
                else
                    GridnewTuis();
            }
            history = CommonTools.GetPlayerHistory();
            if (history != null) PanelHistory();
        }

        private void onWebBusy(object sender, EventArgs e)
        {
            progressMain.Opacity = 1;
        }

        private void unWebBusy(object sender, EventArgs e)
        {
            progressMain.Opacity = 0;
        }

        private void loadAdMSN()
        {
            
        }

        private void LoadedIndexDataCallback(object sender, OpenReadCompletedEventArgs e)
        {
            newTui = _web.JsonToObject<ObservableCollection<VideoInfo>>(e);
            if (newTui != null)
            {
                DataCache.SaveCache("newTui", newTui);
                GridnewTuis();
            }
        }

        private void listNewTui_OnScrollDown(object sender, RoutedEventArgs e)
        {
            _web.Load(App.HOST + "/api?t=" + new Random().Next(10000), LoadedIndexDataCallback);
        }

        private void ListHistory_OnScrollDown(object sender, RoutedEventArgs e)
        {
            if (history != null) history.Clear();
            CommonTools.ClearPlayerHistory();
            ListHistoryItems.Children.Clear();
            toast.show("历史记录已清空");
        }

        private void GridnewTuis()
        {
            gridTui.Children.Clear();
            gridTui.RowDefinitions.Clear();
            int num = 0;
            int row_index = 0;
            Boolean _Left = false;
            Boolean _Right = false;
            foreach (var item in newTui)
            {
                int x = num % 2;
                Boolean _Big = item.title.Length > 18;

                ///控件创建
                Button mButton = new Button();
                mButton.Template = (ControlTemplate)Application.Current.Resources["TileBtnTemlate"];
                mButton.Content = item.title;
                mButton.FontSize = 18;
                mButton.Tag = item;
                mButton.Click += new RoutedEventHandler(mButton_Click);
                TiltEffect.SetIsTiltEnabled(mButton, true);

                if (_Left && _Right)
                {
                    _Left = false;
                    _Right = false;
                    row_index++;
                    addNewRow(gridTui);
                }
                else if (_Right && x == 1)
                {
                    _Right = false;
                    num++;
                    row_index++;
                    x = num % 2;
                    addNewRow(gridTui);
                }
                else if (_Left && x == 0)
                {
                    _Left = false;
                    num++;
                    x = num % 2;
                    addNewRow(gridTui);
                }
                if (x == 0)
                {
                    addNewRow(gridTui);
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
                gridTui.Children.Add(mButton);
            }
        }


        private void addNewRow(Grid grid)
        {
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(80, GridUnitType.Pixel);
            grid.RowDefinitions.Add(row);
        }

        private void PanelHistory()
        {
            if (ListHistoryItems.Children.Count > 0) ListHistoryItems.Children.Clear();
            for (var id = history.Count - 1; id >= 0; id--)
            {
                VideoInfo item = history[id];
                ///控件创建
                Button mButton = new Button();
                mButton.Template = (ControlTemplate)Application.Current.Resources["TileBtnTemlate"];
                mButton.Content = item.title;
                mButton.FontSize = 18;
                mButton.MinHeight = 80;
                mButton.Tag = item;
                mButton.Click += new RoutedEventHandler(mButton_Click);
                TiltEffect.SetIsTiltEnabled(mButton, true);

                ListHistoryItems.Children.Add(mButton);
            }
        }

        private void mButton_Click(object sender, RoutedEventArgs e)
        {
            Button m = sender as Button;
            VideoInfo v = m.Tag as VideoInfo;
            MessageBoxResult msgRst = MessageBox.Show(v.title, "立即播放", MessageBoxButton.OKCancel);
            if (msgRst == MessageBoxResult.OK)
            {
                history = CommonTools.SavePlayerHistory(v);
                PanelHistory();
                NavigationService.Navigate(new Uri("/Player/" + v.id, UriKind.Relative));
            }
        }

        private void getYoukuMoblieMp4Callback(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader(e.Result);
                string url = sr.ReadToEnd();
                CommonTools.playVideo(url);
            }
            catch
            {
                toast.show("网络错误，暂时无法播放！");
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (toast.is_showing == false)
            {
                toast.show("再按一次离开");
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
                marketplaceReviewTask.Show();
            }
            catch
            {
                toast.show("抱歉，您暂时无法参与评分");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgRst = MessageBox.Show("您确定要清理掉所有缓存数据？", "清理缓存", MessageBoxButton.OKCancel);
            if (msgRst == MessageBoxResult.OK)
            {
                IsolatedStorageHelper.ClearObjects();
                toast.show("缓存清理完成");
            }
        }
    }
}