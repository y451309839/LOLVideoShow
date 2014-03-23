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
using System.Diagnostics;

namespace LOLVideoShow.Class
{
    public class DragListBox : ItemsControl
    {
        public static readonly DependencyProperty _RefreshText = 
            DependencyProperty.Register("RefreshText", typeof(string), typeof(DragListBox), new PropertyMetadata("下拉刷新", null));
        public string RefreshText { get { return (string)GetValue(_RefreshText); } set { SetValue(_RefreshText, value); } }

        public static readonly DependencyProperty _DoRefreshText =
            DependencyProperty.Register("DoRefreshText", typeof(string), typeof(DragListBox), new PropertyMetadata("释放刷新", null));
        public string DoRefreshText { get { return (string)GetValue(_DoRefreshText); } set { SetValue(_DoRefreshText, value); } }

        public static readonly DependencyProperty _LoadMoreText =
            DependencyProperty.Register("LoadMoreText", typeof(string), typeof(DragListBox), new PropertyMetadata("加载更多", null));
        public string LoadMoreText { get { return (string)GetValue(_LoadMoreText); } set { SetValue(_LoadMoreText, value); } }

        private Storyboard OnRefresh;
        private Storyboard OnEndRefresh;
        private ScrollViewer mItemsScrollViewer;
        private TextBlock mRefreshTextBlock;
        private StackPanel mRefreshPannel;

        /// <summary>
        /// 下拉事件接口
        /// </summary>
        public event RoutedEventHandler OnScrollDownHandler;
        /// <summary>
        /// 上拉事件接口
        /// </summary>
        public event RoutedEventHandler OnScrollUpHandler;

        public DragListBox()
        {
            this.DefaultStyleKey = typeof(DragListBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnRefresh = GetTemplateChild("onRefresh") as Storyboard;
            OnEndRefresh = GetTemplateChild("onEndRefresh") as Storyboard;
            mItemsScrollViewer = GetTemplateChild("itemsScrollViewer") as ScrollViewer;
            mRefreshTextBlock = GetTemplateChild("refreshTextBlock") as TextBlock;
            mRefreshPannel = GetTemplateChild("refreshPanel") as StackPanel;

            base.MinHeight = 300;
        }

        Boolean IsShowRefresh = false;
        Boolean IsDoRefresh = false;
        Boolean IsDoLoadMore = false;
        double startY;
        double scrolledOffset;
        Boolean IsCheckEvent = false;//检测是否是用户多次快速下拉

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);
            IsCheckEvent = true;
            startY = e.ManipulationOrigin.Y;
            scrolledOffset = (int)mItemsScrollViewer.VerticalOffset;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!IsCheckEvent) return;
            double currentY = e.GetPosition(mItemsScrollViewer).Y;
            double offset = currentY - startY;

            ///启用下拉事件属性触发
            if (OnScrollDownHandler != null)
            {
                if (IsShowRefresh == false && scrolledOffset - offset < -30)
                {
                    setRefreshVisible(true);
                }
                else if (IsShowRefresh == true && scrolledOffset - offset >= -30)
                {
                    setRefreshVisible(false);
                }

                if (IsDoRefresh == false && scrolledOffset - offset < -150)
                {
                    IsDoRefresh = true;
                    mRefreshTextBlock.Text = this.DoRefreshText;
                }
                else if (IsDoRefresh == true && scrolledOffset - offset > -150)
                {
                    IsDoRefresh = false;
                    mRefreshTextBlock.Text = this.RefreshText;
                }
            }

            ///启用上拉事件属性触发
            if (OnScrollUpHandler != null)
            {
                if (scrolledOffset >= mItemsScrollViewer.ScrollableHeight)
                {
                    IsDoLoadMore = true;
                }
            }
        }


        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            IsCheckEvent = false;
            if (OnScrollDownHandler != null && IsDoRefresh == true)
            {
                IsDoRefresh = false;
                OnScrollDownHandler(this, e);
            }
            ///隐藏刷新面板
            if (IsShowRefresh == true) setRefreshVisible(false);

            if (OnScrollUpHandler != null && IsDoLoadMore == true)
            {
                IsDoLoadMore = false;
                OnScrollUpHandler(this, e);
            }
        }

        private void setRefreshVisible(Boolean b)
        {
            if (b == true)
            {
                mRefreshTextBlock.Text = this.RefreshText;
                IsShowRefresh = true;
                OnEndRefresh.Stop();
                OnRefresh.Begin();
            }
            else if (b == false)
            {
                IsShowRefresh = false;
                OnRefresh.Stop();
                OnEndRefresh.Begin();
            }
        }

    }
}
