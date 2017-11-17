using ProjectDashboardAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> ReadAllAsync();
        Task<Notification> ReadAsyncNotificationById(int id);
        Task<NotificationDto> CreateNotification(Notification notification);
    }
}
