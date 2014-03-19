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

namespace LOLVideoShow.Data
{
    public class VideoInfo
    {
        public int id { get; set; }
        public string title { get; set; }
        public string info { get; set; }
        public string from { get; set; }
        public string url { get; set; }
        public int jieshuo { get; set; }
        public int match { get; set; }
        public int hero { get; set; }
        public int looked { get; set; }
        public int flower { get; set; }
        public int dateline { get; set; }
        public string time { get; set; }
        public int locked { get; set; }
    }
}
