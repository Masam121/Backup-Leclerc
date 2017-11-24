using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Models.Dto;

namespace ProjectDashboardAPI.Services
{
    public class NotificationMappingService : INotificationMappingService
    {
        private readonly IMapper<Notification, NotificationDto> _NotificationEntityToNotificationDtoMapper;
        private readonly IMapper<NotificationSAP, Notification> _NotificationSAPToNotificationEntityMapper;

        public NotificationMappingService(IMapper<Notification, NotificationDto> notificationEntityToNotificationDtoMapper,
                                          IMapper<NotificationSAP, Notification> notificationSAPToNotificationEntityMapper)
        {
            _NotificationEntityToNotificationDtoMapper = notificationEntityToNotificationDtoMapper ?? throw new ArgumentNullException(nameof(notificationEntityToNotificationDtoMapper));
            _NotificationSAPToNotificationEntityMapper = notificationSAPToNotificationEntityMapper ?? throw new ArgumentNullException(nameof(notificationSAPToNotificationEntityMapper));
        }

        public NotificationDto Map(Notification entity)
        {
            return _NotificationEntityToNotificationDtoMapper.Map(entity);
        }

        public Notification Map(NotificationSAP entity)
        {
            return _NotificationSAPToNotificationEntityMapper.Map(entity);
        }
    }
}
