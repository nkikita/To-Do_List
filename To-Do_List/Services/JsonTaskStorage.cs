using System.IO;
using System.Reflection.Emit;
using System.Text.Json;
using System.Threading.Tasks;
using To_Do_List.Models;

namespace To_Do_List.Services
{
    public class JsonTaskStorage : ITaskStorage
    {
        private readonly string _filePath = "tasks.json";

        public List<TaskModel> Load()
        {
            if (!File.Exists(_filePath)) return new List<TaskModel>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TaskModel>>(json) ?? new();
        }
        public void Update(TaskModel updated)
        {
            var tasks = Load(); // ← загружаем текущие задачи

            var existing = tasks.FirstOrDefault(t => t.Id == updated.Id);
            if (existing != null)
            {
                existing.Title = updated.Title;
                existing.Description = updated.Description;
                existing.Status = updated.Status;
            }

            Save(tasks); // ← сохраняем обратно
        }


        public void Save(List<TaskModel> tasks)
        {
            Console.WriteLine("Added success");
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        public void Delete(Guid taskId)
        {
            var tasks = Load();
            var taskToRemove = tasks.FirstOrDefault(t => t.Id == taskId);
            Console.WriteLine("Delete: " + taskToRemove);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
                Save(tasks);
            }
        }
    }
}
