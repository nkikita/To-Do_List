using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using To_Do_List.Helpers;
using To_Do_List.Infrastructure;
using To_Do_List.Models;
using To_Do_List.Services;
using To_Do_List.Views;
using MyTaskStatus = To_Do_List.Models.MyTaskStatus;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ITaskStorage _taskStorage;
    public ObservableCollection<TaskModel> AllTasks { get; set; } = new();
    public ObservableCollection<TaskModel> Tasks { get; set; } = new();
    public ObservableCollection<EnumItem<MyTaskStatus>> NewTaskStatus { get; set; } = new();
    public string NewTaskTitle { get; set; }
    public string NewTaskDescription { get; set; }
    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public ICommand FilterCommand { get; }
    public ICommand ToggleCompletedCommand => new RelayCommand(taskObj =>
    {
        if (taskObj is not TaskModel task)
            return;

        if (task.Status.Contains(MyTaskStatus.Completed))
        {
            task.Status.Remove(MyTaskStatus.Completed);
        }
        else
        {
            task.Status.Add(MyTaskStatus.Completed);
        }

        _taskStorage.Save(AllTasks.ToList());
        ApplyFilter();
    });
    public RelayCommand DeleteCommand { get; }
    private bool _isCompletedChecked;
    public bool IsCompletedChecked  
    {
        get => _isCompletedChecked;
        set
        {
            if (_isCompletedChecked != value)
            {
                _isCompletedChecked = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }
    }
    private string _currentFilter = "";
    public string CurrentFilter
    {
        get => _currentFilter;
        set
        {
            _currentFilter = value;
            OnPropertyChanged();
            ApplyFilter(); 
        }
    }

    public static MainViewModel? Instance { get; private set; }

    public MainViewModel(ITaskStorage taskStorage)
    {
        _taskStorage = taskStorage;
        Instance = this;
        AddCommand = new RelayCommand(_ => AddTask());
        DeleteCommand = new RelayCommand(DeleteTask); 
        EditCommand = new RelayCommand(EditTask);
        FilterCommand = new RelayCommand(OnFilter);

        LoadTasks();
    }
    private void EditTask(object parameter)
    {
        if (parameter is not TaskModel task) return;

        var editWindow = new EditTaskWindow(task);
        if (editWindow.ShowDialog() == true)
        {
            _taskStorage.Update(task);
            ApplyFilter();

        }
    }

    private void DeleteTask(object parameter)
    {
        Debug.WriteLine("DeleteTask вызван с параметром: " + parameter);
        if (parameter is not TaskModel task)
        {
            Debug.WriteLine("Не удалось привести к TaskModel.");
            return;
        }

        _taskStorage.Delete(task.Id);
        AllTasks.Remove(task);
        ApplyFilter();
    }
    private void LoadTasks()
    {
        var loaded = _taskStorage.Load();

        AllTasks.Clear();
        Tasks.Clear();

        foreach (var task in loaded)
            AllTasks.Add(task);
        ApplyFilter();
    }
    
    public void ApplyFilter()
    {
        Tasks.Clear();

        string filter = CurrentFilter.Trim();

        var filtered = AllTasks.Where(task =>
        {
            bool isCompleted = task.Status.Contains(MyTaskStatus.Completed);
            bool matchesFilter = true;

            // Применяем фильтр по статусу, если введён
            if (!string.IsNullOrEmpty(filter) && Enum.TryParse<MyTaskStatus>(filter, out var parsedStatus))
            {
                matchesFilter = task.Status.Contains(parsedStatus);
            }

            if (IsCompletedChecked)
            {
                // Завершённые задачи (включая скрытые)
                if (isCompleted) return matchesFilter;

                // Незавершённые и не скрытые
                return !task.Hidden && matchesFilter;
            }
            else
            {
                // Только не скрытые и не завершённые
                return !isCompleted && !task.Hidden && matchesFilter;
            }
        });

        foreach (var t in filtered)
            Tasks.Add(t);
    }


    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle) && string.IsNullOrWhiteSpace(NewTaskDescription))
        {
            MessageBox.Show("Заполните поля с названием и описанием");
            return;
        }

        var task = new TaskModel
        {
            Title = NewTaskTitle,
            Description = NewTaskDescription,
            Status = new ObservableCollection<MyTaskStatus>(NewTaskStatus.Select(e => e.Value)),
            Hidden = false
        };

        AllTasks.Add(task);
        _taskStorage.Save(AllTasks.ToList());
        ApplyFilter(); // обновить отображаемые задачи

        NewTaskTitle = "";
        NewTaskDescription = "";
        NewTaskStatus.Clear();
        OnPropertyChanged(nameof(NewTaskTitle));
        OnPropertyChanged(nameof(NewTaskDescription));
    }
    private void OnFilter(object? parameter)
    {
        string? filter = parameter?.ToString();
        CurrentFilter = filter ?? "";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
