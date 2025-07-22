using System.IO;
using System.Text.Json;
using To_Do_List.Models;

namespace To_Do_List.Services
{ 
    /// Реализация ITaskStorage для хранения задач в JSON-файле.
    public class JsonTaskStorage : ITaskStorage
    {
        // Путь к JSON-файлу, где хранятся задачи
        private readonly string _filePath = "tasks.json";
        /// Загружает все задачи из файла.
        public List<TaskModel> Load()
        {
            // Если файл не существует, возвращаем пустой список
            if (!File.Exists(_filePath))
                return new List<TaskModel>();

            // Читаем содержимое JSON-файла
            var json = File.ReadAllText(_filePath);

            // Десериализуем JSON в список задач
            return JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new();
        }

        /// Обновляет существующую задачу по Id.
        public void Update(TaskModel updated)
        {
            // Загружаем текущие задачи
            var tasks = Load();

            // Ищем задачу по Id
            var existing = tasks.FirstOrDefault(t => t.Id == updated.Id);
            if (existing != null)
            {
                // Обновляем данные задачи
                existing.Title = updated.Title;
                existing.Description = updated.Description;
                existing.Status = updated.Status;
            }

            // Сохраняем обновлённый список обратно в файл
            Save(tasks);
        }

        /// Сохраняет список задач в файл.
        public void Save(List<TaskModel> tasks)
        {
            // Сериализуем задачи в JSON с отступами для читаемости
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });

            // Записываем JSON в файл
            File.WriteAllText(_filePath, json);
        }

        /// Удаляет задачу по Id.
        public void Delete(Guid taskId)
        {
            // Загружаем все задачи
            var tasks = Load();

            // Находим задачу для удаления
            var taskToRemove = tasks.FirstOrDefault(t => t.Id == taskId);

            if (taskToRemove != null)
            {
                // Удаляем задачу из списка и сохраняем
                tasks.Remove(taskToRemove);
                Save(tasks);
            }
        }
    }
}
