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
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DWSInterface"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DWSInterface;assembly=DWSInterface"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class CircleControlwithArrow : Control
    {
        // 注册属性，指定属性名称，属性类型，属性的元数据
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text", typeof(string), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(string)));

        // 属性包装器，允许绑定和样式
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty BT01BackgroundProperty =
            DependencyProperty.Register(
                "BT01Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT01Background
        {
            get { return (Brush)GetValue(BT01BackgroundProperty); }
            set { SetValue(BT01BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BT02BackgroundProperty =
            DependencyProperty.Register(
                "BT02Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT02Background
        {
            get { return (Brush)GetValue(BT02BackgroundProperty); }
            set { SetValue(BT02BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BT03BackgroundProperty =
            DependencyProperty.Register(
                "BT03Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT03Background
        {
            get { return (Brush)GetValue(BT03BackgroundProperty); }
            set { SetValue(BT03BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BT04BackgroundProperty =
            DependencyProperty.Register(
                "BT04Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT04Background
        {
            get { return (Brush)GetValue(BT04BackgroundProperty); }
            set { SetValue(BT04BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BT05BackgroundProperty =
            DependencyProperty.Register(
                "BT05Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT05Background
        {
            get { return (Brush)GetValue(BT05BackgroundProperty); }
            set { SetValue(BT05BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BT06BackgroundProperty =
            DependencyProperty.Register(
                "BT06Background", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));
        // 属性包装器，允许绑定和样式
        public Brush BT06Background
        {
            get { return (Brush)GetValue(BT06BackgroundProperty); }
            set { SetValue(BT06BackgroundProperty, value); }
        }

        public static readonly DependencyProperty TFBackgroundProperty =
            DependencyProperty.Register(
                "TFBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush TFBackground
        {
            get { return (Brush)GetValue(TFBackgroundProperty); }
            set { SetValue(TFBackgroundProperty, value); }
        }
        public static readonly DependencyProperty TRBackgroundProperty =
            DependencyProperty.Register(
                "TRBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush TRBackground
        {
            get { return (Brush)GetValue(TRBackgroundProperty); }
            set { SetValue(TRBackgroundProperty, value); }
        }
        public static readonly DependencyProperty LFBackgroundProperty =
            DependencyProperty.Register(
                "LFBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush LFBackground
        {
            get { return (Brush)GetValue(LFBackgroundProperty); }
            set { SetValue(LFBackgroundProperty, value); }
        }
        public static readonly DependencyProperty LRBackgroundProperty =
            DependencyProperty.Register(
                "LRBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush LRBackground
        {
            get { return (Brush)GetValue(LRBackgroundProperty); }
            set { SetValue(LRBackgroundProperty, value); }
        }
        public static readonly DependencyProperty RFBackgroundProperty =
            DependencyProperty.Register(
                "RFBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush RFBackground
        {
            get { return (Brush)GetValue(RFBackgroundProperty); }
            set { SetValue(RFBackgroundProperty, value); }
        }
        public static readonly DependencyProperty RRBackgroundProperty =
            DependencyProperty.Register(
                "RRBackground", typeof(Brush), typeof(CircleControlwithArrow),
                new PropertyMetadata(default(Brush)));

        // 属性包装器，允许绑定和样式
        public Brush RRBackground
        {
            get { return (Brush)GetValue(RRBackgroundProperty); }
            set { SetValue(RRBackgroundProperty, value); }
        }
        static CircleControlwithArrow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircleControlwithArrow), new FrameworkPropertyMetadata(typeof(CircleControlwithArrow)));
        }
    }
}
