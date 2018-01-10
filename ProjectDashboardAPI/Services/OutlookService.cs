using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using ProjectDashboardAPI.Repositories;

namespace ProjectDashboardAPI.Services
{
    public class OutlookService : IOutlookService
    {
        private IEmployeeRepository _employeeRepository;
        private IProjectRepository _projectRepository;
        private INotificationRepository _notificationRepository;
        private INotificationPartnerRepository _notificationPartnerRepository;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);
        int OutlookTaskRoleId = 14;

        public OutlookService(IEmployeeRepository employeeRepository,
                              IProjectRepository projectRepository,
                              INotificationRepository notificationRepository,
                              INotificationPartnerRepository notificationPartnerRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
        }

        public ExchangeService InstanciateServiceExchange()
        {
            ExchangeService _service = new ExchangeService();
            ExchangeCredentials _credentials = new WebCredentials("sharedtasks@leclerc.ca", "N3ljcJ2d1HJtfQn0YJjS");
            _service.Credentials = (_credentials);
            _service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

            return _service;
        }

        public string SetStatus(string statusOutlook, DateTime estEndDate)
        {
            string status = "";
            switch (statusOutlook)
            {
                case "NotStarted":
                    if (DateTime.Today > estEndDate)
                    {
                        status = "Late";
                    }
                    else
                    {
                        status = "Not Started";
                    }
                    break;
                case "InProgress":
                    if (DateTime.Today > estEndDate)
                    {
                        status = "Late";
                    }
                    else
                    {
                        status = "In Progress";
                    }
                    break;
                case "Completed":
                    status = "Completed";
                    break;
            }
            if(status != "")
            {
                return status;
            }
            else
            {
                throw new System.ArgumentException("Parameter cannot be null", "original");
            }
            
        }

        public async Task<Notification> CreateNotificationWithTaskOutlook(netflix_prContext context, Microsoft.Exchange.WebServices.Data.Task tde,  Project project)
        {
            Notification notification = new Notification();

            notification.ProjectId = project.Id;
            notification.NotificationSapId = tde.Id.UniqueId;
            notification.Description = tde.Subject.ToString();
            notification.StartDate = tde.DateTimeCreated.Date;
            notification.EstEndDate = tde.DueDate.GetValueOrDefault(nullDate);
            notification.ActualEffort = Convert.ToDouble(tde.ActualWork) /60;
            notification.EstEffort = Convert.ToDouble(tde.TotalWork) /60;
            notification.IsCompleted = tde.IsComplete;
            notification.Status = SetStatus(tde.Status.ToString(), tde.DueDate.GetValueOrDefault(nullDate));
            notification.OutlookId = tde.Id.UniqueId.ToString();

            return notification;
        }

        public async void UpdateTask(netflix_prContext context, Item item, string O365Id)
        {
            if (item is Microsoft.Exchange.WebServices.Data.Task)
            {
                Microsoft.Exchange.WebServices.Data.Task tde = (Microsoft.Exchange.WebServices.Data.Task)item;

                if (tde.BillingInformation != null)
                {
                    Project project = await _projectRepository.ReadOneAsyncBySAPId(context, tde.BillingInformation);

                    if(project != null)
                    {
                        Notification notification = await CreateNotificationWithTaskOutlook(context, tde, project);

                        NotificationPartner partner = await CreateNotificationPartnerWithTaskOutlook(context, tde, notification, O365Id);
                        UpdatePartner(context, partner);

                        if (await VerifyIfNotificationOulookAlreadyExists(context, notification))
                        {
                            Notification existingNotification = await _notificationRepository.ReadOneAsyncNotificationByOutlookId(context, notification.OutlookId);

                            if (existingNotification.Description != notification.Description ||
                                existingNotification.StartDate != notification.StartDate ||
                                existingNotification.EstEndDate != notification.EstEndDate ||
                                existingNotification.ActualEffort != notification.ActualEffort ||
                                existingNotification.IsCompleted != notification.IsCompleted ||
                                existingNotification.Status != notification.Status)
                            {
                                existingNotification.Description = notification.Description;
                                existingNotification.StartDate = notification.StartDate;
                                existingNotification.EstEndDate = notification.EstEndDate;
                                existingNotification.ActualEffort = notification.ActualEffort;
                                existingNotification.IsCompleted = notification.IsCompleted;
                                existingNotification.Status = notification.Status;

                                context.Notification.Update(existingNotification);
                            }
                        }
                        else
                        {
                            _notificationRepository.AddNotification(context, notification);
                        }
                       
                    }
                }
            }
        }

        private async void UpdatePartner(netflix_prContext context, NotificationPartner partner)
        {
            if (await _notificationPartnerRepository.VerifyIfPartnerAlreadyExistsByConcatenatedId(context, partner.ConcatenatedId))
            {
                NotificationPartner p = await _notificationPartnerRepository.ReadOneAsyncPartnerByConcatenatedId(context, partner.ConcatenatedId);

                if (p.EstEffort != partner.EstEffort || p.actualEffort != partner.actualEffort)
                {
                    p.EstEffort = partner.EstEffort;
                    p.actualEffort = partner.actualEffort;

                    _notificationPartnerRepository.Update(context, p);
                }
            }
            else
            {
                _notificationPartnerRepository.AddPartner(context, partner);
            }
        }

        public async Task<NotificationPartner> CreateNotificationPartnerWithTaskOutlook(netflix_prContext context, Microsoft.Exchange.WebServices.Data.Task tde, Notification notification, string O365Id)
        {
            NotificationPartner partner = new NotificationPartner();

            Employe employee = await _employeeRepository.ReadOneAsyncByO365Id(context, O365Id);

            partner.ConcatenatedId = CreatePartnerConcatenatedId(tde.Id.UniqueId, employee.Id, OutlookTaskRoleId);
            partner.actualEffort = Convert.ToDouble(tde.ActualWork) / 60;
            partner.EmployeId = employee.Id;
            partner.EstEffort = Convert.ToDouble(tde.TotalWork) / 60;
            partner.Notification = notification;
            partner.RoleId = OutlookTaskRoleId;

            return partner;
           
        }

        public string CreatePartnerConcatenatedId(string OutlookTaskId, int employeeId, int roleId)
        {
            string s_employeeId = employeeId.ToString();
            string s_roleId = roleId.ToString();

            string concatenatedId = OutlookTaskId + s_employeeId + s_roleId;
            return concatenatedId;
        }

        private async Task<bool> VerifyIfNotificationOulookAlreadyExists(netflix_prContext context, Notification notification)
        {
            bool exists = await _notificationRepository.VerifyIfNotificationOulookExists(context, notification);

            return exists;
        }

        public async Task<IActionResult> RefreshOutlookTask()
        {
            using (var context = new netflix_prContext())
            {
                ExchangeService _service = InstanciateServiceExchange();

                List<string> Office365Ids = await _employeeRepository.ReadManyAsyncOffice365Id(context);

                foreach(string O365Id in Office365Ids)
                {
                    if(O365Id == "" || O365Id == null)
                    {
                        continue;
                    }
                    var userMailbox = new Mailbox(O365Id);
                    var folderId = new FolderId(WellKnownFolderName.Tasks, userMailbox);
                    ItemView itemView = new ItemView(int.MaxValue);
                    var userItems = await _service.FindItems(folderId, itemView);

                    if(userItems != null)
                    {
                        foreach(Item item in userItems)
                        {
                            UpdateTask(context, item, O365Id);
                        }
                    }
                }
                context.SaveChanges();
                return new ObjectResult("Successfully refreshed outlook Tasks...");
            }          
        }
    }
}
