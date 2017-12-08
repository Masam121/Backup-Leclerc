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
        private readonly IMapper<netflix_prContext, Notification, NotificationDto> _NotificationEntityToNotificationDtoMapper;
        private readonly IMapper<netflix_prContext, NotificationSAP, Notification> _NotificationSAPToNotificationEntityMapper;

        public NotificationMappingService(IMapper<netflix_prContext , Notification, NotificationDto> notificationEntityToNotificationDtoMapper,
                                          IMapper<netflix_prContext, NotificationSAP, Notification> notificationSAPToNotificationEntityMapper)
        {
            _NotificationEntityToNotificationDtoMapper = notificationEntityToNotificationDtoMapper ?? throw new ArgumentNullException(nameof(notificationEntityToNotificationDtoMapper));
            _NotificationSAPToNotificationEntityMapper = notificationSAPToNotificationEntityMapper ?? throw new ArgumentNullException(nameof(notificationSAPToNotificationEntityMapper));
        }

        public NotificationDto Map(netflix_prContext context, Notification entity)
        {
            return _NotificationEntityToNotificationDtoMapper.Map(context, entity);
        }

        public Notification Map(netflix_prContext context, NotificationSAP entity)
        {
            return _NotificationSAPToNotificationEntityMapper.Map(context, entity);
        }
    }
}
