using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyCourse.Converters
{
    public class WeightStringToValueConverter : IValueConverter
    {
        // From string to int
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string intValue = (string)value;
            return int.Parse(intValue);

        }
        // From int to string
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;
            return intValue.ToString();
        }
    }
}
