using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Services.Mapping;

namespace ProjectDashboardAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private ITaskMappingService _taskMappingService;

        public TaskRepository(ITaskMappingService taskMappingService)
        {
            _taskMappingService = taskMappingService ?? throw new ArgumentNullException(nameof(taskMappingService));
        }

        public void AddTask(netflix_prContext context, Task task)
        {
            context.Task.Add(task);
        }

        public Task<Task> CreateTaskEntity(netflix_prContext context, NotificationTask task, Notification notification)
        {
            return System.Threading.Tasks.Task.FromResult(_taskMappingService.Map(context, Tuple.Create(task, notification)));
        }

        public void DeleteTask(netflix_prContext context, Task task)
        {
            context.Remove(task);
        }

        public Task<List<Task>> ReadManyAsyncTaskByNotificationId(netflix_prContext context, int id)
        {
            List<Task> tasks = (from p in context.Task
                                          where p.NotificationId == id
                                          select p).ToList();

            return System.Threading.Tasks.Task.FromResult(tasks);
        }

        public Task<List<string>> ReadManyAsyncTaskConcatenatedIdByNotificationId(netflix_prContext context, int id)
        {
            List<String> ExistingTasksId = (from p in context.Task
                                            where p.NotificationId == id
                                            select p.ConcatenatedId).ToList();

            return System.Threading.Tasks.Task.FromResult(ExistingTasksId);
        }

        public Task<Task> ReadOneAsycnTaskByConcatenatedId(netflix_prContext context, string concatenatedId)
        {
            Task task = (from p in context.Task
                         where p.ConcatenatedId == concatenatedId
                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(task);
        }

        public void UpdateTask(netflix_prContext context, Task task)
        {
            Task TaskExists = context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

            TaskExists.Description = task.Description;
            TaskExists.ActualEffort = task.ActualEffort;
            TaskExists.AssignationDate = task.AssignationDate;
            TaskExists.EstEffort = task.EstEffort;
            TaskExists.EstEnd = task.EstEnd;
            TaskExists.IsComplete = task.IsComplete;
            TaskExists.Status = task.Status;
            TaskExists.TaskSAPId = task.TaskSAPId;

            context.Task.Update(TaskExists);
        }

        public Task<bool> VerifyIfTaskAlreadyExists(netflix_prContext context, Task task)
        {            
            Task taskExists = context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);
            if (taskExists != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }            
        }

        public Task<bool> VerifyIfTaskAsBeenModified(netflix_prContext context, Task task)
        {
            Task TaskExists = context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

            if (
                    task.Description == TaskExists.Description &&
                    task.ActualEffort == TaskExists.ActualEffort &&
                    task.AssignationDate == TaskExists.AssignationDate &&
                    task.EstEffort == TaskExists.EstEffort &&
                    task.EstEnd == TaskExists.EstEnd &&
                    task.IsComplete == TaskExists.IsComplete &&
                    task.TaskSAPId == TaskExists.TaskSAPId &&
                    task.Status == TaskExists.Status)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }
    }
}
