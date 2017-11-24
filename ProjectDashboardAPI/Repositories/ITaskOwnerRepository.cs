using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface ITaskOwnerRepository
    {
        Task<TaskOwner> CreateTaskOwner(string employeeId, Task task);
        Task<TaskOwner> ReadOneAsyncTaskOwnerByTaskId(int taskId);
        void AddTaskOwner(TaskOwner taskOwner);
        void DeleteTaskOwner(TaskOwner taskOwner);
    }
}
