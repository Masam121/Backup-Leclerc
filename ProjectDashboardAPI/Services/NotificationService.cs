using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Repositories;
using ProjectDashboardAPI.Models.Dto;

namespace ProjectDashboardAPI.Services
{
    public class NotificationService : INotificationService
    {
        private INotificationRepository _notificationRepository;
        private INotificationPartnerRepository _notificationPartnerRepository;
        private IProjectRepository _projectRepository;
        private IEmployeeRepository _employeeRepository;
        private ITaskRepository _taskRepository;
        private ITaskOwnerRepository _taskOwnerRepository;

        private ISapService _SAPService;
        private ITaskService _taskService;

        public NotificationService(INotificationRepository notificationRepository, 
                                   INotificationPartnerRepository notificationPartnerRepository, 
                                   IProjectRepository projectRepository,
                                   IEmployeeRepository employeeRepository,
                                   ITaskRepository taskRepository,
                                   ITaskOwnerRepository taskOwnerRepository,
                                   ISapService SAPService,
                                   ITaskService taskService)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _taskOwnerRepository = taskOwnerRepository ?? throw new ArgumentNullException(nameof(taskOwnerRepository));

            _SAPService = SAPService ?? throw new ArgumentNullException(nameof(SAPService));
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        public List<string> getListOfMonthBetweenTwoDates(DateTime start, DateTime End)
        {
            var diff = Enumerable.Range(0, Int32.MaxValue)
                                 .Select(e => start.AddMonths(e))
                                 .TakeWhile(e => e <= End)
                                 .Select(e => e.ToString("MMMM"));

            return diff.ToList();
        }

        public List<string> createTimeLine()
        {
            var start = DateTime.Today;

            var timeLineStart = start.AddMonths(-3);
            var timeLineEnd = start.AddMonths(9);

            // set end-date to end of month
            timeLineEnd = new DateTime(timeLineEnd.Year, timeLineEnd.Month, DateTime.DaysInMonth(timeLineEnd.Year, timeLineEnd.Month));

            var diff = getListOfMonthBetweenTwoDates(timeLineStart , timeLineEnd);

            return diff.ToList();
        }

        public double getMonthlyWorkload(List<string> notificationtimeLine, double effort)
        {
            int numberOfMouth = notificationtimeLine.Count();

            double monthlyWorkload = effort / numberOfMouth;

            return monthlyWorkload;
        }

        public Dictionary<string, double> createTimelineDictionnary(List<string> timeLine) {
            Dictionary<string, double> dico = new Dictionary<string, double>();
            foreach(string month in timeLine)
            {
                dico.Add(month, 0);
            }
            return dico;
        }

        public void deleteUnexistingNotificationInSAP(List<String> notificationsId)
        {
            if (notificationsId.Any())
            {
                foreach (String NotificationSAPId in notificationsId)
                {
                    Notification NotificationToBeDeleted = _notificationRepository.ReadOneAsyncNotificationByNotificationSAPId(NotificationSAPId).Result;

                    if (NotificationToBeDeleted != null)
                    {
                        _notificationRepository.DeleteNotification(NotificationToBeDeleted);

                        List<Task> taskTobeDeleted = _taskRepository.ReadManyAsyncTaskByNotificationId(NotificationToBeDeleted.Id).Result;
                        
                        foreach (Task task in taskTobeDeleted)
                        {
                            _taskRepository.DeleteTask(task);

                            TaskOwner taskOwnerToBeDeleted = _taskOwnerRepository.ReadOneAsyncTaskOwnerByTaskId(task.Id).Result;
                            _taskOwnerRepository.DeleteTaskOwner(taskOwnerToBeDeleted);
                        }

                        List<NotificationPartner> partnerTobeDeleted = _notificationPartnerRepository.ReadManyPartnersByNotification(NotificationToBeDeleted).Result;

                        foreach (NotificationPartner partner in partnerTobeDeleted)
                        {
                            _notificationPartnerRepository.DeletePartner(partner);
                        }
                    }                    
                }
            }
        }

