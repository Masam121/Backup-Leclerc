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
        private readonly INotificationPartnerMappingService _notificationPartnerMappingService;

        public NotificationPartnerRepository(INotificationPartnerMappingService notificationPartnerMappingService)
        {
            _notificationPartnerMappingService = notificationPartnerMappingService;
        }

        public void AddPartner(netflix_prContext context, NotificationPartner partner)
        {
            context.NotificationPartner.Add(partner);
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

        public bool VerifyIfPartnerAsBeenModified(NotificationPartner partner, Partner partnerSAP)
        {
            var ActualEffort = (double?)double.Parse(partnerSAP.ActualEffort, System.Globalization.CultureInfo.InvariantCulture);
            var EstEffort = (double?)double.Parse(partnerSAP.EstimatedEffort, System.Globalization.CultureInfo.InvariantCulture);
            if (partner.actualEffort == ActualEffort) ;
            if (partner.actualEffort == ActualEffort && partner.EstEffort == EstEffort)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Task<NotificationPartner> CreateNotificationPartnerEntity(netflix_prContext context, Partner partner, Notification notification)
        {
            Tuple<Partner, Notification> partnerInfo = Tuple.Create(partner, notification);
            return System.Threading.Tasks.Task.FromResult(_notificationPartnerMappingService.Map(context, partnerInfo));
        }

        public Task<NotificationPartner> ReadOneNotificationPartnerByConcatenatedId(netflix_prContext context, string concatenatedId)
        {
            var partner = (from p in context.NotificationPartner
                        where p.ConcatenatedId == concatenatedId
                        select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(partner);
        }

        public Task<IEnumerable<PartnerDto>> CreateNotificationPartnersDto(netflix_prContext context, Notification notification)
        {
            var notificationPartners = (from p in context.NotificationPartner
                                        where p.NotificationId == notification.Id
                                        join e in context.Employe on p.EmployeId equals e.Id
                                        join r in context.Role on p.RoleId equals r.Id
                                        select new { notificationPartner = p, employee = e, role = r }).ToList();

            List<PartnerDto> partners = new List<PartnerDto>();

            foreach (var partner in notificationPartners)
            {
                Tuple<Employe, Role, NotificationPartner> tuple = Tuple.Create(partner.employee, partner.role, partner.notificationPartner);
                partners.Add(_notificationPartnerMappingService.Map(context, tuple));
            }
            return System.Threading.Tasks.Task.FromResult(partners.AsEnumerable());            
        }

        public Task<ProjectNetflixContributor> CreateProjectNetflixContributor(netflix_prContext context, NotificationPartner partner)
        {
            int totalEffort = 0;
            var potentielDaysOfWork = 0;
            DateTime latestDate = DateTime.Today;
            DateTime StartDate = DateTime.Today;

            var employee = (from p in context.Employe
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

        public Task<List<NotificationPartner>> ReadManyPartnersByNotification(netflix_prContext context, Notification notification)
        {
            IEnumerable<NotificationPartner> notificaitionPartners = (from p in context.NotificationPartner
                                                               where p.NotificationId == notification.Id
                                                               select p).ToList();

            return System.Threading.Tasks.Task.FromResult(notificaitionPartners.ToList());
        }

        public Task<List<NotificationPartner>> ReadAsyncPartnerByEmployeeId(netflix_prContext context, int id)
        {
            List<NotificationPartner> partners = (from p in context.NotificationPartner
                                                  where p.EmployeId == id
                                                  select p).ToList();

            return System.Threading.Tasks.Task.FromResult(partners);
        }

        public void UpdatePartner(netflix_prContext context, NotificationSAP notification)
        {
            Notification n = (from p in context.Notification
                              where p.NotificationSapId == TrimZerosFromSAPId(notification.NotificationSapId)
                              select p).FirstOrDefault();

            Dictionary<string, NotificationPartner> partners = (from p in context.NotificationPartner
                                                                where p.NotificationId == n.Id
                                                                select p).ToDictionary(p => p.ConcatenatedId, p => p);

            var listOfPartnerTobeDeleted = (from p in context.NotificationPartner
                                            where p.NotificationId == n.Id
                                            select p).ToDictionary(p => p.ConcatenatedId, p => p);

            List<Partner> listOfPartnerTobeAdded = new List<Partner>();

            foreach (var partner in notification.Partners)
            {
                int employeeId = (from p in context.Employe
                                  where p.IdSAP == TrimZerosFromSAPId(partner.EmployeId)
                                  select p.Id).FirstOrDefault();

                int roleId = (from p in context.Role
                              where p.RoleSigle == partner.Role
                              select p.Id).FirstOrDefault();

                string concatenatedId = CreatePartnerConcatenatedId(n.NotificationSapId, employeeId, roleId);

                NotificationPartner NP = ReadOneNotificationPartnerByConcatenatedId(context, concatenatedId).Result;

                if(NP != null)
                {
                    NP.actualEffort = double.Parse(partner.ActualEffort, System.Globalization.CultureInfo.InvariantCulture);
                    NP.EstEffort = double.Parse(partner.EstimatedEffort, System.Globalization.CultureInfo.InvariantCulture);
                    context.NotificationPartner.Update(NP);
                }             

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
                    context.NotificationPartner.Remove(partner);
                }
            }
            if (listOfPartnerTobeAdded.Any())
            {
                foreach (Partner partner in listOfPartnerTobeAdded)
                {
                    try
                    {                        
                        var p = _notificationPartnerMappingService.Map(context, Tuple.Create(partner, n));
                        AddPartner(context, p);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                }
            }
        }

        public void DeletePartner(netflix_prContext context, NotificationPartner partner)
        {
            context.Remove(partner);
        }

        public Task<bool> VerifyIfPartnerAlreadyExistsByConcatenatedId(netflix_prContext context, string concatenatedId)
        {
            NotificationPartner partner = (from p in context.NotificationPartner
                                                  where p.ConcatenatedId == concatenatedId
                                                  select p).FirstOrDefault();

            if(partner != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
        }

        public Task<NotificationPartner> ReadOneAsyncPartnerByConcatenatedId(netflix_prContext context, string concatenatedId)
        {
            NotificationPartner partner = (from p in context.NotificationPartner
                                           where p.ConcatenatedId == concatenatedId
                                           select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(partner);
        }

        public void Update(netflix_prContext context, NotificationPartner partner)
        {
            context.NotificationPartner.Update(partner);
        }
    }
}
