using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do_List.Models;
using System.Windows;

namespace To_Do_List.Services
{
    public class TaskCreationService : ITaskCreationService
    {
        public TaskModel? Create(string title, string description, IEnumerable<MyTaskStatus> statuses)//создание задачи
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(description))//проверка на ввод
            {
               MessageBox.Show("Заполните поля с названием и описанием");
                return null;
            }

            return new TaskModel//создаем новую задачу
            {
                Title = title,
                Description = description,
                Status = new ObservableCollection<MyTaskStatus>(statuses),
                Hidden = false
            };
        }
    }

}
