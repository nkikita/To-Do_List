using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using To_Do_List.Helpers;
using To_Do_List.Infrastructure;
using To_Do_List.Models;
using To_Do_List.Services;
using To_Do_List.Views;
using MyTaskStatus = To_Do_List.Models.MyTaskStatus;

public class MainViewModel : INotifyPropertyChanged
{
    // Сервис для хранения задач (например, JSON, БД)
    private readonly ITaskStorage _taskStorage;
    // Сервис фильтрации задач по статусам и критериям
    private readonly ITaskFilterService _filterService;
    // Сервис создания новых задач
    private readonly ITaskCreationService _creationService;

    // Все загруженные задачи без фильтрации
    public ObservableCollection<TaskModel> AllTasks { get; set; } = new();
    // Отфильтрованные задачи для отображения в UI
    public ObservableCollection<TaskModel> Tasks { get; set; } = new();

    // Список доступных статусов для выбора при добавлении новой задачи
    public ObservableCollection<EnumItem<MyTaskStatus>> NewTaskStatus { get; set; } = new();

    // Заголовок новой задачи, вводимый пользователем
    public string NewTaskTitle { get; set; }
    // Описание новой задачи
    public string NewTaskDescription { get; set; }

    // Команды добавление, изменение и удаление
    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public ICommand FilterCommand { get; }
    // Команда переключения статуса "завершена" для задачи
    public ICommand ToggleCompletedCommand { get; }

    private bool _isCompletedChecked;
    // Флаг, отвечающий за показ завершённых задач
    public bool IsCompletedChecked
    {
        get => _isCompletedChecked;
        set
        {
            if (_isCompletedChecked != value)
            {
                _isCompletedChecked = value;
                OnPropertyChanged();
                ApplyFilter(); // Обновляем фильтр при изменении значения
            }
        }
    }

    private string _currentFilter = "";
    // Текущий выбранный фильтр по статусу 
    public string CurrentFilter
    {
        get => _currentFilter;
        set
        {
            _currentFilter = value;
            OnPropertyChanged();
            ApplyFilter(); // Обновляем фильтр при смене значения
        }
    }

    // Статический экземпляр для упрощённого доступа
    public static MainViewModel? Instance { get; private set; }

    // Конструктор с внедрением зависимостей: хранилище, фильтр, создание
    public MainViewModel(ITaskStorage taskStorage, ITaskFilterService filterService, ITaskCreationService creationService)
    {
        _taskStorage = taskStorage;
        _filterService = filterService;
        _creationService = creationService;

        Instance = this; 

        AddCommand = new RelayCommand(_ => AddTask());
        DeleteCommand = new RelayCommand(DeleteTask);
        EditCommand = new RelayCommand(EditTask);
        FilterCommand = new RelayCommand(OnFilter);
        LoadTasks();
    }

    // Открывает окно редактирования задачи, при успешном сохранении обновляет хранилище и фильтр
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

    // Удаляет задачу из хранилища и из коллекции, обновляет фильтр
    private void DeleteTask(object parameter)
    {
        if (parameter is not TaskModel task) return;

        _taskStorage.Delete(task.Id);
        AllTasks.Remove(task);
        ApplyFilter();
    }

    // Загружает задачи из хранилища в коллекцию AllTasks и применяет фильтр
    private void LoadTasks()
    {
        var loaded = _taskStorage.Load();

        AllTasks.Clear();
        Tasks.Clear();

        foreach (var task in loaded)
            AllTasks.Add(task);

        ApplyFilter();
    }

    // Применяет фильтрацию с помощью сервиса, обновляя коллекцию Tasks для UI
    public void ApplyFilter()
    {
        Tasks.Clear();

        var filtered = _filterService.Filter(AllTasks, CurrentFilter, IsCompletedChecked);

        foreach (var t in filtered)
            Tasks.Add(t);
    }

    // Добавляет новую задачу с помощью сервиса создания, сохраняет, очищает поля и обновляет фильтр
    private void AddTask()
    {
        var task = _creationService.Create(NewTaskTitle, NewTaskDescription, NewTaskStatus.Select(e => e.Value));
        if (task == null) return;

        AllTasks.Add(task);
        _taskStorage.Save(AllTasks.ToList());
        ApplyFilter();

        NewTaskTitle = "";
        NewTaskDescription = "";
        NewTaskStatus.Clear();

        OnPropertyChanged(nameof(NewTaskTitle));
        OnPropertyChanged(nameof(NewTaskDescription));
    }

    // Обработка команды фильтрации, устанавливает текущий фильтр и обновляет отображение
    private void OnFilter(object? parameter)
    {
        string? filter = parameter?.ToString();
        CurrentFilter = filter ?? "";
    }

    // Событие уведомления об изменении свойства (для INotifyPropertyChanged)
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
