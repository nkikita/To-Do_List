using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace To_Do_List.Helpers
{
    public class BooleanInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // Преобразует значение в противоположное (true -> false, false -> true)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) // Обратное преобразование (используется при двусторонней привязке)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}
