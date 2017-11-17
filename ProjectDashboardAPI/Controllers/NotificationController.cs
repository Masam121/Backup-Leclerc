using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboardAPI;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ProjectDashboardAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using ProjectDashboardAPI.Services;

namespace ProjectDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly netflix_prContext _context;
        private readonly INotificationService _notificationService;
        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

        public NotificationController(netflix_prContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        protected String RemoveUnusedDigitFromSAPProjectId(String projectSAPidWithDigit)
        {
            String projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        protected string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        protected Notification CreateNotification(NotificationSAP notification)
        {
            Notification notificationEntity = new Notification();
            string projectSAPId = RemoveUnusedDigitFromSAPProjectId(notification.ProjectId);

            if (VerifyIfNotificationHasValideProjectAffiliated(projectSAPId))
            {
                notificationEntity.ProjectId = (from p
                                                in _context.Project
                                                where p.ProjectSapId == projectSAPId
                                                select p.Id).FirstOrDefault();
                notificationEntity.NotificationSapId = TrimZerosFromSAPId(notification.NotificationSapId);
                notificationEntity.Description = notification.Description;
                notificationEntity.CreationDate = notification.CreationDate;
                notificationEntity.StartDate = notification.StartDate;
                notificationEntity.EstEndDate = notification.EstEndDate;
                notificationEntity.Status = SetNotificationStatus(notification);
                notificationEntity.Department = notification.Department;
                notificationEntity.Priority = notification.Priority;
                notificationEntity.EstEffort = string.IsNullOrEmpty(notification.EstEffort) ? 0 : Convert.ToInt32(notification.EstEffort);
                notificationEntity.ActualEffort = string.IsNullOrEmpty(notification.ActualEffort) ? 0 : Convert.ToInt32(notification.ActualEffort);
                notificationEntity.IsCompleted = SetNotificationIsCompleted(notification);
                notificationEntity.CompletedDate = notification.CompletedDate;

                return notificationEntity;
            }
            else
            {
                throw new System.ArgumentException("A notification needs to be affilated to a projectID", "original");
            }
        }

        protected String SetNotificationStatus(NotificationSAP notification)
        {
            String status;
            if (notification.CompletedDate != nullDate)
            {
                status = "Completed";
            }
            else
            {
                if (notification.EstEndDate < System.DateTime.Today && notification.EstEndDate != nullDate)
                {
                    status = "Late";
                }
                if (notification.EstEndDate == nullDate)
                {
                    status = "Not Started";
                }
                else
                {
                    status = "In Progress";
                }
            }
            return status;
        }

        protected String SetTaskSatus(NotificationTask task)
        {
            String status;
            if (task.Status == "Complete")
            {
                status = "Completed";
            }
            else
            {
                if (task.EstEnd < System.DateTime.Today && task.EstEnd != nullDate)
                {
                    status = "Late";
                }
                if (task.EstEnd == nullDate)
                {
                    status = "Not Started";
                }
                else
                {
                    status = "In Progress";
                }
            }
            return status;
        }

        protected bool SetNotificationIsCompleted(NotificationSAP notification)
        {
            if (notification.IsCompleted == "True")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected Task CreateTask(NotificationTask task, Notification notification)
        {
            int id = (from p in _context.Notification
                      where p.NotificationSapId == notification.NotificationSapId
                      select p.Id).FirstOrDefault();

            Task taskEntity = new Task();
            taskEntity.NotificationId = id;
            taskEntity.ConcatenatedId = CreateTaskConcatenatedId(notification.NotificationSapId, task.TaskKey);
            taskEntity.Description = task.Description;
            taskEntity.Type = task.Type;
            taskEntity.ActualEffort = 5;
            //taskEntity.ActualEffort = Int32.Parse(task.ActualEffort);
            taskEntity.AssignationDate = task.AssignationDate;
            taskEntity.EstEffort = 5;
            taskEntity.EstEnd = task.EstEnd;
            if(task.IsComplete == "No")
            {
                taskEntity.IsComplete = false;
            }
            else
            {
                taskEntity.IsComplete = true;
            }
            taskEntity.Status = SetTaskSatus(task);
            taskEntity.TaskSAPId = task.SAPid;

            return taskEntity;
        }

        protected TaskOwner CreateTaskOwner(string employeeSAPdD, Task task)
        {
            Employe employee = _context.Employe.FirstOrDefault(x => x.IdSAP == employeeSAPdD);
            TaskOwner taskOwner = new TaskOwner();
            taskOwner.Task = task;
            taskOwner.EmployeId = employee.Id;

            return taskOwner;
        }

        protected NotificationPartner CreatePartner(Partner partner, Notification notification)
        {
            NotificationPartner partnerEntity = new NotificationPartner();
            partnerEntity.Notification = notification;
            string employeeSAPId = TrimZerosFromSAPId(partner.EmployeId);
            int employeeId = (from p in _context.Employe where p.IdSAP == employeeSAPId select p.Id).FirstOrDefault();
            if (employeeId != 0)
            {
                partnerEntity.EmployeId = employeeId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs an employeeId to be valid");
            }
            var roleId = (from p in _context.Role where p.RoleSigle == partner.Role select p.Id).FirstOrDefault();
            if (roleId != 0)
            {
                partnerEntity.RoleId = roleId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs a role to be valid");
            }
            partnerEntity.ConcatenatedId = CreatePartnerConcatenatedId(notification.NotificationSapId, employeeId, roleId);
            return partnerEntity;         
        }

        protected string CreateTaskConcatenatedId(string notificationSAPId, string taskKey)
        {
            string concatenatedId = notificationSAPId + taskKey;
            return concatenatedId;
        }

        protected string CreatePartnerConcatenatedId(string notificationSAPId, int employeeId, int roleId)
        {
            string s_employeeId = employeeId.ToString();
            string s_roleId = roleId.ToString();

            string concatenatedId = notificationSAPId + s_employeeId + s_roleId;
            return concatenatedId;
        }

        protected NotificationPartner CreatePartnerWithExistingNotification(Partner partner, NotificationSAP notification)
        {
            NotificationPartner partnerEntity = new NotificationPartner();
            Notification n = (from p in _context.Notification
                              where p.NotificationSapId == TrimZerosFromSAPId(notification.NotificationSapId)
                              select p).FirstOrDefault();

            partnerEntity.NotificationId = n.Id;

            string employeeSAPId = TrimZerosFromSAPId(partner.EmployeId);
            int employeeId = (from p in _context.Employe where p.IdSAP == employeeSAPId select p.Id).FirstOrDefault();
            if (employeeId != 0)
            {
                partnerEntity.EmployeId = employeeId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs an employeeId to be valid");
            }
            var roleId = (from p in _context.Role where p.RoleSigle == partner.Role select p.Id).FirstOrDefault();
            if (roleId != 0)
            {
                partnerEntity.RoleId = roleId;
            }
            else
            {
                throw new System.ArgumentException("A partner needs a role to be valid");
            }
            partnerEntity.ConcatenatedId = CreatePartnerConcatenatedId(n.NotificationSapId , employeeId, roleId);
            return partnerEntity;       
        }

        protected bool VerifyIfNotificationAlreadyExistsInDataBase(Notification notification)
        {
            Notification NotificationExists = _context.Notification.FirstOrDefault(x => x.NotificationSapId == notification.NotificationSapId);
            if (NotificationExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        protected bool VerifyIfNotificationAsBeenModified(Notification notification)
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
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void updateNotificationInDatabase(Notification notification)
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

        protected NotificationDto CreateNotificationDto(Notification notification)
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
            notificationDto.partners = GetAllNotificationPartners(notification);

            return notificationDto;
        }

        protected List<PartnerDto> GetAllNotificationPartners(Notification notification)
        {
            var notificationPartners = (from p in _context.NotificationPartner
                                        where p.NotificationId == notification.Id
                                        join e in _context.Employe on p.EmployeId equals e.Id
                                        join r in _context.Role on p.RoleId equals r.Id
                                        select new { notificationPartner = p, employee = e, role = r }).ToList();

            List<PartnerDto> partners = new List<PartnerDto>();

            foreach (var partner in notificationPartners)
            {
                PartnerDto partnerDto = new PartnerDto();
                partnerDto.employeeName = partner.employee.Name;
                partnerDto.roleName = partner.role.RoleName;
                partnerDto.roleSigle = partner.role.RoleSigle;
                partners.Add(partnerDto);
            }

            return partners;
        }

        protected String FormatPartner(String roleSigle, String roleName, String employeeName)
        {
            String formatedPartner = roleSigle + "-" + roleName + "-" + employeeName;
            return formatedPartner;
        }

        protected void deleteUnexistingNotificationInSAP(List<String> notificationsId)
        {
            if (notificationsId.Any())
            {
                foreach (String NotificationSAPId in notificationsId)
                {
                    Notification NotificationToBeDeleted = (from p in _context.Notification where p.NotificationSapId == NotificationSAPId select p).FirstOrDefault();
                    if (NotificationToBeDeleted != null)
                    {
                        _context.Notification.Remove(NotificationToBeDeleted);
                    }

                    List<Task> taskTobeDeleted = (from p in _context.Task where p.NotificationId == NotificationToBeDeleted.Id select p).ToList();
                    foreach (Task task in taskTobeDeleted)
                    {
                        _context.Task.Remove(task);
                        TaskOwner taskOwnerToBeDeleted = (from p in _context.TaskOwner where p.TaskId == task.Id select p).FirstOrDefault();
                        _context.TaskOwner.Remove(taskOwnerToBeDeleted);
                    }

                    List<NotificationPartner> partnerTobeDeleted = (from p in _context.NotificationPartner where p.NotificationId == NotificationToBeDeleted.Id select p).ToList();
                    foreach (NotificationPartner partner in partnerTobeDeleted)
                    {
                        _context.NotificationPartner.Remove(partner);
                    }
                }
            }
        }

        protected void updateNotificationTasks(List<NotificationTask> tasks, Notification notification)
        {
            List<String> ExistingTasksId = new List<String>();
            ExistingTasksId = (from p in _context.Task
                             where p.NotificationId == notification.Id
                             select p.ConcatenatedId).ToList();

            foreach (NotificationTask task in tasks)
            {
                Task taskEntity = CreateTask(task, notification);
                if (verifyIfTaskAlreadyExists(taskEntity))
                {
                    if (verifyIfTaskAsBeenModified(taskEntity))
                    {
                        updateTask(taskEntity);
                    }
                }
                else
                {
                    _context.Task.Add(taskEntity);
                    try
                    {
                        addTaskOwner(task.EmployeeId, taskEntity);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                ExistingTasksId.Remove(task.SAPid);
            }

            if (ExistingTasksId.Any())
            {
                foreach (String ConcatenatedId in ExistingTasksId)
                {
                    var TaskToBeDeleted = (from p in _context.Task
                                           where p.ConcatenatedId == ConcatenatedId
                                           select p).FirstOrDefault();

                    var TaskOwnerToBeDeleted = (from p in _context.TaskOwner
                                                where p.TaskId == TaskToBeDeleted.Id
                                                select p).FirstOrDefault();

                    if (TaskToBeDeleted != null)
                    {
                        removeTask(TaskToBeDeleted, TaskOwnerToBeDeleted);                 
                    }
                }
            }
        }

        protected void removeTask(Task task, TaskOwner taskOwner)
        {
            _context.Remove(taskOwner);
            _context.Remove(task);
        }

        protected void addTaskOwner(string employeeSAPId, Task task)
        {
            if(employeeSAPId == "" || employeeSAPId == null)
            {
                throw new System.ArgumentException("A TaskOnwer needs to be affilated to a valid employeeId", "employeeSAPId :" + employeeSAPId);
            }
            else
            {
                int employeeId = (from p in _context.Employe
                                  where p.IdSAP == TrimZerosFromSAPId(employeeSAPId)
                                  select p.Id).FirstOrDefault();

                if(employeeId == 0)
                {
                    throw new System.ArgumentException("The id needs to belongs to an existing employee in the database", "employeeSAPId :" + employeeSAPId);
                }
                TaskOwner taskOwner = new TaskOwner();
                taskOwner.Task = task;
                taskOwner.EmployeId = employeeId;
                _context.TaskOwner.Add(taskOwner);
            }           
        }

        protected bool verifyIfTaskAlreadyExists(Task task)
        {
            Task taskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);
            if (taskExists != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool verifyIfTaskAsBeenModified(Task task)
        {
            Task TaskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

            if (
                    task.Description == TaskExists.Description &&
                    task.ActualEffort == TaskExists.ActualEffort &&
                    task.AssignationDate == TaskExists.AssignationDate &&
                    task.EstEffort == TaskExists.EstEffort &&
                    task.EstEnd == TaskExists.EstEnd &&
                    task.IsComplete == TaskExists.IsComplete &&
                    task.TaskSAPId == TaskExists.TaskSAPId &&
                    task.Status == TaskExists.Status)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected void updateTask(Task task)
        {
            Task TaskExists = _context.Task.FirstOrDefault(x => x.ConcatenatedId == task.ConcatenatedId);

            TaskExists.Description = task.Description;
            TaskExists.ActualEffort = task.ActualEffort;
            TaskExists.AssignationDate = task.AssignationDate;
            TaskExists.EstEffort = task.EstEffort;
            TaskExists.EstEnd = task.EstEnd;
            TaskExists.IsComplete = task.IsComplete;
            TaskExists.Status = task.Status;
            TaskExists.TaskSAPId = task.TaskSAPId;

            _context.Task.Update(TaskExists);
        }

        protected void VerifyIfNotificationPartnerAsBeenModified(NotificationSAP notification)
        {
            Notification n = (from p in _context.Notification
                              where p.NotificationSapId == TrimZerosFromSAPId(notification.NotificationSapId)
                              select p).FirstOrDefault();

            Dictionary<string, NotificationPartner> partners = (from p in _context.NotificationPartner
                                                              where p.NotificationId == n.Id
                                                              select p).ToDictionary(p => p.ConcatenatedId, p => p);

            var listOfPartnerTobeDeleted = (from p in _context.NotificationPartner
                                            where p.NotificationId == n.Id
                                            select p).ToDictionary(p => p.ConcatenatedId, p => p);

            List<Partner> listOfPartnerTobeAdded = new List<Partner>();

            foreach (var partner in notification.Partners)
            {
                int employeeId = (from p in _context.Employe
                                    where p.IdSAP == TrimZerosFromSAPId(partner.EmployeId)
                                    select p.Id).FirstOrDefault();

                int roleId = (from p in _context.Role
                              where p.RoleSigle == partner.Role
                              select p.Id).FirstOrDefault();

                string concatenatedId = CreatePartnerConcatenatedId(n.NotificationSapId, employeeId, roleId);

                if (partners.ContainsKey(concatenatedId))
                {
                    listOfPartnerTobeDeleted.Remove(concatenatedId);
                }
                else
                {
                    listOfPartnerTobeAdded.Add(partner);
                }
            }
            if (listOfPartnerTobeDeleted.Any())
            {
                foreach (NotificationPartner partner in listOfPartnerTobeDeleted.Values)
                {
                    _context.NotificationPartner.Remove(partner);
                }
            }
            if (listOfPartnerTobeAdded.Any())
            {
                foreach (Partner partner in listOfPartnerTobeAdded)
                {
                    try
                    {
                        _context.NotificationPartner.Add(CreatePartnerWithExistingNotification(partner, notification)); 
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                }
            }
        }      

        protected async Task<IEnumerable<NotificationSAP>> GetNotificationsSAP()
        {
            IEnumerable<NotificationSAP> notifications = new List<NotificationSAP>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = await client.GetAsync(string.Concat("http://api.dev.gbl/v3/", "notifications"));
                data.EnsureSuccessStatusCode();
                var stringResult = await data.Content.ReadAsStringAsync();
                notifications = JsonConvert.DeserializeObject<IEnumerable<NotificationSAP>>(stringResult);
            }
            return notifications;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            List<NotificationDto> notificationList = await _notificationService.GetAllNotifications();

            return Ok(notificationList);
        }

        [HttpGet("{departmentId}", Name = "getByDepartementalIdNotifications")]
        public async Task<IActionResult> GetDepartementalNotifications(string departmentId)
        {
            if(departmentId.Substring(0, 3) == "All")
            {
                return Ok(GetAllNotifications());
            }
            else
            {
                List<NotificationDto> notificationList = new List<NotificationDto>();
                string id = departmentId.Substring(0, 3);
                List<Notification> notifications = (from p in _context.Notification
                                                    where p.Department == id
                                                    select p).ToList();

                foreach (Notification notification in notifications)
                {
                    NotificationDto notificationDto = CreateNotificationDto(notification);
                    notificationList.Add(notificationDto);
                }
                return Ok(notificationList);
            }            
        }

        [HttpGet("project/{projectId}", Name = "getByProjectIdNotification")]
        public IEnumerable<NotificationDto> getNotificationByProjectId(string projectId)
        {
            List<NotificationDto> notificationList = new List<NotificationDto>();
            int myProjectId = (from p in _context.Project
                             where p.ProjectSapId == projectId
                             select p.Id).FirstOrDefault();

            List<Notification> notifications = (from p in _context.Notification
                                                where p.ProjectId == myProjectId
                                                select p).ToList();

            foreach (Notification notification in notifications)
            {
                NotificationDto notificationDto = CreateNotificationDto(notification);
                notificationList.Add(notificationDto);
            }
            return notificationList;
        }

        [HttpGet("employee/{id}", Name = "getByEmployeeIdNotifications")]
        public IEnumerable<NotificationDto> getByEmployeeIdNotifications(string id)
        {
            int employeeId = (from p in _context.Employe
                              where p.IdSAP == id
                              select p.Id).FirstOrDefault();

            List<NotificationPartner> partners = (from p in _context.NotificationPartner
                                                  where p.EmployeId == employeeId
                                                  select p).ToList();

            List<string> partnerIdAlreadyAdded = new List<string>();
            List<NotificationDto> notifications = new List<NotificationDto>();

            foreach (var partner in partners)
            {
                Notification notification = (from p in _context.Notification
                                             where p.Id == partner.NotificationId
                                             select p).FirstOrDefault();

                if (!partnerIdAlreadyAdded.Contains(notification.NotificationSapId))
                {                    
                    notifications.Add(CreateNotificationDto(notification));
                    partnerIdAlreadyAdded.Add(notification.NotificationSapId);
                }
            }

            return notifications;
        }

        [HttpGet("Refresh", Name = "RefreshNotifications")]
        public void RefreshNotifications()
        {
            IEnumerable<NotificationSAP> notifications = new List<NotificationSAP>();
            notifications = GetNotificationsSAP().Result;

            List<String> ExistingNotificationsInDatabase = new List<String>();
            ExistingNotificationsInDatabase = (from p in _context.Notification select p.NotificationSapId).ToList();            
            
            foreach (NotificationSAP notification in notifications)
            {
                Notification notificationEntitity = new Notification();
                try
                {
                    notificationEntitity = CreateNotification(notification);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                if (notificationEntitity.Description == "PPM-Project Portfolio Management netflix")
                {
                    System.Console.WriteLine("Ici");
                }
                if (!VerifyIfNotificationAlreadyExistsInDataBase(notificationEntitity))
                {
                    _context.Notification.Add(notificationEntitity);
                    if (notification.Partners.Any())
                    {
                        foreach (Partner partner in notification.Partners)
                        {
                            try
                            {
                                NotificationPartner partnerEntity = CreatePartner(partner, notificationEntitity);
                                _context.NotificationPartner.Add(partnerEntity);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (VerifyIfNotificationAsBeenModified(notificationEntitity))
                    {
                        updateNotificationInDatabase(notificationEntitity);
                    }
                    VerifyIfNotificationPartnerAsBeenModified(notification);
                }
                ExistingNotificationsInDatabase.Remove(notificationEntitity.NotificationSapId);

                if (notification.Tasks.Any())
                {
                    updateNotificationTasks(notification.Tasks, notificationEntitity);                    
                }                 
            }

            deleteUnexistingNotificationInSAP(ExistingNotificationsInDatabase);
            
            _context.SaveChanges();
        }
    }
}
