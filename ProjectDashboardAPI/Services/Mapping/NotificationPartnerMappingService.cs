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
        private readonly IMapper<netflix_prContext, Tuple<Employe, Role>, PartnerDto> _NotificationPartnerToPartnerDtoMapper;
        private readonly IMapper<netflix_prContext, Tuple<Partner, Notification>, NotificationPartner> _NotificationPartnerSAPToNotificationPartnerEntityMapper;

        public NotificationPartnerMappingService(IMapper<netflix_prContext, Tuple<Employe, Role>, PartnerDto> notificationPartnerToPartnerDtoMapper,
                                                 IMapper<netflix_prContext, Tuple<Partner, Notification>, NotificationPartner> notificationPartnerSAPToNotificationPartnerEntityMapper)
        {
            _NotificationPartnerToPartnerDtoMapper = notificationPartnerToPartnerDtoMapper ?? 
                                                     throw new ArgumentNullException(nameof(notificationPartnerToPartnerDtoMapper));
            _NotificationPartnerSAPToNotificationPartnerEntityMapper = notificationPartnerSAPToNotificationPartnerEntityMapper ?? 
                                                                       throw new ArgumentNullException(nameof(notificationPartnerSAPToNotificationPartnerEntityMapper));
        }

        public PartnerDto Map(netflix_prContext context, Tuple<Employe, Role> entity)
        {
            return _NotificationPartnerToPartnerDtoMapper.Map(context, entity);
        }

        public NotificationPartner Map(netflix_prContext context, Tuple<Partner, Notification> entity)
        {
            return _NotificationPartnerSAPToNotificationPartnerEntityMapper.Map(context, entity);
        }
    }
}
