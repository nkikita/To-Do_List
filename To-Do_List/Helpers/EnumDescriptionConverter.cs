using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace To_Do_List.Helpers
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // Преобразует значение enum в строку-описание из атрибута [Description], если он указан.
        {
            if (value == null)
                return string.Empty;

            var field = value.GetType().GetField(value.ToString()); 
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();  // Пытаемся найти атрибут [Description] у этого поля
            return attr?.Description ?? value.ToString();// Если атрибут найден — возвращаем его значение, иначе — просто имя enum
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
