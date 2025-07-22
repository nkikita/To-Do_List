using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using To_Do_List.Helpers;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace To_Do_List.Models
{
   
    public enum MyTaskStatus
    {
        [Description("Новая задача")]
        New,

        [Description("В работе")]
        InProgress,

        [Description("Приостановленная")]
        Stoped,

        [Description("Завершена")]
        Completed
    }


    public class TaskModel : INotifyPropertyChanged
    {
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ObservableCollection<MyTaskStatus> Status { get; set; } = new(); //выбранные статусы
        public string StatusString
        {
            get => string.Join(", ", Status.Select(s => Helpers.TaskStatusValues.GetDescription(s))); //статусы переведенные в стрроку
        }
        public bool IsCompleted => Status.Contains(MyTaskStatus.Completed);

        public bool Hidden { get; set; }//идентификатор для скрытия

        private bool _hiddenas;// Внутреннее поле для свойства Hiddenas
        public bool Hiddenas
        {
            get => _hiddenas;
            set
            {
                if (_hiddenas != value)
                {
                    _hiddenas = value;
                    OnPropertyChanged();

                    if (_hiddenas)
                    {
                        Hidden = true;
                        if (!Status.Contains(MyTaskStatus.Completed))// Если статус "Завершена" отсутствует, добавляем его
                            Status.Add(MyTaskStatus.Completed);
                        
                    }
                    else// Если Hiddenas установлен в false
                    {
                        Hidden = false;
                        Status.Remove(MyTaskStatus.Completed); // Удаляем статус "Завершена" из коллекции статусов
                    }

                    MainViewModel.Instance?.ApplyFilter(); //Вызов метода фильтрации задач в главной модели 
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
