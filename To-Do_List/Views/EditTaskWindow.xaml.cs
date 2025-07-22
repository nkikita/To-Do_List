using System.Collections.ObjectModel;
using System.Windows;
using To_Do_List.Helpers;
using To_Do_List.Models;

namespace To_Do_List.Views
{
    public partial class EditTaskWindow : Window
    {
        public string TitleText { get; set; }
        public string DescriptionText { get; set; }
        public ObservableCollection<EnumItem<MyTaskStatus>> NewTaskStatus { get; set; } = new();  // Коллекция возможных статусов задачи, которую можно выбрать в UI

        private readonly TaskModel _originalTask;

        public EditTaskWindow(TaskModel task)// Конструктор окна, принимает задачу и инициализирует поля
        {
            InitializeComponent();
            _originalTask = task;
            TitleText = task.Title;
            DescriptionText = task.Description;
            DataContext = this;
        }
        // Обработчик кнопки "Сохранить"
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _originalTask.Title = TitleText;
            _originalTask.Description = DescriptionText;
            
            _originalTask.Status.Clear();
            foreach (var status in NewTaskStatus.Select(s => s.Value))
                _originalTask.Status.Add(status);

            DialogResult = true;
            Close();
        }
        // Обработчик кнопки "Отмена"
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
