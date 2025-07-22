using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using To_Do_List.Models;

namespace To_Do_List.Helpers
{
    public class EnumItem<T>
    {
        public T Value { get; set; }
        public string Description { get; set; }
    }
    // Утилитный класс для получения описаний статусов задач из enum MyTaskStatus
    public static class TaskStatusValues
    {
        public static List<EnumItem<MyTaskStatus>> All =>
            Enum.GetValues(typeof(MyTaskStatus))
                .Cast<MyTaskStatus>()
                .Select(value => new EnumItem<MyTaskStatus>
                {
                    Value = value,
                    Description = GetDescription(value)
                })
                .ToList();
        // Метод, получающий строку из атрибута [Description(...)] для значения enum
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
