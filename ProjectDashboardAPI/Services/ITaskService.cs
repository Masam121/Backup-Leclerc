using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface ITaskService
    {
        void UpdateTasks(List<NotificationTask> tasks , Notification notification);
    }
}
