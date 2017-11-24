using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class TaskSAPToTaskEntityMapper : IMapper<Tuple<NotificationTask, Notification>, Task>
    {
        private INotificationRepository _notificationRepository;

        public TaskSAPToTaskEntityMapper(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        }

        public string CreateTaskConcatenatedId(string notificationSAPId, string taskKey)
        {
            string concatenatedId = notificationSAPId + taskKey;
            return concatenatedId;
        }

        public string SetTaskSatus(NotificationTask task)
        {
            string status;
            DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);
            if (task.Status == "Complete")
            {
                status = "Completed";
            }
            else
            {
                if (task.EstEnd < System.DateTime.Today && task.EstEnd != nullDate)
                {
                    status = "Late";
                }
                if (task.EstEnd == nullDate)
                {
                    status = "Not Started";
                }
                else
                {
                    status = "In Progress";
                }
            }
            return status;
        }

        public Task Map(Tuple<NotificationTask, Notification> entity)
        {                     
            Task taskEntity = new Task();

            taskEntity.NotificationId = _notificationRepository.ReadOneAsyncNotificationIdByNotificationSAPId(entity.Item2.NotificationSapId).Result;
            taskEntity.ConcatenatedId = CreateTaskConcatenatedId(entity.Item2.NotificationSapId, entity.Item1.TaskKey);
            taskEntity.Description = entity.Item1.Description;
            taskEntity.Type = entity.Item1.Type;
            taskEntity.ActualEffort = 5;
            //taskEntity.ActualEffort = Int32.Parse(task.ActualEffort);
            taskEntity.AssignationDate = entity.Item1.AssignationDate;
            taskEntity.EstEffort = 5;
            taskEntity.EstEnd = entity.Item1.EstEnd;
            if (entity.Item1.IsComplete == "No")
            {
                taskEntity.IsComplete = false;
            }
            else
            {
                taskEntity.IsComplete = true;
            }
            taskEntity.Status = SetTaskSatus(entity.Item1);
            taskEntity.TaskSAPId = entity.Item1.SAPid;

            return taskEntity;
        }
    }
}
