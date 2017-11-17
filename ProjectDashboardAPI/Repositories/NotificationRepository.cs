using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Controllers;

namespace ProjectDashboardAPI.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly netflix_prContext _context;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

        public NotificationRepository(netflix_prContext context)
        {
            _context = context;
        }

        public Task<NotificationDto> CreateNotification(Notification notification)
        {
            NotificationDto notificationDto = new NotificationDto();

            string projectName = (from p in _context.Project
                                  where p.Id == notification.ProjectId
                                  select p.ProjectName).FirstOrDefault();
            notificationDto.projectName = projectName;
            notificationDto.Id = notification.NotificationSapId;
            notificationDto.description = notification.Description;
            notificationDto.creationDate = notification.CreationDate.ToString("yyyy-MM-dd");
            if (notification.EstEndDate == nullDate)
            {
                notificationDto.endDate = null;
            }
            else
            {
                notificationDto.endDate = notification.EstEndDate.ToString("yyyy-MM-dd");
            }
            notificationDto.status = notification.Status;

            return System.Threading.Tasks.Task.FromResult(notificationDto);
        }

        public Task<IEnumerable<Notification>> ReadAllAsync()
        {
            IEnumerable<Notification> notifications = (from p in _context.Notification
                                                select p).ToList();

            return System.Threading.Tasks.Task.FromResult(notifications);
        }

        public Task<Notification> ReadAsyncNotificationById(int id)
        {
            Notification notification = (from p in _context.Notification
                                         where p.Id == id
                                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }
    }
}
