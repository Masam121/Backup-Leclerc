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
        Task<List<NotificationPartner>> ReadManyPartnersByNotification(netflix_prContext context, Notification notification);
        Task<ProjectNetflixContributor> CreateProjectNetflixContributor(netflix_prContext context, NotificationPartner partner);
        Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(netflix_prContext context, int id);
        Task<IEnumerable<PartnerDto>> CreateNotificationPartnersDto(netflix_prContext context, Notification notification);
        Task<NotificationPartner> CreateNotificationPartnerEntity(netflix_prContext context, Partner partner, Notification notification);
        void UpdatePartner(netflix_prContext context, NotificationSAP notification);
        void AddPartner(netflix_prContext context, NotificationPartner partner);
        void DeletePartner(netflix_prContext context, NotificationPartner partner);
    }
}
