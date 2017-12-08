using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationPartnerToPartnerDtoMapper : IMapper<netflix_prContext, Tuple<Employe, Role>, PartnerDto>
    {
        public PartnerDto Map(netflix_prContext context, Tuple<Employe, Role> entity)
        {
            PartnerDto partnerDto = new PartnerDto();

            partnerDto.employeeName = entity.Item1.Name;
            partnerDto.roleName = entity.Item2.RoleName;
            partnerDto.roleSigle = entity.Item2.RoleSigle;

            return partnerDto;
        }
    }
}
