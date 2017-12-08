using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface INotificationRepository
    {
        Task<List<NotificationDto>> ReadManyAsync(netflix_prContext context);
        Task<NotificationDto> ReadOneAsyncNotificationDtoById(netflix_prContext context, int id);
        Task<Notification> ReadOneAsyncNotificationById(netflix_prContext context, int id);
        Task<int> ReadOneAsyncNotificationIdByNotificationSAPId(netflix_prContext context, string id);
        Task<Notification> ReadOneAsyncNotificationByNotificationSAPId(netflix_prContext context, string id);
        Task<NotificationDto> CreateNotification(netflix_prContext context, Notification notification);
        Task<Notification> CreateNotificationEntity(netflix_prContext context, NotificationSAP notificationSAP);
        Task<List<NotificationDto>> ReadManyAsyncDepartmentalNotification(netflix_prContext context, string departmentId);
        Task<List<NotificationDto>> ReadManyAsyncProjectNotification(netflix_prContext context, int projectId);
        Task<List<NotificationDto>> ReadManyAsyncNotificationFromPartners(netflix_prContext context, List<NotificationPartner> partners);
        Task<List<string>> ReadManyAsyncNotificationSAPId(netflix_prContext context);
        Task<Boolean> VerifiyIfNotificationExists(netflix_prContext context, Notification notification);
        Task<Boolean> VerifiyIfNotificationAsBeenUpdated(netflix_prContext context, Notification notification);
        void UpdateNotification(netflix_prContext context, Notification notification);
        void AddNotification(netflix_prContext context, Notification notification);
        void DeleteNotification(netflix_prContext context, Notification notification);
        void SaveData(netflix_prContext context);
    }
}
