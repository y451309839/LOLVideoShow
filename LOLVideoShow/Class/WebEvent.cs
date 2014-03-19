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

namespace LOLVideoShow.Class
{
    public class WebEvent
    {
        public string url;
        public OpenReadCompletedEventHandler callback;

        public WebEvent(string url, OpenReadCompletedEventHandler back = null)
        {
            this.url = url;
            this.callback = back;
        }
    }
}