        public void AddPartners(List<Partner> partners, Notification notification)
        {
            foreach (Partner partner in partners)
            {
                try
                {
                    NotificationPartner partnerEntity = _notificationPartnerRepository.CreateNotificationPartnerEntity(partner, notification).Result;
                    _notificationPartnerRepository.AddPartner(partnerEntity);
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public async Task<List<NotificationDto>> GetAllNotifications()
        {
            IEnumerable<NotificationDto> notifications = await _notificationRepository.ReadManyAsync();

            return notifications.ToList();
        }

        public async Task<List<NotificationDto>> GetDepartementalNotifications(string departmentId)
        {
            string id = departmentId.Substring(0, 3);
            IEnumerable<NotificationDto> notifications = await _notificationRepository.ReadManyAsyncDepartmentalNotification(id);
            
            return notifications.ToList();
        }

        public async Task<List<NotificationDto>> GetEmployeeNotifications(string id)
        {
            int employeeId = _employeeRepository.ReadAsyncEmployeeId(id).Result;
            var partners = _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(employeeId).Result;

            return await _notificationRepository.ReadManyAsyncNotificationFromPartners(partners);                   
        }

        public async Task<List<NotificationDto>> GetProjectNotification(string projectId)
        {
            List<NotificationDto> notificationList = new List<NotificationDto>();
            Project project = _projectRepository.ReadOneAsyncBySAPId(projectId).Result;

            return await _notificationRepository.ReadManyAsyncProjectNotification(project.Id);
        }

        public async Task<List<double>> getEmployeeMonthlyWorkload(int employeeId)
        {
            double averageNumberOfWeekPerMonth = 4.33;
            var employee = await _employeeRepository.ReadOneAsyncById(employeeId);

            double monthlyWorkload = (double)(averageNumberOfWeekPerMonth * employee.ProjectWorkRatio * employee.Workload);

            List<string> timeLine = createTimeLine();
            Dictionary<string, double> dictionnaryMonthlyWorkloadTimeLine = createTimelineDictionnary(timeLine);

            foreach(var month in dictionnaryMonthlyWorkloadTimeLine.Keys)
            {
                dictionnaryMonthlyWorkloadTimeLine[month] = monthlyWorkload;
            }

            return dictionnaryMonthlyWorkloadTimeLine.Values.ToList();
        }

        public async Task<WorkloadDataDto> GetEmployeeNotificationsWorkload(string id)
        {
            int employeeId = await _employeeRepository.ReadAsyncEmployeeId(id);

            List<NotificationPartner> partners = await _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(employeeId);

            List<string> timeLine = createTimeLine();
            Dictionary<string, double> dictionnaryActualEffortTimeLine = createTimelineDictionnary(timeLine);
            Dictionary<string, double> dictionnaryEstimatedEffortTimeLine = createTimelineDictionnary(timeLine);

            foreach (NotificationPartner partner in partners)
            {
                Notification notification = await _notificationRepository.ReadOneAsyncNotificationById(partner.NotificationId);
                List<string> notificationtimeLine = getListOfMonthBetweenTwoDates(Convert.ToDateTime(notification.CreationDate), Convert.ToDateTime(notification.EstEndDate));

                double actualMonthlyWorkload = getMonthlyWorkload(notificationtimeLine, partner.actualEffort);
                double estimatedMonthlyWorkload = getMonthlyWorkload(notificationtimeLine, partner.EstEffort);

                foreach (string month in notificationtimeLine)
                {
                    dictionnaryActualEffortTimeLine[month] = actualMonthlyWorkload;
                    dictionnaryEstimatedEffortTimeLine[month] = estimatedMonthlyWorkload;
                }
            }

            WorkloadDataDto workload = new WorkloadDataDto();
            workload.MonthCategory = timeLine;
            workload.EstimatedSerie = dictionnaryEstimatedEffortTimeLine.Values.ToList();
            workload.ActualSerie = dictionnaryActualEffortTimeLine.Values.ToList();
            workload.MonthlyWorkload = await getEmployeeMonthlyWorkload(employeeId);

            return workload;
        }

        public async Task<IActionResult> RefreshNotificationsData()
        {
            IEnumerable<NotificationSAP> notifications = new List<NotificationSAP>();
            notifications = await _SAPService.GetSapNotification();
            List<String> ExistingNotificationsInDatabase = await _notificationRepository.ReadManyAsyncNotificationSAPId();

            foreach (NotificationSAP notification in notifications)
            {
                Notification notificationEntitity = new Notification();
                try
                {
                    notificationEntitity = await _notificationRepository.CreateNotificationEntity(notification);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                if (!_notificationRepository.VerifiyIfNotificationExists(notificationEntitity).Result)
                {
                    _notificationRepository.AddNotification(notificationEntitity);

                    if (notification.Partners.Any())
                    {
                        try
                        {
                            AddPartners(notification.Partners, notificationEntitity);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            continue;
                        }
                    }
                }
                else
                {
                    if (await _notificationRepository.VerifiyIfNotificationAsBeenUpdated(notificationEntitity))
                    {
                        _notificationRepository.UpdateNotification(notificationEntitity);
                    }
                    _notificationPartnerRepository.UpdatePartner(notification);
                }
                ExistingNotificationsInDatabase.Remove(notificationEntitity.NotificationSapId);

                if (notification.Tasks.Any())
                {
                    try
                    {
                        _taskService.UpdateTasks(notification.Tasks, notificationEntitity);

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            deleteUnexistingNotificationInSAP(ExistingNotificationsInDatabase);

            _notificationRepository.SaveData();
            return new ObjectResult("Successfully refreshed...");            
        }

        
    }
}
