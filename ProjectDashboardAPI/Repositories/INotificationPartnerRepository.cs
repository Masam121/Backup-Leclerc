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
        Task<List<NotificationPartner>> ReadManyPartnersByNotification(Notification notification);
        Task<ProjectNetflixContributor> CreateProjectNetflixContributor(NotificationPartner partner);
        Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(int id);
        Task<IEnumerable<PartnerDto>> CreateNotificationPartnersDto(Notification notification);
        Task<NotificationPartner> CreateNotificationPartnerEntity(Partner partner, Notification notification);
        void UpdatePartner(NotificationSAP notification);
        void AddPartner(NotificationPartner partner);
        void DeletePartner(NotificationPartner partner);
    }
}
