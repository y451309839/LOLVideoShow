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
using System.Collections.Generic;
using Microsoft.Phone.Tasks;
using System.Collections.ObjectModel;
using LOLVideoShow.Data;
using Microsoft.Phone.Shell;

namespace LOLVideoShow.Class
{
    public class CommonTools
    {
        //获取子类型
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }

        public static VideoInfo findById(ObservableCollection<VideoInfo> obj, int id)
        {
            foreach (var item in obj)
            {
                if (item.id == id)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// 播放网络视频
        /// </summary>
        /// <param name="url">视频地址</param>
        public static void playVideo(string url)
        {
            if (url == null) return;
            MediaPlayerLauncher mediaPlayerLauncher = new MediaPlayerLauncher();
            mediaPlayerLauncher.Media = new Uri(url, UriKind.Absolute);
            mediaPlayerLauncher.Location = MediaLocationType.Data;
            mediaPlayerLauncher.Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;
            mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;

            mediaPlayerLauncher.Show();
        }

        /// <summary>
        /// 获取播放历史记录
        /// </summary>
        /// <returns>ObservableCollection<VideoInfo></returns>
        public static ObservableCollection<VideoInfo> GetPlayerHistory()
        {
            return IsolatedStorageHelper.GetObject<ObservableCollection<VideoInfo>>("PlayerHistory");
        }

        /// <summary>
        /// 添加一个播放记录并保存
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ObservableCollection<VideoInfo> SevePlayerHistory(VideoInfo v)
        {
            ObservableCollection<VideoInfo> history = IsolatedStorageHelper.GetObject<ObservableCollection<VideoInfo>>("PlayerHistory");
            if (history == null) history = new ObservableCollection<VideoInfo>();
            if (history.Count == 0 || (history.Count > 0 && !(history[history.Count - 1].title == v.title)))
            {
                history.Add(v);
                IsolatedStorageHelper.SaveObject("PlayerHistory", history);
            }
            return history;
        }

        /// <summary>
        /// 清空播放历史记录
        /// </summary>
        public static void ClearPlayerHistory()
        {
            IsolatedStorageHelper.DeleteObject("PlayerHistory");
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name=”timeStamp”>时间戳</param>
        /// <returns></returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); 
            return dtStart.Add(toNow);
        }
 
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”>DateTime时间</param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = new System.DateTime(1970, 1, 1);
            return (int)(time - startTime).TotalSeconds;
        }

    }
}
