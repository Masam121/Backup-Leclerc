using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Repositories;

namespace ProjectDashboardAPI.Services
{
    public class NotificationService : INotificationService
    {
        private INotificationRepository _notificationRepository;
        private INotificationPartnerRepository _notificationPartnerRepository;

        public NotificationService(INotificationRepository notificationRepository, INotificationPartnerRepository notificationPartnerRepository)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
        }

        public async Task<List<NotificationDto>> GetAllNotifications()
        {
            List<NotificationDto> notificationList = new List<NotificationDto>();
            IEnumerable<Notification> notifications = await _notificationRepository.ReadAllAsync();

            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = _notificationRepository.CreateNotification(notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(notification).Result.ToList();
                notificationList.Add(notificationDto);
            }
            return notificationList;
        }
    }
}
