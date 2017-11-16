using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Models.Dto;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly netflix_prContext _context;

        public TaskController(netflix_prContext context)
        {
            _context = context;
        }

        protected List<ProjectNetflixTaskStatusEffort> setEffort(List<Task> tasks)
        {
            List<ProjectNetflixTaskStatusEffort> task_effort_netflix = new List<ProjectNetflixTaskStatusEffort>();
            ProjectNetflixTaskStatusEffort lateEffort = new ProjectNetflixTaskStatusEffort() { Name = "Late", Effort = 0 };
            ProjectNetflixTaskStatusEffort notStartedEffort = new ProjectNetflixTaskStatusEffort() { Name = "Not Started", Effort = 0 };
            ProjectNetflixTaskStatusEffort inProgressEffort = new ProjectNetflixTaskStatusEffort() { Name = "In Progress", Effort = 0 };
            ProjectNetflixTaskStatusEffort completedEffort = new ProjectNetflixTaskStatusEffort() { Name = "Completed", Effort = 0 };

            foreach(Task task in tasks)
            {
                switch (task.Status)
                {
                    case "Late":
                        lateEffort.Effort = lateEffort.Effort + task.EstEffort;
                        break;
                    case "Not Started":
                        notStartedEffort.Effort = notStartedEffort.Effort + task.EstEffort;
                        break;
                    case "In Progress":
                        inProgressEffort.Effort = inProgressEffort.Effort + task.EstEffort;
                        break;
                    case "Completed":
                        completedEffort.Effort = completedEffort.Effort + task.EstEffort;
                        break;
                    default:
                        throw new System.ArgumentException("The Task's status is not a valid one. It should be Late, Not Started, In Progess or Completed", "original");
                }
            }
            task_effort_netflix.Add(lateEffort);
            task_effort_netflix.Add(notStartedEffort);
            task_effort_netflix.Add(inProgressEffort);
            task_effort_netflix.Add(completedEffort);

            return task_effort_netflix;
        }

        protected Dictionary<int, int> getAllNotificationPartner(long id)
        {
            Dictionary<int, int> partnerMap = new Dictionary<int, int>();
            int NotificationId = (from p in _context.Notification
                                        where p.NotificationSapId == id.ToString()
                                        select p.Id).FirstOrDefault();

            var partners = (from p in _context.NotificationPartner
                               where p.NotificationId == NotificationId
                               join t in _context.Employe on p.EmployeId equals t.Id
                               select new { partner = p, partnerDetail = t }).ToList();

            List<int> partnerIdAlreadyAdded = new List<int>();
            foreach(var partner in partners)
            {
                if (!partnerIdAlreadyAdded.Contains(partner.partnerDetail.Id))
                {
                    partnerMap.Add(partner.partnerDetail.Id, partnerMap.Count);
                    partnerIdAlreadyAdded.Add(partner.partnerDetail.Id);
                }               
            }
            return partnerMap;
        }

        protected Dictionary<int, int> getAllProjectPartner(long id)
        {
            Dictionary<int, int> partnerMap = new Dictionary<int, int>();

            int projectId = (from p in _context.Project
                             where p.ProjectSapId == id.ToString()
                             select p.Id).FirstOrDefault();

            List<Notification> notifications = _context.Notification.Where(x => x.ProjectId == projectId).ToList();

            List<int> partnerIdAlreadyAdded = new List<int>();
            foreach (Notification notification in notifications)
            {
                var partners = (from p in _context.NotificationPartner
                                where p.NotificationId == notification.Id
                                join t in _context.Employe on p.EmployeId equals t.Id
                                select new { partner = p, partnerDetail = t }).ToList();
                foreach(var partner in partners)
                {
                    if (!partnerIdAlreadyAdded.Contains(partner.partnerDetail.Id))
                    {
                        partnerMap.Add(partner.partnerDetail.Id, partnerMap.Count);
                        partnerIdAlreadyAdded.Add(partner.partnerDetail.Id);
                    }
                }               
            }
            return partnerMap;
        }

        protected List<NetflixSerie> setSerie(Dictionary<int, int> partnerMap, long id)
        {
            List<NetflixSerie> task_serie_netflix = new List<NetflixSerie>();
            NetflixSerie lateSerie = new NetflixSerie() { Name = "Late", Data = new List<int>() };
            NetflixSerie notstartedSerie = new NetflixSerie() { Name = "Not Started", Data = new List<int>() };
            NetflixSerie inProgressSerie = new NetflixSerie() { Name = "In Progress", Data = new List<int>() };
            NetflixSerie completedSerie = new NetflixSerie() { Name = "Completed", Data = new List<int>() };

            int projectId = (from p in _context.Project
                                  where p.ProjectSapId == id.ToString()
                                  select p.Id).FirstOrDefault();
            var notifications = (from p in _context.Notification
                                  where p.ProjectId == projectId
                                  select p.Id).ToList();

            foreach (int ownerId in partnerMap.Keys)
            {
                lateSerie.Data.Add(0);
                notstartedSerie.Data.Add(0);
                inProgressSerie.Data.Add(0);
                completedSerie.Data.Add(0);
            }

            foreach (int notificationId in notifications)
            {
                var tasks = (from p in _context.Task
                             join e in _context.TaskOwner on p.Id equals e.TaskId
                             where p.NotificationId == notificationId
                             select new { task = p, employee = e }).ToList();               

                foreach (var task in tasks)
                {
                    if (partnerMap.ContainsKey(task.employee.EmployeId))
                    {
                        switch (task.task.Status)
                        {
                            case "Late":
                                lateSerie.Data[partnerMap[task.employee.EmployeId]]++;
                                break;
                            case "Not Started":
                                notstartedSerie.Data[partnerMap[task.employee.EmployeId]]++;
                                break;
                            case "In Progress":
                                inProgressSerie.Data[partnerMap[task.employee.EmployeId]]++;
                                break;
                            case "Completed":
                                completedSerie.Data[partnerMap[task.employee.EmployeId]]++;
                                break;
                            default:
                                throw new System.ArgumentException("The Task's status is not a valid one. It should be Late, Not Started, In Progess or Completed", "original");
                        }
                    }
                }
            }
            
            task_serie_netflix.Add(lateSerie);
            task_serie_netflix.Add(notstartedSerie);
            task_serie_netflix.Add(inProgressSerie);
            task_serie_netflix.Add(completedSerie);

            return task_serie_netflix;
        }

        protected List<NetflixSerie> setSerieForOneNotification(Dictionary<int, int> partnerMap, long id)
        {
            List<NetflixSerie> task_serie_netflix = new List<NetflixSerie>();
            NetflixSerie lateSerie = new NetflixSerie() { Name = "Late", Data = new List<int>() };
            NetflixSerie notstartedSerie = new NetflixSerie() { Name = "Not Started", Data = new List<int>() };
            NetflixSerie inProgressSerie = new NetflixSerie() { Name = "In Progress", Data = new List<int>() };
            NetflixSerie completedSerie = new NetflixSerie() { Name = "Completed", Data = new List<int>() };

            var notificationId = (from p in _context.Notification
                                 where p.NotificationSapId == id.ToString()
                                select p.Id).FirstOrDefault();

            foreach (int ownerId in partnerMap.Keys)
            {
                lateSerie.Data.Add(0);
                notstartedSerie.Data.Add(0);
                inProgressSerie.Data.Add(0);
                completedSerie.Data.Add(0);
            }

            var tasks = (from p in _context.Task
                            join e in _context.TaskOwner on p.Id equals e.TaskId
                            where p.NotificationId == notificationId
                            select new { task = p, employee = e }).ToList();

            foreach (var task in tasks)
            {
                if (partnerMap.ContainsKey(task.employee.EmployeId))
                {
                    switch (task.task.Status)
                    {
                        case "Late":
                            lateSerie.Data[partnerMap[task.employee.EmployeId]]++;
                            break;
                        case "Not Started":
                            notstartedSerie.Data[partnerMap[task.employee.EmployeId]]++;
                            break;
                        case "In Progress":
                            inProgressSerie.Data[partnerMap[task.employee.EmployeId]]++;
                            break;
                        case "Completed":
                            completedSerie.Data[partnerMap[task.employee.EmployeId]]++;
                            break;
                        default:
                            throw new System.ArgumentException("The Task's status is not a valid one. It should be Late, Not Started, In Progess or Completed", "original");
                    }
                }
            }

            task_serie_netflix.Add(lateSerie);
            task_serie_netflix.Add(notstartedSerie);
            task_serie_netflix.Add(inProgressSerie);
            task_serie_netflix.Add(completedSerie);

            return task_serie_netflix;
        }

        protected List<NetflixCategory> setCategory(Dictionary<int, int> partnerMap)
        {
            List<NetflixCategory> notificationCategory = new List<NetflixCategory>();

            foreach (int ownerId in partnerMap.Keys)
            {
                Employe employee = (from p in _context.Employe
                                    where p.Id == ownerId
                                    select p).FirstOrDefault();

                NetflixCategory category = new NetflixCategory();
                category.id = employee.IdSAP;
                category.Name = employee.Name;

                notificationCategory.Add(category);
            }
            return notificationCategory;
        }

        protected TaskDto createTaskDto(Task task)
        {
            Employe employee = (from p in _context.TaskOwner
                                join e in _context.Employe on p.EmployeId equals e.Id
                                where p.TaskId == task.Id
                                select e).FirstOrDefault();

            TaskDto taskDto = new TaskDto();

            if (employee == null)
            {
                taskDto.InChargeName = "Not Assigned Yet";
                taskDto.InchargeId = "Not Assigned Yet";
            }
            else
            {
                taskDto.InChargeName = employee.Name;
                taskDto.InchargeId = employee.IdSAP;
            }
            
            taskDto.Type = task.Type;
            taskDto.ActualEffort = task.ActualEffort;
            taskDto.AssignationDate = task.AssignationDate.ToString("yyyy-MM-dd"); ;
            taskDto.EstEffort = task.EstEffort;
            taskDto.EstEnd = task.EstEnd.ToString("yyyy-MM-dd");
            taskDto.IdSAP = task.TaskSAPId;           
            taskDto.Status = task.Status;
            taskDto.Description = task.Description;

            return taskDto;
        }

        [HttpGet]
        public IEnumerable<Task> GetAll()
        {
            return _context.Task.ToList();
        }

        [HttpGet("{id}", Name = "GetTask")]
        public IActionResult GetById(long id)
        {
            var item = _context.Task.FirstOrDefault(t => t.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpGet("Notification/{id}", Name = "GetNotficationByIdTask")]
        public IActionResult GetAllNotificationTasksByNotificationId(long id)
        {

            int notificaitonSAPId = (from p in _context.Notification
                                     where p.NotificationSapId == id.ToString()
                                     select p.Id).FirstOrDefault();
            List<Task> tasks = _context.Task.Where(x => x.NotificationId == notificaitonSAPId).ToList();
            List<TaskDto> tasksDto = new List<TaskDto>();
            if (tasks == null)
            {
                return NotFound();
            }
            foreach(Task task in tasks)
            {
                tasksDto.Add(createTaskDto(task));
            }
            return new ObjectResult(tasksDto);
        }

        [HttpGet("Notification/{id}/effort", Name = "GetByNotificationIdTaskEffortStatus")]
        public IActionResult GetNotificationTaskEffortStatus(long id)
        {
            int notificaitonSAPId = (from p in _context.Notification
                                     where p.NotificationSapId == id.ToString()
                                     select p.Id).FirstOrDefault();
            List<ProjectNetflixTaskStatusEffort> notificationTaskEffortStatus = new List<ProjectNetflixTaskStatusEffort>();
            List<Task> tasks = _context.Task.Where(x => x.NotificationId == notificaitonSAPId).ToList();

            if (tasks == null)
            {
                return NotFound();
            }
            try
            {
                notificationTaskEffortStatus = setEffort(tasks);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }               
           
            return new ObjectResult(notificationTaskEffortStatus);
        }

        [HttpGet("Notification/{id}/distribution", Name = "GetByNotificationIdTaskDistribution")]
        public IActionResult GetByNotificationIdTaskDistribution(long id)
        {
            TaskDistribution distribution = new TaskDistribution();
            Dictionary<int, int> partnerMap = new Dictionary<int, int>();

            partnerMap = getAllNotificationPartner(id);
            distribution.Categories = setCategory(partnerMap);
            distribution.Series = setSerieForOneNotification(partnerMap, id);

            return new ObjectResult(distribution);
        }

        [HttpGet("Project/{id}/effort", Name = "GetByProjectIdTaskEffortStatus")]
        public IActionResult GetByProjectIdTaskEffortStatus(long id)
        {
            int projectId = (from p in _context.Project
                             where p.ProjectSapId == id.ToString()
                             select p.Id).FirstOrDefault();

            List<Notification> notifications = _context.Notification.Where(x => x.ProjectId == projectId).ToList();
            List<Task> tasks = new List<Task>();
            foreach(Notification notification in notifications)
            {
                List<Task> notificationTasks = _context.Task.Where(x => x.NotificationId == notification.Id).ToList();
                tasks.AddRange(notificationTasks);
            }
            List <ProjectNetflixTaskStatusEffort> notificationTaskEffortStatus = new List<ProjectNetflixTaskStatusEffort>();

            if (tasks == null)
            {
                return NotFound();
            }
            try
            {
                notificationTaskEffortStatus = setEffort(tasks);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }

            return new ObjectResult(notificationTaskEffortStatus);
        }

        [HttpGet("Project/{id}/distribution", Name = "GetByProjectIdTaskDistribution")]
        public IActionResult GetByProjectIdTaskDistribution(long id)
        {
            TaskDistribution distribution = new TaskDistribution();
            Dictionary<int, int> partnerMap = new Dictionary<int, int>();

            partnerMap = getAllProjectPartner(id);
            distribution.Categories = setCategory(partnerMap);
            distribution.Series = setSerie(partnerMap, id);

            return new ObjectResult(distribution);
        }
    }
}
