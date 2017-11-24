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
        Task<List<NotificationDto>> ReadManyAsync();
        Task<NotificationDto> ReadOneAsyncNotificationDtoById(int id);
        Task<Notification> ReadOneAsyncNotificationById(int id);
        Task<int> ReadOneAsyncNotificationIdByNotificationSAPId(string id);
        Task<Notification> ReadOneAsyncNotificationByNotificationSAPId(string id);
        Task<NotificationDto> CreateNotification(Notification notification);
        Task<Notification> CreateNotificationEntity(NotificationSAP notificationSAP);
        Task<List<NotificationDto>> ReadManyAsyncDepartmentalNotification(string departmentId);
        Task<List<NotificationDto>> ReadManyAsyncProjectNotification(int projectId);
        Task<List<NotificationDto>> ReadManyAsyncNotificationFromPartners(List<NotificationPartner> partners);
        Task<List<string>> ReadManyAsyncNotificationSAPId();
        Task<Boolean> VerifiyIfNotificationExists(Notification notification);
        Task<Boolean> VerifiyIfNotificationAsBeenUpdated(Notification notification);
        void UpdateNotification(Notification notification);
        void AddNotification(Notification notification);
        void DeleteNotification(Notification notification);
        void SaveData();
    }
}
