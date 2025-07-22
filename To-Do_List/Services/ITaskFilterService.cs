using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using To_Do_List.Models;

namespace To_Do_List.Services
{
    public interface ITaskFilterService
    {
        IEnumerable<TaskModel> Filter(IEnumerable<TaskModel> allTasks, string filter, bool isCompletedChecked);
    }

}
