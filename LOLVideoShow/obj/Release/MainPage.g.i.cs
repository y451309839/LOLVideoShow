﻿#pragma checksum "F:\projects\LOLVideoShow\LOLVideoShow\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "14F3D13E55E8617EECFD1BA66E7EF740"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1022
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using LOLVideoShow.Class;
using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace LOLVideoShow {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ProgressBar progressMain;
        
        internal Microsoft.Phone.Controls.Panorama MainPanorama;
        
        internal LOLVideoShow.Class.DragListBox listMatch;
        
        internal LOLVideoShow.Class.DragListBox listNewTui;
        
        internal System.Windows.Controls.Grid gridTui;
        
        internal LOLVideoShow.Class.DragListBox ListHistory;
        
        internal System.Windows.Controls.StackPanel ListHistoryItems;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/LOLVideoShow;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.progressMain = ((System.Windows.Controls.ProgressBar)(this.FindName("progressMain")));
            this.MainPanorama = ((Microsoft.Phone.Controls.Panorama)(this.FindName("MainPanorama")));
            this.listMatch = ((LOLVideoShow.Class.DragListBox)(this.FindName("listMatch")));
            this.listNewTui = ((LOLVideoShow.Class.DragListBox)(this.FindName("listNewTui")));
            this.gridTui = ((System.Windows.Controls.Grid)(this.FindName("gridTui")));
            this.ListHistory = ((LOLVideoShow.Class.DragListBox)(this.FindName("ListHistory")));
            this.ListHistoryItems = ((System.Windows.Controls.StackPanel)(this.FindName("ListHistoryItems")));
        }
    }
}

