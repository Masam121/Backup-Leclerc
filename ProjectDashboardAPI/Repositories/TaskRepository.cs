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
        private readonly netflix_prContext _context;
        private ITaskMappingService _taskMappingService;

        public TaskRepository(netflix_prContext context, ITaskMappingService taskMappingService)
        {
            _context = context;
            _taskMappingService = taskMappingService ?? throw new ArgumentNullException(nameof(taskMappingService));
        }

        public void AddTask(Task task)
        {
            _context.Task.Add(task);
        }

        public Task<Task> CreateTaskEntity(NotificationTask task, Notification notification)
        {
            return System.Threading.Tasks.Task.FromResult(_taskMappingService.Map(Tuple.Create(task, notification)));
        }

        public void DeleteTask(Task task)
        {
            _context.Remove(task);
        }

        public Task<List<Task>> ReadManyAsyncTaskByNotificationId(int id)
        {
            List<Task> tasks = (from p in _context.Task
                                          where p.NotificationId == id
                                          select p).ToList();

            return System.Threading.Tasks.Task.FromResult(tasks);
        }

        public Task<List<string>> ReadManyAsyncTaskConcatenatedIdByNotificationId(int id)
        {
            List<String> ExistingTasksId = (from p in _context.Task
                                            where p.NotificationId == id
                                            select p.ConcatenatedId).ToList();

            return System.Threading.Tasks.Task.FromResult(ExistingTasksId);
        }

        public Task<Task> ReadOneAsycnTaskByConcatenatedId(string concatenatedId)
        {
            Task task = (from p in _context.Task
                         where p.ConcatenatedId == concatenatedId
                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(task);
        }

        public void UpdateTask(Task task)
        {
            Task TaskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

            TaskExists.Description = task.Description;
            TaskExists.ActualEffort = task.ActualEffort;
            TaskExists.AssignationDate = task.AssignationDate;
            TaskExists.EstEffort = task.EstEffort;
            TaskExists.EstEnd = task.EstEnd;
            TaskExists.IsComplete = task.IsComplete;
            TaskExists.Status = task.Status;
            TaskExists.TaskSAPId = task.TaskSAPId;

            _context.Task.Update(TaskExists);
        }

        public Task<bool> VerifyIfTaskAlreadyExists(Task task)
        {            
            Task taskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);
            if (taskExists != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }            
        }

        public Task<bool> VerifyIfTaskAsBeenModified(Task task)
        {
            Task TaskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

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
