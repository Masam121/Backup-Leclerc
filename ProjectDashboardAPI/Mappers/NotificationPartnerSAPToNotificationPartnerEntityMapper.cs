using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationPartnerSAPToNotificationPartnerEntityMapper : IMapper<netflix_prContext, Tuple<Partner, Notification>, NotificationPartner>
    {
        private IEmployeeRepository _emplopyeeRepository;
        private IRoleRepository _roleRepository;

        public NotificationPartnerSAPToNotificationPartnerEntityMapper(IEmployeeRepository emplopyeeRepository, IRoleRepository roleRepository)
        {
            _emplopyeeRepository = emplopyeeRepository ?? throw new ArgumentNullException(nameof(emplopyeeRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        public string CreatePartnerConcatenatedId(string notificationSAPId, int employeeId, int roleId)
        {
            string s_employeeId = employeeId.ToString();
            string s_roleId = roleId.ToString();

            string concatenatedId = notificationSAPId + s_employeeId + s_roleId;
            return concatenatedId;
        }

        public NotificationPartner Map(netflix_prContext context, Tuple<Partner, Notification> entity)
        {
            NotificationPartner partnerEntity = new NotificationPartner();

            partnerEntity.Notification = entity.Item2;
            string employeeSAPId = TrimZerosFromSAPId(entity.Item1.EmployeId);
            int employeeId = _emplopyeeRepository.ReadAsyncEmployeeId(context, employeeSAPId).Result;

            if (employeeId != 0)
            {
                partnerEntity.EmployeId = employeeId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs an employeeId to be valid");
            }

            int roleId = _roleRepository.ReadOneRoleIdByRoleSigle(context, entity.Item1.Role).Result;
            if (roleId != 0)
            {
                partnerEntity.RoleId = roleId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs a role to be valid");
            }
            partnerEntity.ConcatenatedId = CreatePartnerConcatenatedId(entity.Item2.NotificationSapId, employeeId, roleId);

            partnerEntity.actualEffort = double.Parse(entity.Item1.ActualEffort, System.Globalization.CultureInfo.InvariantCulture);
            partnerEntity.EstEffort = double.Parse(entity.Item1.EstimatedEffort, System.Globalization.CultureInfo.InvariantCulture);
            return partnerEntity;
        }
            
    }
}
