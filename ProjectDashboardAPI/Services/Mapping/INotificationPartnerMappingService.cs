using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services.Mapping
{
    public interface INotificationPartnerMappingService : IMapper<netflix_prContext, Tuple<Employe, Role>, PartnerDto> ,IMapper<netflix_prContext, Tuple<Partner, Notification>, NotificationPartner>
    {
    }
}
