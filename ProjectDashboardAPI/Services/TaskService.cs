using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Repositories;

namespace ProjectDashboardAPI.Services
{
    public class TaskService : ITaskService
    {
        private ITaskRepository _taskRepository;
        private ITaskOwnerRepository _taskOwnerRepository;

        public TaskService(ITaskRepository taskRepository, ITaskOwnerRepository taskOwnerRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _taskOwnerRepository = taskOwnerRepository ?? throw new ArgumentNullException(nameof(taskOwnerRepository));
        }

        public void UpdateTasks(List<NotificationTask> tasks, Notification notification)
        {
            List<String> ExistingTasksId = _taskRepository.ReadManyAsyncTaskConcatenatedIdByNotificationId(notification.Id).Result;

            foreach (NotificationTask task in tasks)
            {
                Task taskEntity = _taskRepository.CreateTaskEntity(task, notification).Result;
                if (_taskRepository.VerifyIfTaskAlreadyExists(taskEntity).Result)
                {
                    if (_taskRepository.VerifyIfTaskAsBeenModified(taskEntity).Result)
                    {
                        _taskRepository.UpdateTask(taskEntity);
                    }
                }
                else
                {
                    _taskRepository.AddTask(taskEntity);

                    try
                    {
                        var taskOwner = _taskOwnerRepository.CreateTaskOwner(task.EmployeeId, taskEntity).Result;
                        _taskOwnerRepository.AddTaskOwner(taskOwner);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                ExistingTasksId.Remove(task.SAPid);
            }

            if (ExistingTasksId.Any())
            {
                foreach (String ConcatenatedId in ExistingTasksId)
                {
                    Task TaskToBeDeleted = _taskRepository.ReadOneAsycnTaskByConcatenatedId(ConcatenatedId).Result;
                    

                    if (TaskToBeDeleted != null)
                    {
                        TaskOwner TaskOwnerToBeDeleted = _taskOwnerRepository.ReadOneAsyncTaskOwnerByTaskId(TaskToBeDeleted.Id).Result;

                        _taskRepository.DeleteTask(TaskToBeDeleted);
                        _taskOwnerRepository.DeleteTaskOwner(TaskOwnerToBeDeleted);
                    }
                }
            }
        }
    }
}
