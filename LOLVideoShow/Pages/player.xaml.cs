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
using System.Windows.Data;
using System.Threading;
using System.Windows.Media.Imaging;
using LOLVideoShow.Data;
using System.IO;
using LOLVideoShow.Class;
using Microsoft.Phone.Tasks;

namespace LOLVideoShow.Pages
{
    public partial class player : PhoneApplicationPage
    {
        private int id;
        private WebData _web;
        private VideoInfo _video;
        private Thread ThreadPlayerProgress;
        private double PlayProgressIndex = 0;
        private Boolean showBar = true;
        private Boolean isSetVideoProgress = true;
        private Boolean isPlaying = true;

        public player()
        {
            InitializeComponent();
            _web = new WebData();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.id = Convert.ToInt32(NavigationContext.QueryString["id"]);
            _web.Load(App.HOST + "/api/get_info/video/" + this.id, loadedInfoCallback);
            _web.Load(App.HOST + "/api/go_youku/" + this.id, getYoukuMoblieMp4Callback);
        }

        private void getYoukuMoblieMp4Callback(object sneder, OpenReadCompletedEventArgs e)
        {
            StreamReader sr = new StreamReader(e.Result);
            string url = sr.ReadToEnd();
            if (url != "")
            {
                mediaPlayer.Source  = new Uri(url, UriKind.Absolute);
            }
            else
            {
                MessageBoxResult msgRst = MessageBox.Show("暂时还没有这个视频的播放资源！是否尝试打开网页观看？", "视频连接失败", MessageBoxButton.OKCancel);
                if (msgRst == MessageBoxResult.OK)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(_video.url, UriKind.Absolute);
                    webBrowserTask.Show();
                    this.NavigationService.GoBack();
                }
            }
        }

        private void loadedInfoCallback(object sender, OpenReadCompletedEventArgs e)
        {
            _video = _web.JsonToObject<VideoInfo>(e);
            if (_video != null)
            {
                MediaTitle.Text = _video.title;
                CommonTools.SavePlayerHistory(_video);
            }
        }

        private void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaTime.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");

            if (mediaPlayer.CanSeek)
            {
                TimeSpan _progress = IsolatedStorageHelper.GetObject<TimeSpan>("PlayVideo_Progress_" + this.id);
                if (_progress != default(TimeSpan) && _progress != mediaPlayer.NaturalDuration.TimeSpan)
                {
                    MessageBoxResult msgRst = MessageBox.Show("上次播放到 " + _progress.ToString(@"hh\:mm\:ss"), "是否继续上次播放？", MessageBoxButton.OKCancel);
                    if (msgRst == MessageBoxResult.OK)
                    {
                        mediaPlayer.Position = _progress;
                    }
                }
            }
            ThreadPlayerProgress = new Thread(new ThreadStart(ThreadPlayerProgressSlider));
            ThreadPlayerProgress.Start();
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isSetVideoProgress)
            {
                Slider mSlider = sender as Slider;
                double rate = mSlider.Value / mSlider.Maximum;
                long ticks = Convert.ToInt64(rate * mediaPlayer.NaturalDuration.TimeSpan.Ticks);
                mediaPlayer.Position = new TimeSpan(ticks);
            }
        }

        private void ThreadPlayerProgressSlider()
        {
            while (true)
            {
                Thread.Sleep(1000);
                Dispatcher.BeginInvoke(setPlayerProgressSlider);
            }
        }
        /// <summary>
        /// 每秒更新播放进度条，且保存进度状态
        /// </summary>
        private void setPlayerProgressSlider()
        {
            if (mediaPlayer.Position.Ticks > 0 && playerBar.Visibility == System.Windows.Visibility.Visible)
            {
                isSetVideoProgress = false;
                double progress = ((double)mediaPlayer.Position.Ticks / (double)mediaPlayer.NaturalDuration.TimeSpan.Ticks) * 100;
                PlayProgressIndex = progress;
                MediaSlider.Value = progress;
                MediaIndex.Text = mediaPlayer.Position.ToString(@"hh\:mm\:ss");
                isSetVideoProgress = true;
            }
            if (mediaPlayer.Position < mediaPlayer.NaturalDuration.TimeSpan)
                IsolatedStorageHelper.SaveObject("PlayVideo_Progress_" + this.id, mediaPlayer.Position);
            else
                IsolatedStorageHelper.DeleteObject("PlayVideo_Progress_" + this.id);
        }

        private void mediaPlayer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (showBar == false)
            {
                playerBar.Visibility = System.Windows.Visibility.Visible;
                showBar = true;
            }
            else
            {
                playerBar.Visibility = System.Windows.Visibility.Collapsed;
                showBar = false;
            }
        }

        /// <summary>
        /// 实现PLAY按钮的样式且换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_btn_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            Image img = sender as Image;
            if (isPlaying)
            {
                img.Source = new BitmapImage(new Uri("/LOLVideoShow;component/images/ui/media_pause_click.png", UriKind.Relative));
            }
            else
            {
                img.Source = new BitmapImage(new Uri("/LOLVideoShow;component/images/ui/media_play_click.png", UriKind.Relative));
            }
        }
        /// <summary>
        /// 实现PLAY按钮的样式且换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play_btn_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Image img = sender as Image;
            if (isPlaying)
            {
                isPlaying = false;
                mediaPlayer.Pause();
                img.Source = new BitmapImage(new Uri("/LOLVideoShow;component/images/ui/media_play.png", UriKind.Relative));
            }
            else
            {
                isPlaying = true;
                mediaPlayer.Play();
                img.Source = new BitmapImage(new Uri("/LOLVideoShow;component/images/ui/media_pause.png", UriKind.Relative));
            }
        }

    }
}