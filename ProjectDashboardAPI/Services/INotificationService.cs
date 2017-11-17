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
    }
}
