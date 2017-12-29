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

        public void UpdateTasks(netflix_prContext context, List<NotificationTask> tasks, Notification notification)
        {
            List<String> ExistingTasksId = _taskRepository.ReadManyAsyncTaskConcatenatedIdByNotificationId(context, notification.Id).Result;

            foreach (NotificationTask task in tasks)
            {
                Task taskEntity = _taskRepository.CreateTaskEntity(context, task, notification).Result;
                if (_taskRepository.VerifyIfTaskAlreadyExists(context, taskEntity).Result)
                {
                    if (_taskRepository.VerifyIfTaskAsBeenModified(context, taskEntity).Result)
                    {
                        _taskRepository.UpdateTask(context, taskEntity);
                    }
                }
                else
                {
                    _taskRepository.AddTask(context, taskEntity);

                    try
                    {
                        var taskOwner = _taskOwnerRepository.CreateTaskOwner(context, task.EmployeeId, taskEntity).Result;
                        _taskOwnerRepository.AddTaskOwner(context, taskOwner);
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
                    Task TaskToBeDeleted = _taskRepository.ReadOneAsycnTaskByConcatenatedId(context, ConcatenatedId).Result;


                    if (TaskToBeDeleted != null)
                    {
                        TaskOwner TaskOwnerToBeDeleted = _taskOwnerRepository.ReadOneAsyncTaskOwnerByTaskId(context, TaskToBeDeleted.Id).Result;

                        _taskRepository.DeleteTask(context, TaskToBeDeleted);
                        _taskOwnerRepository.DeleteTaskOwner(context, TaskOwnerToBeDeleted);
                    }
                }
            }               
        }
    }
}
