using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<List<string>> ReadManyAsyncTaskConcatenatedIdByNotificationSAPId(netflix_prContext context, string id);
        Task<Task> ReadOneAsycnTaskByConcatenatedId(netflix_prContext context, string concatenatedId);
        Task<List<Task>> ReadManyAsyncTaskByNotificationId(netflix_prContext context, int id);
        Task<Task> CreateTaskEntity(netflix_prContext context, NotificationTask task, Notification notification);
        Task<bool> VerifyIfTaskAlreadyExists(netflix_prContext context, Task task);
        Task<bool> VerifyIfTaskAsBeenModified(netflix_prContext context, Task task);
        void UpdateTask(netflix_prContext context, Task task);
        void DeleteTask(netflix_prContext context, Task task);
        void AddTask(netflix_prContext context, Task task);
    }
}
