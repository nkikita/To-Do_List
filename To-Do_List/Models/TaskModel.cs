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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ObservableCollection<MyTaskStatus> Status { get; set; } = new();
        public string StatusString
        {
            get => string.Join(", ", Status.Select(s => Helpers.TaskStatusValues.GetDescription(s)));
        }
        public bool IsCompleted => Status.Contains(MyTaskStatus.Completed);

        public bool Hidden { get; set; }

        private bool _hiddenas;
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
                        if (!Status.Contains(MyTaskStatus.Completed))
                            Status.Add(MyTaskStatus.Completed);
                        
                    }
                    else
                    {
                        Hidden = false;
                        Status.Remove(MyTaskStatus.Completed);
                    }

                    MainViewModel.Instance?.ApplyFilter(); //
                }
            }
        }

    }
}
