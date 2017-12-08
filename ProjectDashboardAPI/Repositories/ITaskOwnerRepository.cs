using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface ITaskOwnerRepository
    {
        Task<TaskOwner> CreateTaskOwner(netflix_prContext context, string employeeId, Task task);
        Task<TaskOwner> ReadOneAsyncTaskOwnerByTaskId(netflix_prContext context, int taskId);
        void AddTaskOwner(netflix_prContext context, TaskOwner taskOwner);
        void DeleteTaskOwner(netflix_prContext context, TaskOwner taskOwner);
    }
}
