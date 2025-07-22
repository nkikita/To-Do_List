using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do_List.Models;

namespace To_Do_List.Services
{
    public class TaskFilterService : ITaskFilterService
    {
        // Метод фильтрации задач по статусу и признаку завершённости
        public IEnumerable<TaskModel> Filter(IEnumerable<TaskModel> allTasks, string filter, bool isCompletedChecked)
        {
            // Удаляем лишние пробелы из строки фильтра
            filter = filter?.Trim();

            // Возвращаем отфильтрованные задачи
            return allTasks.Where(task =>
            {
                // Определяем, является ли задача завершённой
                bool isCompleted = task.Status.Contains(MyTaskStatus.Completed);
                bool matchesFilter = true;

                // Если фильтр не пустой и удалось распарсить статус
                if (!string.IsNullOrEmpty(filter) && Enum.TryParse<MyTaskStatus>(filter, out var parsedStatus))
                {
                    // Проверяем, содержит ли задача нужный статус
                    matchesFilter = task.Status.Contains(parsedStatus);
                }

                // Если установлен флаг "Показать завершённые"
                if (isCompletedChecked)
                {
                    if (isCompleted)
                        // Показываем задачу, если она завершена и соответствует фильтру
                        return matchesFilter;

                    // Показываем незавершённую и не скрытую задачу, если она соответствует фильтру
                    return !task.Hidden && matchesFilter;
                }
                else
                {
                    // Показываем только незавершённые и не скрытые задачи, соответствующие фильтру
                    return !isCompleted && !task.Hidden && matchesFilter;
                }
            });
        }
    }
}
