using To_Do_List.Models;
using System.Collections.Generic;

namespace To_Do_List.Services
{
    public interface ITaskStorage
    {
        List<TaskModel> Load();
        void Save(List<TaskModel> tasks);
        void Update(TaskModel task);
        void Delete(Guid id);
    }
}
