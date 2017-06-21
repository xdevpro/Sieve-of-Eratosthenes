using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Sieve.Eratosthenes.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, object parameter, string language)
        {
            string color = "White";
            if ((bool)value)
            {
                color = parameter.ToString().Split(':')[1];
            }
            else
            {
                color = parameter.ToString().Split(':')[0];
            }
            return color;
        }

        public Object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
