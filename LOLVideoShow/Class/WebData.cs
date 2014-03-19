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
using System.IO;
using System.Text.RegularExpressions;
using LOLVideoShow.Class;
using System.Collections.Generic;

namespace LOLVideoShow.Data
{
    public class WebData
    {
        private WebClient client;
        private List<WebEvent> events;
        private int eventIndex = 0;
        private EventHandler _onBusy;
        private EventHandler _unBusy;
        public Boolean isBusy = false;

        public WebData()
        {
            events = new List<WebEvent>();
            client = new WebClient();
            client.OpenReadCompleted += OnOpenReadCompleted;
        }

        public void SetBusyEvent(EventHandler on, EventHandler un)
        {
            this._onBusy = on;
            this._unBusy = un;
        }

        public void Load(string url, OpenReadCompletedEventHandler callback = null)
        {
            events.Add(new WebEvent(url, callback));
            //如果web空闲，立即执行
            if (!isBusy && eventIndex < events.Count)
            {
                this.isBusy = true;
                if (this._onBusy != null) _onBusy(url, null);
                client.OpenReadAsync(new Uri(events[eventIndex].url));
            }
        }

        private void OnOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (events[eventIndex].callback != null)
            {
                events[eventIndex].callback(null, e);
            }
            eventIndex++;
            //如果后面还有请求，处理
            if (eventIndex < events.Count)
            {
                client.OpenReadAsync(new Uri(events[eventIndex].url));
            }
            else
            {
                this.isBusy = false;
                if (this._unBusy != null) _unBusy(this, null);
            }
        }

        public T JsonToObject<T>(OpenReadCompletedEventArgs e)
        {
            try
            {
                StreamReader sr = new StreamReader(e.Result);
                string str = sr.ReadToEnd();
                string json = Regex.Unescape(str);
                return IsolatedStorageHelper.Deserialize<T>(json);
            }
            catch
            {
                return default(T);
            }
        }
    }
}
