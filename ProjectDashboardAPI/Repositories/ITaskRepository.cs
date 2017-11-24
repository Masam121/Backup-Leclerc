using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface ITaskRepository
    {
        Task<List<string>> ReadManyAsyncTaskConcatenatedIdByNotificationId(int id);
        Task<Task> ReadOneAsycnTaskByConcatenatedId(string concatenatedId);
        Task<List<Task>> ReadManyAsyncTaskByNotificationId(int id);
        Task<Task> CreateTaskEntity(NotificationTask task, Notification notification);
        Task<bool> VerifyIfTaskAlreadyExists(Task task);
        Task<bool> VerifyIfTaskAsBeenModified(Task task);
        void UpdateTask(Task task);
        void DeleteTask(Task task);
        void AddTask(Task task);
    }
}
