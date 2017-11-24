using Microsoft.AspNetCore.Mvc;
using ProjectDashboardAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAllNotifications();
        Task<List<NotificationDto>> GetDepartementalNotifications(string departmentId);
        Task<List<NotificationDto>> GetProjectNotification(string projectId);
        Task<List<NotificationDto>> GetEmployeeNotifications(string id);
        Task<IActionResult> RefreshNotificationsData();
    }
}
