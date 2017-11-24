using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services.Mapping
{
    public class NotificationPartnerMappingService : INotificationPartnerMappingService
    {
        private readonly IMapper<Tuple<Employe, Role>, PartnerDto> _NotificationPartnerToPartnerDtoMapper;
        private readonly IMapper<Tuple<Partner, Notification>, NotificationPartner> _NotificationPartnerSAPToNotificationPartnerEntityMapper;

        public NotificationPartnerMappingService(IMapper<Tuple<Employe, Role>, PartnerDto> notificationPartnerToPartnerDtoMapper,
                                                 IMapper<Tuple<Partner, Notification>, NotificationPartner> notificationPartnerSAPToNotificationPartnerEntityMapper)
        {
            _NotificationPartnerToPartnerDtoMapper = notificationPartnerToPartnerDtoMapper ?? 
                                                     throw new ArgumentNullException(nameof(notificationPartnerToPartnerDtoMapper));
            _NotificationPartnerSAPToNotificationPartnerEntityMapper = notificationPartnerSAPToNotificationPartnerEntityMapper ?? 
                                                                       throw new ArgumentNullException(nameof(notificationPartnerSAPToNotificationPartnerEntityMapper));
        }

        public PartnerDto Map(Tuple<Employe, Role> entity)
        {
            return _NotificationPartnerToPartnerDtoMapper.Map(entity);
        }

        public NotificationPartner Map(Tuple<Partner, Notification> entity)
        {
            return _NotificationPartnerSAPToNotificationPartnerEntityMapper.Map(entity);
        }
    }
}
