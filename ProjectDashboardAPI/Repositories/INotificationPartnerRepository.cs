using NetflixAPI.Models;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface INotificationPartnerRepository
    {
        Task<IEnumerable<NotificationPartner>> ReadAllPartnersFromNotification(Notification notification);
        Task<ProjectNetflixContributor> CreateProjectNetflixContributor(NotificationPartner partner);
        Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(int id);
        Task<IEnumerable<PartnerDto>> CreateNotificationPartnersDto(Notification notification);
    }
}
