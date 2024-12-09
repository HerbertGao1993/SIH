using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWSInterface
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class Pager : UserControl
    {
        public static RoutedEvent FirstPageEvent;
        public static RoutedEvent PreviousPageEvent;
        public static RoutedEvent NextPageEvent;
        public static RoutedEvent LastPageEvent;
        
        public static readonly DependencyProperty CurrentPageProperty;
        public static readonly DependencyProperty TotalPageProperty;
        public static readonly DependencyProperty TotalTitleProperty;
        public static readonly DependencyProperty PageTitleProperty;

        public string TotalTitle
        {
            get { return (string)GetValue(TotalTitleProperty); }
            set
            { SetValue(TotalTitleProperty, (string)value); }
        }
        public string PageTitle
        {
            get { return (string)GetValue(PageTitleProperty); }
            set
            { SetValue(PageTitleProperty, (string)value); }
        }
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set 
            { SetValue(CurrentPageProperty, (int)value); }
        }

        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set { SetValue(TotalPageProperty, (int)value); }
        }

        public Pager()
        {
            InitializeComponent();
        }
        static Pager()
        {
            FirstPageEvent = EventManager.RegisterRoutedEvent("FirstPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            PreviousPageEvent = EventManager.RegisterRoutedEvent("PreviousPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            NextPageEvent = EventManager.RegisterRoutedEvent("NextPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));
            LastPageEvent = EventManager.RegisterRoutedEvent("LastPage", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Pager));

            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(Pager), new PropertyMetadata(0));
            TotalPageProperty = DependencyProperty.Register("TotalPage", typeof(int), typeof(Pager), new PropertyMetadata(0));
            TotalTitleProperty = DependencyProperty.Register("TotalTitle", typeof(string), typeof(Pager), new PropertyMetadata(""));
            PageTitleProperty = DependencyProperty.Register("PageTitle", typeof(string), typeof(Pager), new PropertyMetadata(""));
        }

        public event RoutedEventHandler FirstPage
        {
            add { AddHandler(FirstPageEvent, value); }
            remove { RemoveHandler(FirstPageEvent, value); }
        }

        public event RoutedEventHandler PreviousPage
        {
            add { AddHandler(PreviousPageEvent, value); }
            remove { RemoveHandler(PreviousPageEvent, value); }
        }

        public event RoutedEventHandler NextPage
        {
            add { AddHandler(NextPageEvent, value); }
            remove { RemoveHandler(NextPageEvent, value); }
        }

        public event RoutedEventHandler LastPage
        {
            add { AddHandler(LastPageEvent, value); }
            remove { RemoveHandler(LastPageEvent, value); }
        }
        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(FirstPageEvent, this));
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(PreviousPageEvent, this));
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(NextPageEvent, this));
        }

        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(LastPageEvent, this));
        }

        private void rCurrent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                totaltext.Focus();
            }
        }
        
        private void rCurrent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isTextAllowed = Int32.TryParse(e.Text, out _);
            e.Handled = !isTextAllowed;
        }
    }
}
