﻿using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationSAPToNotificationEntityMapper : IMapper<netflix_prContext, NotificationSAP, Notification>
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

        public Notification Map(netflix_prContext context, NotificationSAP entity)
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
            notificationEntity.EstEffort = double.Parse(entity.EstEffort, System.Globalization.CultureInfo.InvariantCulture);
            notificationEntity.ActualEffort = double.Parse(entity.ActualEffort, System.Globalization.CultureInfo.InvariantCulture);
            notificationEntity.IsCompleted = SetNotificationIsCompleted(entity);
            notificationEntity.CompletedDate = entity.CompletedDate;

            return notificationEntity;
        }
    }
}
