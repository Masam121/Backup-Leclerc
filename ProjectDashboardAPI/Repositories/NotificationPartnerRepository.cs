using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Services.Mapping;

namespace ProjectDashboardAPI.Repositories
{
    public class NotificationPartnerRepository : INotificationPartnerRepository
    {
        private readonly netflix_prContext _context;
        private readonly INotificationPartnerMappingService _notificationPartnerMappingService;

        public NotificationPartnerRepository(netflix_prContext context, INotificationPartnerMappingService notificationPartnerMappingService)
        {
            _context = context;
            _notificationPartnerMappingService = notificationPartnerMappingService;
        }

        public Task<IEnumerable<PartnerDto>> CreateNotificationPartnersDto(Notification notification)
        {
            var notificationPartners = (from p in _context.NotificationPartner
                                        where p.NotificationId == notification.Id
                                        join e in _context.Employe on p.EmployeId equals e.Id
                                        join r in _context.Role on p.RoleId equals r.Id
                                        select new { notificationPartner = p, employee = e, role = r }).ToList();

            List<PartnerDto> partners = new List<PartnerDto>();

            foreach (var partner in notificationPartners)
            {
                Tuple<Employe, Role> tuple = Tuple.Create(partner.employee, partner.role);
                partners.Add(_notificationPartnerMappingService.Map(tuple));
            }
            return System.Threading.Tasks.Task.FromResult(partners.AsEnumerable());            
        }

        public Task<ProjectNetflixContributor> CreateProjectNetflixContributor(NotificationPartner partner)
        {
            int totalEffort = 0;
            var potentielDaysOfWork = 0;
            DateTime latestDate = DateTime.Today;
            DateTime StartDate = DateTime.Today;

            var employee = (from p in _context.Employe
                            where p.Id == partner.EmployeId
                            select p).First();

            //var tasks = (from p in _context.TaskOwner
            //             join t in _context.Task on p.TaskId equals t.Id
            //             where p.EmployeId == partner.EmployeId
            //             select new { taskOwner = p, task = t }).ToList();

            //foreach (var task in tasks)
            //{
            //    if (task.task.Status != "Completed")
            //    {
            //        totalEffort = totalEffort + task.task.EstEffort;
            //        if (latestDate < task.task.EstEnd)
            //        {
            //            latestDate = task.task.EstEnd;
            //        }
            //    }
            //}

            //while (StartDate <= latestDate)
            //{
            //    if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
            //    {
            //        ++potentielDaysOfWork;
            //    }
            //    StartDate = StartDate.AddDays(1);
            //}
            //var availableTimeForProjects = potentielDaysOfWork * (employee.Workload / 5) * employee.ProjectWorkRatio / 100;
            //var occupancyRate = totalEffort / availableTimeForProjects * 100;

            ProjectNetflixContributor contributor_netflix = new ProjectNetflixContributor()
            {
                Id = partner.Id,
                Department = employee.Department,
                Name = employee.Name,
                EmployeeId = employee.IdSAP,
                Picture = employee.Picture,
                Title = employee.Title,
                OccupancyRate = 0
            };
            return System.Threading.Tasks.Task.FromResult(contributor_netflix);
        }

        public Task<IEnumerable<NotificationPartner>> ReadAllPartnersFromNotification(Notification notification)
        {
            IEnumerable<NotificationPartner> notificaitionPartners = (from p in _context.NotificationPartner
                                                               where p.NotificationId == notification.Id
                                                               select p).ToList();

            return System.Threading.Tasks.Task.FromResult(notificaitionPartners);
        }

        public Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(int id)
        {
            List<NotificationPartner> partners = (from p in _context.NotificationPartner
                                                  where p.EmployeId == id
                                                  select p).ToList();

            return System.Threading.Tasks.Task.FromResult(partners);
        }
    }
}
