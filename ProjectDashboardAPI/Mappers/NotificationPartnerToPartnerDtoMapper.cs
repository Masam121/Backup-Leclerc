using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationPartnerToPartnerDtoMapper : IMapper<netflix_prContext, Tuple<Employe, Role, NotificationPartner>, PartnerDto>
    {
        public PartnerDto Map(netflix_prContext context, Tuple<Employe, Role, NotificationPartner> entity)
        {
            PartnerDto partnerDto = new PartnerDto();

            partnerDto.employeeName = entity.Item1.Name;
            partnerDto.roleName = entity.Item2.RoleName;
            partnerDto.roleSigle = entity.Item2.RoleSigle;
            partnerDto.estEffort = Math.Round(Convert.ToDouble(entity.Item3.EstEffort), 2).ToString();

            return partnerDto;
        }
    }
}
