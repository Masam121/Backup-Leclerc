using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Services;

namespace ProjectDashboardAPI.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly netflix_prContext _context;

        private INotificationMappingService _notificationMappingService;
        private INotificationPartnerRepository _notificationPartnerRepository;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

        public NotificationRepository(netflix_prContext context, 
                                      INotificationMappingService notificationMappingService, 
                                      INotificationPartnerRepository notificationPartnerRepository)
        {
            _context = context;

            _notificationMappingService = notificationMappingService ?? throw new ArgumentNullException(nameof(notificationMappingService));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
        }

        public string RemoveUnusedDigitFromSAPProjectId(string projectSAPidWithDigit)
        {
            string projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        protected bool VerifyIfNotificationHasValideProjectAffiliated(string projectSAPId)
        {
            int projectId = (from p
                             in _context.Project
                             where p.ProjectSapId == projectSAPId
                             select p.Id).FirstOrDefault();
            if (projectId == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Task<NotificationDto> CreateNotification(Notification notification)
        {
            NotificationDto notificationDto = new NotificationDto();

            string projectName = (from p in _context.Project
                                  where p.Id == notification.ProjectId
                                  select p.ProjectName).FirstOrDefault();
            notificationDto.projectName = projectName;
            notificationDto.Id = notification.NotificationSapId;
            notificationDto.description = notification.Description;
            notificationDto.creationDate = notification.CreationDate.ToString("yyyy-MM-dd");
            if (notification.EstEndDate == nullDate)
            {
                notificationDto.endDate = null;
            }
            else
            {
                notificationDto.endDate = notification.EstEndDate.ToString("yyyy-MM-dd");
            }
            notificationDto.status = notification.Status;

            return System.Threading.Tasks.Task.FromResult(notificationDto);
        }       

        public Task<List<NotificationDto>> ReadManyAsync()
        {
            IEnumerable<Notification> notifications = (from p in _context.Notification
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<List<NotificationDto>> ReadManyAsyncNotificationFromPartners(List<NotificationPartner> partners)
        {
            List<string> partnerIdAlreadyAdded = new List<string>();
            List<NotificationDto> notifications = new List<NotificationDto>();

            foreach (var partner in partners)
            {
                Notification notification = (from p in _context.Notification
                                             where p.Id == partner.NotificationId
                                             select p).FirstOrDefault();

                if (!partnerIdAlreadyAdded.Contains(notification.NotificationSapId))
                {
                    notifications.Add(CreateNotification(notification).Result);
                    partnerIdAlreadyAdded.Add(notification.NotificationSapId);
                }
            }

            return System.Threading.Tasks.Task.FromResult(notifications);
        }

        public Task<List<NotificationDto>> ReadManyAsyncDepartmentalNotification(string departmentId)
        {
            IEnumerable<Notification> notifications = (from p in _context.Notification
                                                where p.Department == departmentId
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<NotificationDto> ReadOneAsyncNotificationDtoById(int id)
        {
            Notification notification = (from p in _context.Notification
                                         where p.Id == id
                                         select p).FirstOrDefault();

            NotificationDto notificationDto = CreateNotification(notification).Result;
            notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(notification).Result.ToList();

            return System.Threading.Tasks.Task.FromResult(notificationDto);
        }

        public Task<List<NotificationDto>> ReadManyAsyncProjectNotification(int projectId)
        {
            List<Notification> notifications = (from p in _context.Notification
                                                where p.ProjectId == projectId
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<List<string>> ReadManyAsyncNotificationSAPId()
        {
            List<String> NotificationsSAPId = (from p in _context.Notification select p.NotificationSapId).ToList();

            return System.Threading.Tasks.Task.FromResult(NotificationsSAPId);
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }

        public void AddNotification(Notification notification)
        {
            _context.Notification.Add(notification);
        }

        public void DeleteNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public void UpdateNotification(Notification notification)
        {
            Notification existingNotificationInDatabase = _context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);

            existingNotificationInDatabase.ProjectId = notification.ProjectId;
            existingNotificationInDatabase.NotificationSapId = notification.NotificationSapId;
            existingNotificationInDatabase.Description = notification.Description;
            existingNotificationInDatabase.CreationDate = notification.CreationDate;
            existingNotificationInDatabase.StartDate = notification.StartDate;
            existingNotificationInDatabase.EstEndDate = notification.EstEndDate;
            existingNotificationInDatabase.Status = notification.Status;
            existingNotificationInDatabase.Department = notification.Department;
            existingNotificationInDatabase.Priority = notification.Priority;
            existingNotificationInDatabase.EstEffort = notification.EstEffort;
            existingNotificationInDatabase.ActualEffort = notification.ActualEffort;

            _context.Notification.Update(existingNotificationInDatabase);
        }

        public Task<Notification> CreateNotificationEntity(NotificationSAP notificationSAP)
        {
            string projectSAPId = RemoveUnusedDigitFromSAPProjectId(notificationSAP.ProjectId);
            if (VerifyIfNotificationHasValideProjectAffiliated(projectSAPId))
            {
                var notification = _notificationMappingService.Map(notificationSAP);
                notification.ProjectId = (from p in _context.Project
                                          where p.ProjectSapId == projectSAPId
                                          select p.Id).FirstOrDefault();

                return System.Threading.Tasks.Task.FromResult(notification);
            }
            else
            {
                throw new System.ArgumentException("A notification needs to be affilated to a projectID", "original");
            }                     
        }

        public Task<bool> VerifiyIfNotificationExists(Notification notification)
        {
            Notification NotificationExists = _context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);
            if (NotificationExists != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
        }

        public Task<bool> VerifiyIfNotificationAsBeenUpdated(Notification notification)
        {
            Notification existingNotificationInDatabase = _context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);

            if (
                    notification.ProjectId == existingNotificationInDatabase.ProjectId &&
                    notification.NotificationSapId == existingNotificationInDatabase.NotificationSapId &&
                    notification.Description == existingNotificationInDatabase.Description &&
                    notification.CreationDate == existingNotificationInDatabase.CreationDate &&
                    notification.StartDate == existingNotificationInDatabase.StartDate &&
                    notification.EstEndDate == existingNotificationInDatabase.EstEndDate &&
                    notification.Status == existingNotificationInDatabase.Status &&
                    notification.Department == existingNotificationInDatabase.Department &&
                    notification.Priority == existingNotificationInDatabase.Priority &&
                    notification.EstEffort == existingNotificationInDatabase.EstEffort &&
                    notification.ActualEffort == existingNotificationInDatabase.ActualEffort)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        public Task<int> ReadOneAsyncNotificationIdByNotificationSAPId(string id)
        {
            int n = (from p in _context.Notification
                      where p.NotificationSapId == id
                      select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(n);
        }

        public Task<Notification> ReadOneAsyncNotificationByNotificationSAPId(string id)
        {
            Notification notification = (from p in _context.Notification where p.NotificationSapId == id select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }

        public Task<Notification> ReadOneAsyncNotificationById(int id)
        {
            Notification notification = (from p in _context.Notification
                                         where p.Id == id
                                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }
    }
}
