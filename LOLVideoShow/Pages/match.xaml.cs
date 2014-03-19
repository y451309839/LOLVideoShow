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

namespace LOLVideoShow.Pages
{
    public partial class match : PhoneApplicationPage
    {
        private WebData _web = new WebData();
        private ObservableCollection<MatchInfo> Matchs;
        public match()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(match_Loaded);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Matchs == null)
            {
                Matchs = DataCache.GetCache<ObservableCollection<MatchInfo>>("Matchs", !App.isNetworkEnabled);
                if (Matchs == null)
                {
                    _web.Load(App.HOST + "/api/get_list/match?t=" + new Random().Next(100000),
                        matchLoadedCallback);    //随机数防止URL缓存
                }
                else
                {
                    MatchList.DataContext = Matchs;
                }
            }
        }

        public void matchLoadedCallback(object sender, OpenReadCompletedEventArgs e)
        {
            Matchs = _web.JsonToObject<ObservableCollection<MatchInfo>>(e);
            if (Matchs != null)
            {
                DataCache.SaveCache("Matchs", Matchs);
                MatchList.DataContext = Matchs;
            }
        }
    }
}