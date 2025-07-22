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

        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
