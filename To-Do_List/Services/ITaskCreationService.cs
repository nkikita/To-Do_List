using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do_List.Models;

namespace To_Do_List.Services
{
    public interface ITaskCreationService
    {
        TaskModel? Create(string title, string description, IEnumerable<MyTaskStatus> statuses);
    }

}
