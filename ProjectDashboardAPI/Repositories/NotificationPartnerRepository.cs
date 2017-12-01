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

        public void AddPartner(NotificationPartner partner)
        {
            _context.NotificationPartner.Add(partner);
        }

        public string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        public string CreateTaskConcatenatedId(string notificationSAPId, string taskKey)
        {
            string concatenatedId = notificationSAPId + taskKey;
            return concatenatedId;
        }

        public string CreatePartnerConcatenatedId(string notificationSAPId, int employeeId, int roleId)
        {
            string s_employeeId = employeeId.ToString();
            string s_roleId = roleId.ToString();

            string concatenatedId = notificationSAPId + s_employeeId + s_roleId;
            return concatenatedId;
        }

        public Task<NotificationPartner> CreateNotificationPartnerEntity(Partner partner, Notification notification)
        {
            Tuple<Partner, Notification> partnerInfo = Tuple.Create(partner, notification);
            return System.Threading.Tasks.Task.FromResult(_notificationPartnerMappingService.Map(partnerInfo));
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

        public Task<List<NotificationPartner>> ReadManyPartnersByNotification(Notification notification)
        {
            IEnumerable<NotificationPartner> notificaitionPartners = (from p in _context.NotificationPartner
                                                               where p.NotificationId == notification.Id
                                                               select p).ToList();

            return System.Threading.Tasks.Task.FromResult(notificaitionPartners.ToList());
        }

        public Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(int id)
        {
            List<NotificationPartner> partners = (from p in _context.NotificationPartner
                                                  where p.EmployeId == id
                                                  select p).ToList();

            return System.Threading.Tasks.Task.FromResult(partners);
        }

        public void UpdatePartner(NotificationSAP notification)
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
                        var p = _notificationPartnerMappingService.Map(Tuple.Create(partner, n));
                        AddPartner(p);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                }
            }
        }

        public void DeletePartner(NotificationPartner partner)
        {
            _context.Remove(partner);
        }
    }
}
