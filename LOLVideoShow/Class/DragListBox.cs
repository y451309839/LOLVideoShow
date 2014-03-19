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
        private Storyboard OnLoadMore;
        private Storyboard OnEndLoadMore;
        private Storyboard OnEventRun;
        private Storyboard UnEventRun;
        private ScrollViewer ItemsScrollViewer;
        private TextBlock mRefreshTextBlock;
        private TextBlock mLoadMoreTextBlock;

        /// <summary>
        /// 下拉事件接口
        /// </summary>
        public event RoutedEventHandler OnScrollDown;
        /// <summary>
        /// 上拉事件接口
        /// </summary>
        public event RoutedEventHandler OnScrollUp;

        public DragListBox()
        {
            this.DefaultStyleKey = typeof(DragListBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnRefresh = GetTemplateChild("onRefresh") as Storyboard;
            OnEndRefresh = GetTemplateChild("onEndRefresh") as Storyboard;
            OnLoadMore = GetTemplateChild("onLoadMore") as Storyboard;
            OnEndLoadMore = GetTemplateChild("onEndLoadMore") as Storyboard;
            OnEventRun = GetTemplateChild("onEventRun") as Storyboard;
            UnEventRun = GetTemplateChild("unEventRun") as Storyboard;
            ItemsScrollViewer = GetTemplateChild("ItemsScrollViewer") as ScrollViewer;
            mRefreshTextBlock = GetTemplateChild("RefreshTextBlock") as TextBlock;
            mLoadMoreTextBlock = GetTemplateChild("LoadMoreTextBlock") as TextBlock;
            base.MinHeight = 300;
        }

        Boolean IsShowRefresh = false;
        Boolean IsDoRefresh = false;
        Boolean IsShowLoadMore = false;
        Boolean IsDoLoadMore = false;
        double startY;
        int scrolledOffset;
        int scrolledHeight;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            startY = e.GetPosition(ItemsScrollViewer).Y;
            scrolledHeight = (int)ItemsScrollViewer.ScrollableHeight;
            //Debug.WriteLine("OnManipulationStarted startY = " + startY + " scrolledOffset = " + scrolledOffset);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            double currentY = e.GetPosition(ItemsScrollViewer).Y;
            base.OnMouseMove(e);
            double offset = currentY - startY;
            double indexY = scrolledOffset - offset;

            scrolledOffset = (int)ItemsScrollViewer.VerticalOffset;
            //Debug.WriteLine("currentY=" + currentY + " offset=" + offset + " scrolledOffset=" + scrolledOffset);

            ///启用下拉事件属性触发
            if (OnScrollDown != null)
            {
                ///播放下拉刷新动画
                if (IsShowRefresh == false && scrolledOffset <= 0 && indexY < -100)
                {
                    setRefreshVisible(true);
                }
                else if (IsShowRefresh == true && indexY > -100)
                {
                    setRefreshVisible(false);
                }

                ///显示释放刷新
                if (IsDoRefresh == false && indexY < -150)
                {
                    IsDoRefresh = true;
                    mRefreshTextBlock.Text = this.DoRefreshText;
                    OnEventRun.Begin();
                }
                ///上拉时放弃下拉事件
                else if (IsDoRefresh == true && indexY > -150)
                {
                    IsDoRefresh = false;
                    mRefreshTextBlock.Text = this.RefreshText;
                    UnEventRun.Begin();
                }
            }

            ///启用上拉事件属性触发
            if (OnScrollUp != null)
            {
                if (IsShowLoadMore == false && scrolledOffset >= scrolledHeight && offset < -100)
                {
                    setLoadMoreVisible(true);
                    IsDoLoadMore = true;
                }
            }
        }
        

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (OnScrollDown != null && IsDoRefresh == true)
            {
                IsDoRefresh = false;
                OnScrollDown(this, e);
            }
            ///隐藏刷新面板
            setRefreshVisible(false);

            if (OnScrollUp != null && IsDoLoadMore == true)
            {
                IsDoLoadMore = false;
                OnScrollUp(this, e);
            }
            ///隐藏加载面板
            setLoadMoreVisible(false);
        }

        private void setRefreshVisible(Boolean b)
        {
            if (b == true && IsShowRefresh == false)
            {
                mRefreshTextBlock.Text = this.RefreshText;
                IsShowRefresh = true;
                OnRefresh.Begin();
            }
            else if (b == false && IsShowRefresh == true)
            {
                IsShowRefresh = false;
                OnEndRefresh.Begin();
            }
        }

        private void setLoadMoreVisible(Boolean b)
        {
            if (b == true && IsShowLoadMore == false)
            {
                mLoadMoreTextBlock.Text = this.LoadMoreText;
                IsShowLoadMore = true;
                OnLoadMore.Begin();
            }
            else if (b == false && IsShowLoadMore == true)
            {
                IsShowLoadMore = false;
                OnEndLoadMore.Begin();
            }
        }

    }
}
