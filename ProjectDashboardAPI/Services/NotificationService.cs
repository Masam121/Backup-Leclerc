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

        private double averageNumberOfWeekPerMonth = 4.33;
        private int oneFullDayOfWork = 8;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

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
            var timeLineEnd = start.AddMonths(8);

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

        public void deleteUnexistingNotificationInSAP(netflix_prContext context, List<String> notificationsId)
        {
            if (notificationsId.Any())
            {
                foreach (String NotificationSAPId in notificationsId)
                {
                    Notification NotificationToBeDeleted = _notificationRepository.ReadOneAsyncNotificationByNotificationSAPId(context, NotificationSAPId).Result;
                    if (NotificationToBeDeleted != null)
                    {
                        _notificationRepository.DeleteNotification(context, NotificationToBeDeleted);
                        List<NotificationPartner> partnersToBeDeleted = _notificationPartnerRepository.ReadManyPartnersByNotification(context, NotificationToBeDeleted).Result;
                        foreach (NotificationPartner partner in partnersToBeDeleted)
                        {
                            _notificationPartnerRepository.DeletePartner(context, partner);
                        }
                        List<Task> taskToBeDeleted = _taskRepository.ReadManyAsyncTaskByNotificationId(context, NotificationToBeDeleted.Id).Result;
                        foreach (Task task in taskToBeDeleted)
                        {
                            _taskRepository.DeleteTask(context, task);
                            TaskOwner taskOnwerToBeDeleted = _taskOwnerRepository.ReadOneAsyncTaskOwnerByTaskId(context, task.Id).Result;
                            if (taskOnwerToBeDeleted != null)
                            {
                                _taskOwnerRepository.DeleteTaskOwner(context, taskOnwerToBeDeleted);
                            }
                        }
                    }            
                }
            }
        }

        public void AddPartners(netflix_prContext context, List<Partner> partners, Notification notification)
        {
            foreach (Partner partner in partners)
            {
                try
                {
                    NotificationPartner partnerEntity = _notificationPartnerRepository.CreateNotificationPartnerEntity(context, partner, notification).Result;
                    _notificationPartnerRepository.AddPartner(context, partnerEntity);
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }
        }

        public async Task<List<NotificationDto>> GetAllActiveNotifications()
        {
            using (var context = new netflix_prContext())
            {
                IEnumerable<NotificationDto> notifications = await _notificationRepository.ReadManyAsyncActive(context);

                return notifications.ToList();
            }               
        }

        public async Task<List<NotificationDto>> GetAllNotifications()
        {
            using (var context = new netflix_prContext())
            {
                IEnumerable<NotificationDto> notifications = await _notificationRepository.ReadManyAsyncActive(context);

                return notifications.ToList();
            }
        }

        public async Task<NotificationDto> GetNotification(string id)
        {
            using (var context = new netflix_prContext())
            {
                Notification notification = await _notificationRepository.ReadOneAsyncNotificationByNotificationSAPId(context, id);
                NotificationDto notificationDto = await _notificationRepository.CreateNotification(context, notification);

                return notificationDto;
            }
        }

        public async Task<List<NotificationDto>> GetDepartementalNotifications(string departmentId)
        {
            using (var context = new netflix_prContext())
            {
                string id = departmentId.Substring(0, 3);
                IEnumerable<NotificationDto> notifications = await _notificationRepository.ReadManyAsyncDepartmentalNotification(context, id);

                return notifications.ToList();
            }                
        }

        public async Task<List<NotificationDto>> GetEmployeeNotifications(string id)
        {
            using (var context = new netflix_prContext())
            {
                int employeeId = _employeeRepository.ReadAsyncEmployeeId(context, id).Result;
                var partners = _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(context, employeeId).Result;

                return await _notificationRepository.ReadManyAsyncNotificationDtoFromPartners(context, partners);
            }                                 
        }

        public async Task<List<NotificationDto>> GetEmployeeNotificationsForSpecifiedMonth(string id, string month)
        {
            using (var context = new netflix_prContext())
            {
                int employeeId = _employeeRepository.ReadAsyncEmployeeId(context, id).Result;
                var partners = _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(context, employeeId).Result;

                List<NotificationDto> notifications = await _notificationRepository.ReadManyAsyncNotificationDtoFromPartners(context, partners);
                List<NotificationDto> notificationsForSpecifiedMonth = new List<NotificationDto>();
                foreach (NotificationDto notification in notifications)
                {
                    if (notification.startDate != null && notification.endDate != null)
                    {
                        List<string> notificationtimeLine = getListOfMonthBetweenTwoDates(Convert.ToDateTime(notification.startDate), Convert.ToDateTime(notification.endDate));
                        if (notificationtimeLine.Contains(month))
                        {
                            notificationsForSpecifiedMonth.Add(notification);
                        }
                    }
                }
                return notificationsForSpecifiedMonth;
            }
        }

        public async Task<List<NotificationDto>> GetProjectNotification(string projectId)
        {
            using (var context = new netflix_prContext())
            {
                List<NotificationDto> notificationList = new List<NotificationDto>();
                Project project = _projectRepository.ReadOneAsyncBySAPId(context, projectId).Result;

                return await _notificationRepository.ReadManyAsyncProjectNotificationDto(context, project.Id);
            }                
        }

        public async Task<List<double>> getEmployeeMonthlyWorkload(netflix_prContext context, int employeeId)
        {           
            Employe employee = await _employeeRepository.ReadOneAsyncById(context, employeeId);

            double monthlyWorkload = (double)((averageNumberOfWeekPerMonth * employee.ProjectWorkRatio/100 * employee.Workload)/ oneFullDayOfWork);

            List<string> timeLine = createTimeLine();
            Dictionary<string, double> dictionnaryMonthlyWorkloadTimeLine = createTimelineDictionnary(timeLine);
            List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>(dictionnaryMonthlyWorkloadTimeLine);

            foreach (KeyValuePair<string, double> kvp in list)
            {
                dictionnaryMonthlyWorkloadTimeLine[kvp.Key] = monthlyWorkload;
            }

            return dictionnaryMonthlyWorkloadTimeLine.Values.ToList();             
        }

        public async Task<EmployeeWorkloadDataDto> GetEmployeesNotificationsWorkload()
        {
            using (var context = new netflix_prContext())
            {
                EmployeeWorkloadDataDto data = new EmployeeWorkloadDataDto();
                List<string> timeLine = createTimeLine();
                List<EmployeeDataTableRow> dataTableRows = new List<EmployeeDataTableRow>();

                List<Employe> employees = await _employeeRepository.ReadManyAsyncEmployee(context);

                foreach (Employe employee in employees)
                {
                    List<NotificationPartner> partners = await _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(context, employee.Id);

                    if(partners.Any())
                    {
                        EmployeeDataTableRow row = new EmployeeDataTableRow();
                        Dictionary<string, double> dictionnaryEstimatedEffortTimeLine = createTimelineDictionnary(timeLine);

                        foreach (NotificationPartner partner in partners)
                        {
                            Notification notification = await _notificationRepository.ReadOneAsyncNotificationById(context, partner.NotificationId);
                            if (notification.StartDate != nullDate && notification.EstEndDate != nullDate)
                            {
                                List<string> notificationtimeLine = getListOfMonthBetweenTwoDates(Convert.ToDateTime(notification.StartDate), Convert.ToDateTime(notification.EstEndDate));
                                double estimatedMonthlyWorkload = getMonthlyWorkload(notificationtimeLine, (double)partner.EstEffort);

                                foreach (string month in notificationtimeLine)
                                {
                                    dictionnaryEstimatedEffortTimeLine[month] = dictionnaryEstimatedEffortTimeLine[month] + Math.Round(estimatedMonthlyWorkload, 2, MidpointRounding.AwayFromZero);
                                }
                            }
                        }

                        List<double> monthlyWorkloadList = dictionnaryEstimatedEffortTimeLine.Values.ToList();

                        row.Disponibility = Math.Round((double)((averageNumberOfWeekPerMonth * employee.ProjectWorkRatio / 100 * employee.Workload) / oneFullDayOfWork),2);
                        row.Name = employee.Name;
                        row.SAPId = employee.IdSAP;
                        row.Title = employee.Title;
                        row.Department = employee.Department;
                        //La structure suivante est due au format demandé par ngx-datatable
                        row.month0workload = Math.Round((monthlyWorkloadList[0] /row.Disponibility)*100, 2);
                        row.month1workload = Math.Round((monthlyWorkloadList[1] / row.Disponibility) * 100, 2);
                        row.month2workload = Math.Round((monthlyWorkloadList[2] / row.Disponibility) * 100, 2);
                        row.month3workload = Math.Round((monthlyWorkloadList[3] / row.Disponibility) * 100, 2);
                        row.month4workload = Math.Round((monthlyWorkloadList[4] / row.Disponibility) * 100, 2);
                        row.month5workload = Math.Round((monthlyWorkloadList[5] / row.Disponibility) * 100, 2);
                        row.month6workload = Math.Round((monthlyWorkloadList[6] / row.Disponibility) * 100, 2);
                        row.month7workload = Math.Round((monthlyWorkloadList[7] / row.Disponibility) * 100, 2);
                        row.month8workload = Math.Round((monthlyWorkloadList[8] / row.Disponibility) * 100, 2);
                        row.month9workload = Math.Round((monthlyWorkloadList[9] / row.Disponibility) * 100, 2);
                        row.month10workload = Math.Round((monthlyWorkloadList[10] / row.Disponibility) * 100, 2);
                        row.month11workload = Math.Round((monthlyWorkloadList[11] / row.Disponibility) * 100, 2);                       

                        dataTableRows.Add(row);
                    }

                }
                data.Rows = dataTableRows;
                data.TimeLine = timeLine;

                return data;
            }
        }

        public async Task<WorkloadDataDto> GetEmployeeNotificationsWorkload(string id)
        {
            using (var context = new netflix_prContext())
            {
                int employeeId = await _employeeRepository.ReadAsyncEmployeeId(context, id);

                List<NotificationPartner> partners = await _notificationPartnerRepository.ReadAsyncPartnerByEmployeeId(context, employeeId);

                List<string> timeLine = createTimeLine();
                Dictionary<string, double> dictionnaryActualEffortTimeLine = createTimelineDictionnary(timeLine);
                Dictionary<string, double> dictionnaryEstimatedEffortTimeLine = createTimelineDictionnary(timeLine);

                foreach (NotificationPartner partner in partners)
                {
                    Notification notification = await _notificationRepository.ReadOneAsyncNotificationById(context, partner.NotificationId);
                    if(notification.StartDate != nullDate && notification.EstEndDate != nullDate)
                    {
                        List<string> notificationtimeLine = getListOfMonthBetweenTwoDates(Convert.ToDateTime(notification.StartDate), Convert.ToDateTime(notification.EstEndDate));

                        double actualMonthlyWorkload = getMonthlyWorkload(notificationtimeLine, (double)partner.actualEffort);
                        double estimatedMonthlyWorkload = getMonthlyWorkload(notificationtimeLine, (double)partner.EstEffort);

                        foreach (string month in notificationtimeLine)
                        {
                            dictionnaryActualEffortTimeLine[month] = dictionnaryActualEffortTimeLine[month] + Math.Round(actualMonthlyWorkload, 2, MidpointRounding.AwayFromZero);
                            dictionnaryEstimatedEffortTimeLine[month] = dictionnaryEstimatedEffortTimeLine[month] + Math.Round(estimatedMonthlyWorkload, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                }

                WorkloadDataDto workload = new WorkloadDataDto();
                workload.MonthCategory = timeLine;
                workload.EstimatedSerie = dictionnaryEstimatedEffortTimeLine.Values.ToList();
                workload.ActualSerie = dictionnaryActualEffortTimeLine.Values.ToList();
                workload.MonthlyWorkload = await getEmployeeMonthlyWorkload(context, employeeId);

                return workload;
            }               
        }

        public async Task<IActionResult> RefreshNotificationsData()
        {
            using (var context = new netflix_prContext())
            {
                IEnumerable<NotificationSAP> notifications = new List<NotificationSAP>();
                notifications = await _SAPService.GetSapNotification();
                List<String> ExistingNotificationsInDatabase = await _notificationRepository.ReadManyAsyncNotificationSAPId(context);

                foreach (NotificationSAP notification in notifications)
                {
                    Notification notificationEntitity = new Notification();
                    try
                    {
                        notificationEntitity = await _notificationRepository.CreateNotificationEntity(context, notification);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    if (!_notificationRepository.VerifiyIfNotificationExists(context, notificationEntitity).Result)
                    {
                        _notificationRepository.AddNotification(context, notificationEntitity);

                        if (notification.Partners.Any())
                        {
                            try
                            {
                                AddPartners(context, notification.Partners, notificationEntitity);
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
                        if (await _notificationRepository.VerifiyIfNotificationAsBeenUpdated(context, notificationEntitity))
                        {
                            _notificationRepository.UpdateNotification(context, notificationEntitity);
                        }
                        _notificationPartnerRepository.UpdatePartner(context, notification);
                    }
                    ExistingNotificationsInDatabase.Remove(notificationEntitity.NotificationSapId);

                    if (notification.Tasks.Any())
                    {
                        try
                        {
                            _taskService.UpdateTasks(context, notification.Tasks, notificationEntitity);

                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
                deleteUnexistingNotificationInSAP(context, ExistingNotificationsInDatabase);

                context.SaveChanges();
                return new ObjectResult("Successfully refreshed...");
            }                   
        }        
    }
}
