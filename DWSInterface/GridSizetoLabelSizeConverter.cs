using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace DWSInterface
{
    class GridSizetoLabelSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double gridWidth = (double)values[0];
            double gridHeight = (double)values[1];
            // 根据Grid的大小和当前字体大小计算新的字体大小
            // 这里是一个简单的例子，可以根据实际需求进行调整
            double newlabelSize = Math.Max(Math.Max(gridHeight, gridHeight) * 0.6, 10);
            return newlabelSize;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
