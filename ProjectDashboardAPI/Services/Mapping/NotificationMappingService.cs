using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Mappers;

namespace ProjectDashboardAPI.Services
{
    public class NotificationMappingService : INotificationMappingService
    {
        private readonly IMapper<Notification, NotificationDto> _NotificationEntityToNotificationDtoMapper;


        public NotificationMappingService(IMapper<Notification, NotificationDto> notificationEntityToNotificationDtoMapper)
        {
            _NotificationEntityToNotificationDtoMapper = notificationEntityToNotificationDtoMapper ?? throw new ArgumentNullException(nameof(notificationEntityToNotificationDtoMapper));
        }

        public NotificationDto Map(Notification entity)
        {
            return _NotificationEntityToNotificationDtoMapper.Map(entity);
        }
    }
}
