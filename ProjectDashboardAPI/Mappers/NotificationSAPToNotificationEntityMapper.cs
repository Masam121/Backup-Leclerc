using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationSAPToNotificationEntityMapper : IMapper<NotificationSAP, Notification>
    {
        public string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        public String SetNotificationStatus(NotificationSAP notification)
        {
            DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);
            String status;

            if (notification.CompletedDate != nullDate)
            {
                status = "Completed";
            }
            else
            {
                if (notification.EstEndDate < System.DateTime.Today && notification.EstEndDate != nullDate)
                {
                    status = "Late";
                }
                if (notification.EstEndDate == nullDate)
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

        public bool SetNotificationIsCompleted(NotificationSAP notification)
        {
            if (notification.IsCompleted == "True")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Notification Map(NotificationSAP entity)
        {
            Notification notificationEntity = new Notification();
            
            notificationEntity.NotificationSapId = TrimZerosFromSAPId(entity.NotificationSapId);
            notificationEntity.Description = entity.Description;
            notificationEntity.CreationDate = entity.CreationDate;
            notificationEntity.StartDate = entity.StartDate;
            notificationEntity.EstEndDate = entity.EstEndDate;
            notificationEntity.Status = SetNotificationStatus(entity);
            notificationEntity.Department = entity.Department;
            notificationEntity.Priority = entity.Priority;
            notificationEntity.EstEffort = string.IsNullOrEmpty(entity.EstEffort) ? 0 : Convert.ToInt32(entity.EstEffort);
            notificationEntity.ActualEffort = string.IsNullOrEmpty(entity.ActualEffort) ? 0 : Convert.ToInt32(entity.ActualEffort);
            notificationEntity.IsCompleted = SetNotificationIsCompleted(entity);
            notificationEntity.CompletedDate = entity.CompletedDate;

            return notificationEntity;
        }
    }
}
