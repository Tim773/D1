using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace D1.Converters
{
    class ImagePathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is null)
                return "/Image/picture.png";


            string path = value.ToString();


            return new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + path));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
