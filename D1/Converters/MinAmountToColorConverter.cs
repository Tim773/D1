using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace D1.Converters
{
    class MinAmountToColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            int MinCount = System.Convert.ToInt32(values[0]);

            int CountInStock = System.Convert.ToInt32(values[1]);

            if(CountInStock < MinCount)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#f19292"));
            }else if(CountInStock >= MinCount * 3)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffba01"));

            }
            else
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#ffffff"));
            }



        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
