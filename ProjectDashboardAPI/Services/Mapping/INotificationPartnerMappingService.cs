using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services.Mapping
{
    public interface INotificationPartnerMappingService : IMapper<Tuple<Employe, Role>, PartnerDto>
    {
    }
}
