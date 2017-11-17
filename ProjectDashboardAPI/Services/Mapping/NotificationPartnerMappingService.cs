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

        public NotificationPartnerMappingService(IMapper<Tuple<Employe, Role>, PartnerDto> notificationPartnerToPartnerDtoMapper)
        {
            _NotificationPartnerToPartnerDtoMapper = notificationPartnerToPartnerDtoMapper ?? throw new ArgumentNullException(nameof(notificationPartnerToPartnerDtoMapper));
        }

        public PartnerDto Map(Tuple<Employe, Role> entity)
        {
            return _NotificationPartnerToPartnerDtoMapper.Map(entity);
        }
    }
}
