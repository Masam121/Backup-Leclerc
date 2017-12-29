using Microsoft.AspNetCore.Mvc;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetAllNotifications();
        Task<List<NotificationDto>> GetAllActiveNotifications();
        Task<List<NotificationDto>> GetDepartementalNotifications(string departmentId);
        Task<List<NotificationDto>> GetProjectNotification(string projectId);
        Task<List<NotificationDto>> GetEmployeeNotifications(string id);
        Task<List<NotificationDto>> GetEmployeeNotificationsForSpecifiedMonth(string id, string month);
        Task<WorkloadDataDto> GetEmployeeNotificationsWorkload(string id);
        Task<EmployeeWorkloadDataDto> GetEmployeesNotificationsWorkload();
        Task<IActionResult> RefreshNotificationsData();
        Task<NotificationDto> GetNotification(string id);
    }
}
