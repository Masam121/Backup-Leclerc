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
        private INotificationMappingService _notificationMappingService;
        private INotificationPartnerRepository _notificationPartnerRepository;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

        public NotificationRepository(INotificationMappingService notificationMappingService, 
                                      INotificationPartnerRepository notificationPartnerRepository)
        {
            _notificationMappingService = notificationMappingService ?? throw new ArgumentNullException(nameof(notificationMappingService));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
        }

        public string RemoveUnusedDigitFromSAPProjectId(string projectSAPidWithDigit)
        {
            string projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        public double GetBusinessDays(DateTime startD, DateTime endD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if (endD.DayOfWeek == DayOfWeek.Saturday) calcBusinessDays--;
            if (startD.DayOfWeek == DayOfWeek.Sunday) calcBusinessDays--;

            return calcBusinessDays;
        }

        protected bool VerifyIfNotificationHasValideProjectAffiliated(netflix_prContext context, string projectSAPId)
        {
            int projectId = (from p
                             in context.Project
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

        public Task<NotificationDto> CreateNotification(netflix_prContext context, Notification notification)
        {
            NotificationDto notificationDto = new NotificationDto();

            string projectName = (from p in context.Project
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
            if (notification.StartDate == nullDate)
            {
                notificationDto.startDate = null;
            }
            else
            {
                notificationDto.startDate = notification.StartDate.ToString("yyyy-MM-dd");
            }
            notificationDto.estEffort = notification.EstEffort.ToString();
            notificationDto.actualEffort = notification.ActualEffort.ToString();
            if (notification.StartDate != nullDate && notification.EstEndDate != nullDate)
            {
                double workingDays = GetBusinessDays(notification.StartDate, notification.EstEndDate);
                double workingDaysSoFar = GetBusinessDays(notification.StartDate, DateTime.Today);
                notificationDto.comparator = ((workingDaysSoFar / workingDays) * 100).ToString();

                if(notification.ActualEffort != 0 && notification.EstEffort != 0)
                {
                    double percentageCompletion = Convert.ToDouble((notification.ActualEffort / notification.EstEffort) * 100);
                    notificationDto.completion = Math.Round(percentageCompletion).ToString();
                }
                else
                {
                    notificationDto.completion = 0.ToString();
                }
                
            }
            else
            {
                notificationDto.completion = 0.ToString();
            }
            notificationDto.status = notification.Status;

            return System.Threading.Tasks.Task.FromResult(notificationDto);
        }       

        public Task<List<NotificationDto>> ReadManyAsync(netflix_prContext context)
        {
            IEnumerable<Notification> notifications = (from p in context.Notification
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(context ,notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public async Task<List<NotificationDto>> ReadManyAsyncNotificationDtoFromPartners(netflix_prContext context, List<NotificationPartner> partners)
        {
            List<string> partnerIdAlreadyAdded = new List<string>();
            List<NotificationDto> notifications = new List<NotificationDto>();

            foreach (var partner in partners)
            {
                Notification notification = (from p in context.Notification
                                             where p.Id == partner.NotificationId
                                             select p).FirstOrDefault();

                if (!partnerIdAlreadyAdded.Contains(notification.NotificationSapId))
                {
                    NotificationDto notificationDto = await CreateNotification(context, notification);
                    notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();

                    notifications.Add(notificationDto);

                    partnerIdAlreadyAdded.Add(notification.NotificationSapId);
                }
            }

            return notifications;
        }

        public Task<List<NotificationDto>> ReadManyAsyncDepartmentalNotification(netflix_prContext context, string departmentId)
        {
            IEnumerable<Notification> notifications = (from p in context.Notification
                                                where p.Department == departmentId
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(context, notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<NotificationDto> ReadOneAsyncNotificationDtoById(netflix_prContext context, int id)
        {
            Notification notification = (from p in context.Notification
                                         where p.Id == id
                                         select p).FirstOrDefault();

            NotificationDto notificationDto = CreateNotification(context, notification).Result;
            notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();

            return System.Threading.Tasks.Task.FromResult(notificationDto);
        }

        public Task<List<NotificationDto>> ReadManyAsyncProjectNotificationDto(netflix_prContext context, int projectId)
        {
            List<Notification> notifications = (from p in context.Notification
                                                where p.ProjectId == projectId
                                                select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(context, notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<List<Notification>> ReadManyAsyncProjectNotification(netflix_prContext context, int projectId)
        {
            List<Notification> notifications = (from p in context.Notification
                                                where p.ProjectId == projectId
                                                select p).ToList();

            return System.Threading.Tasks.Task.FromResult(notifications);
        }

        public Task<List<string>> ReadManyAsyncNotificationSAPId(netflix_prContext context)
        {
            List<String> NotificationsSAPId = (from p in context.Notification select p.NotificationSapId).ToList();

            return System.Threading.Tasks.Task.FromResult(NotificationsSAPId);
        }

        public void SaveData(netflix_prContext context)
        {
            context.SaveChanges();
        }

        public void AddNotification(netflix_prContext context, Notification notification)
        {
            context.Notification.Add(notification);
        }

        public void DeleteNotification(netflix_prContext context, Notification notification)
        {
            context.Notification.Remove(notification);
        }

        public void UpdateNotification(netflix_prContext context, Notification notification)
        {
            Notification existingNotificationInDatabase = context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);

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

            context.Notification.Update(existingNotificationInDatabase);
        }

        public Task<Notification> CreateNotificationEntity(netflix_prContext context, NotificationSAP notificationSAP)
        {
            string projectSAPId = RemoveUnusedDigitFromSAPProjectId(notificationSAP.ProjectId);
            if (VerifyIfNotificationHasValideProjectAffiliated(context ,projectSAPId))
            {
                var notification = _notificationMappingService.Map(context, notificationSAP);
                notification.ProjectId = (from p in context.Project
                                          where p.ProjectSapId == projectSAPId
                                          select p.Id).FirstOrDefault();

                return System.Threading.Tasks.Task.FromResult(notification);
            }
            else
            {
                throw new System.ArgumentException("A notification needs to be affilated to a projectID", "original");
            }                     
        }

        public Task<bool> VerifiyIfNotificationExists(netflix_prContext context, Notification notification)
        {
            Notification NotificationExists = context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);
            if (NotificationExists != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
        }

        public Task<bool> VerifiyIfNotificationAsBeenUpdated(netflix_prContext context, Notification notification)
        {
            Notification existingNotificationInDatabase = context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);

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

        public Task<int> ReadOneAsyncNotificationIdByNotificationSAPId(netflix_prContext context, string id)
        {
            int n = (from p in context.Notification
                      where p.NotificationSapId == id
                      select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(n);
        }

        public Task<Notification> ReadOneAsyncNotificationByNotificationSAPId(netflix_prContext context, string id)
        {
            Notification notification = (from p in context.Notification where p.NotificationSapId == id select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }

        public Task<Notification> ReadOneAsyncNotificationById(netflix_prContext context, int id)
        {
            Notification notification = (from p in context.Notification
                                         where p.Id == id
                                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }

        public Task<List<NotificationDto>> ReadManyAsyncActive(netflix_prContext context)
        {
            IEnumerable<Notification> notifications = (from p in context.Notification
                                                       where p.Status != "Completed"
                                                       select p).ToList();

            List<NotificationDto> notificationList = new List<NotificationDto>();
            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotification(context, notification).Result;
                notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, notification).Result.ToList();
                notificationList.Add(notificationDto);
            }

            return System.Threading.Tasks.Task.FromResult(notificationList);
        }

        public Task<bool> VerifyIfNotificationOulookExists(netflix_prContext context, Notification notification)
        {
            string id = (from p in context.Notification
                        where p.OutlookId == notification.OutlookId
                        select p.OutlookId).FirstOrDefault();
            if(id == null)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        public Task<Notification> ReadOneAsyncNotificationByOutlookId(netflix_prContext context, string id)
        {
            Notification notification = (from p in context.Notification
                                         where p.OutlookId == id
                                         select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(notification);
        }
    }
}
