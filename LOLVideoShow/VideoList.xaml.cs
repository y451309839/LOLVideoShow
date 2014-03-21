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
using Microsoft.Phone.Tasks;
using System.IO;
using LOLVideoShow.Class;
using System.Collections.ObjectModel;

namespace LOLVideoShow
{
    public partial class VideoList : PhoneApplicationPage
    {
        private string mod;
        private int id;
        private WebData _web;
        private ChennelInfo mChennel;
        private ObservableCollection<VideoInfo> NewVideoList;
        private ObservableCollection<VideoInfo> HotVideoList;
        private int NewVideoList_pageIndex = 1;
        private int HotVideoList_pageIndex = 1;

        public VideoList()
        {
            InitializeComponent();
            _web = new WebData();
            _web.SetBusyEvent(onWebBusy, unWebBusy);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.Count != 0)
            {
                this.mod = NavigationContext.QueryString["mod"];
                this.id = Convert.ToInt32(NavigationContext.QueryString["id"]);

                if (mChennel == null)
                {
                    mChennel = DataCache.GetCache<ChennelInfo>("Chennel_" + this.mod + "_" + this.id, !App.isNetworkEnabled);
                    if (mChennel == null)
                    {
                        _web.Load(App.HOST + "/api/get_info/" + this.mod + "/" + this.id, loadedInfoCallback);
                    }
                    else
                    {
                        listPivot.Title = mChennel.title;
                    }
                }

                if (NewVideoList == null)
                {
                    NewVideoList = DataCache.GetCache<ObservableCollection<VideoInfo>>("NewVideoList_" + this.mod + "_" + this.id, !App.isNetworkEnabled);
                    if (NewVideoList == null)
                    {
                        _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/news", loadedNewsCallback);
                    }
                    else
                    {
                        News.DataContext = NewVideoList;
                    }
                }

                if (HotVideoList == null)
                {
                    HotVideoList = DataCache.GetCache<ObservableCollection<VideoInfo>>("HotVideoList_" + this.mod + "_" + this.id, !App.isNetworkEnabled);
                    if (HotVideoList == null)
                    {
                        _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/hots", loadedHotsCallback);
                    }
                    else
                    {
                        Hots.DataContext = HotVideoList;
                    }
                }
            }
        }

        private void onWebBusy(object sender, EventArgs e)
        {
            progressMain.Opacity = 1;
        }

        private void unWebBusy(object sender, EventArgs e)
        {
            progressMain.Opacity = 0;
        }

        private void loadedInfoCallback(object sender, OpenReadCompletedEventArgs e)
        {
            switch (Chennel.getChennelType(this.mod))
            {
                case ChennelType.Hero:
                    HeroInfo hero = _web.JsonToObject<HeroInfo>(e);
                    mChennel = Chennel.From(hero);
                    break;
                case ChennelType.Jieshuo:
                    JieshuoInfo jieshuo = _web.JsonToObject<JieshuoInfo>(e);
                    mChennel = Chennel.From(jieshuo);
                    break;
                case ChennelType.Match:
                    MatchInfo match = _web.JsonToObject<MatchInfo>(e);
                    mChennel = Chennel.From(match);
                    break;
            }
            if (mChennel != null)
            {
                DataCache.SaveCache("Chennel_" + this.mod + "_" + this.id, mChennel);
                listPivot.Title = mChennel.title;
            }
        }

        private void News_OnScrollDown(object sender, RoutedEventArgs e)
        {
            NewVideoList_pageIndex = 1;
            _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/news/" + NewVideoList_pageIndex + "?t=" + new Random().Next(10000), loadedNewsCallback);
        }

        private void News_OnScrollUp(object sender, RoutedEventArgs e)
        {
            if (NewVideoList_pageIndex > 0)
            {
                NewVideoList_pageIndex++;
                _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/news/" + NewVideoList_pageIndex + "?t=" + new Random().Next(10000), loadedNewsCallback);
            }
        }

        private void loadedNewsCallback(object sender, OpenReadCompletedEventArgs e)
        {
            ObservableCollection<VideoInfo> temp = _web.JsonToObject<ObservableCollection<VideoInfo>>(e);
            if (temp == null)
            {
                NewVideoList_pageIndex = 0;
            }
            else
            {
                temp = createTimeLine(temp);
                if (NewVideoList_pageIndex > 1 && NewVideoList != null)
                {
                    foreach (var item in temp)
                    {
                        NewVideoList.Add(item);
                    }
                }
                else if (NewVideoList_pageIndex == 1)
                {
                    NewVideoList = temp;
                    DataCache.SaveCache("NewVideoList_" + this.mod + "_" + this.id, NewVideoList);
                }
                News.DataContext = NewVideoList;
            }
        }

        private void Hots_OnScrollDown(object sender, RoutedEventArgs e)
        {
            HotVideoList_pageIndex = 1;
            _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/hots/" + HotVideoList_pageIndex + "?t=" + new Random().Next(10000), loadedHotsCallback);
        }

        private void Hots_OnScrollUp(object sender, RoutedEventArgs e)
        {
            if (HotVideoList_pageIndex > 0)
            {
                HotVideoList_pageIndex++;
                _web.Load(App.HOST + "/api/video_list/" + this.mod + "/" + this.id + "/hots/" + HotVideoList_pageIndex + "?t=" + new Random().Next(10000), loadedHotsCallback);
            }
        }

        private void loadedHotsCallback(object sender, OpenReadCompletedEventArgs e)
        {
            ObservableCollection<VideoInfo> temp = _web.JsonToObject<ObservableCollection<VideoInfo>>(e);
            if (temp == null)
            {
                HotVideoList_pageIndex = 0;
            }
            else
            {
                temp = createTimeLine(temp);
                if (HotVideoList_pageIndex > 1 && HotVideoList != null)
                {
                    foreach (var item in temp)
                    {
                        HotVideoList.Add(item);
                    }
                }
                else if (HotVideoList_pageIndex == 1)
                {
                    HotVideoList = temp;
                    DataCache.SaveCache("HotVideoList_" + this.mod + "_" + this.id, HotVideoList);
                }
                Hots.DataContext = HotVideoList;
            }
        }

        private ObservableCollection<VideoInfo> createTimeLine(ObservableCollection<VideoInfo> l)
        {
            foreach (var i in l)
            {
                DateTime t = CommonTools.GetTime(i.dateline.ToString());
                i.time = t.ToString();
            }
            return l;
        }

        private void VideoBtn_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton m = sender as HyperlinkButton;
            VideoInfo v = m.Tag as VideoInfo;
            MessageBoxResult msgRst = MessageBox.Show(v.title, "立即播放", MessageBoxButton.OKCancel);
            if (msgRst == MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Uri("/Player/" + v.id, UriKind.Relative));
            }
        }

    }
}