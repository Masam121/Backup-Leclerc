using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectDashboardAPI;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployeController : Controller
    {
        private readonly netflix_prContext _context;
        public EmployeController(netflix_prContext context)
        {
            _context = context;
        }

        protected ProjectNetflixCard CreateNetflixProjectCard(Project project)
        {
            ProjectNetflixCard project_netxlix_card = new ProjectNetflixCard();
            var manager = (from p in _context.Employe
                           where p.Id == project.ProjectManagerId
                           select p).FirstOrDefault();

            if (manager == null)
            {
                project_netxlix_card.ManagerName = "Unknown";
                project_netxlix_card.ManagerPicture = "http://www.getsmartcontent.com/content/uploads/2014/08/shutterstock_149293433.jpg";
            }
            else
            {
                project_netxlix_card.ManagerName = manager.Name;
                project_netxlix_card.ManagerPicture = manager.Picture;
            }

            project_netxlix_card.Id = project.Id;
            project_netxlix_card.ProjectManagerId = project.ProjectManagerId;
            project_netxlix_card.ProjectName = project.ProjectName;
            project_netxlix_card.ProjectOwnerId = project.ProjectOwnerId;
            project_netxlix_card.ProjectSapId = project.ProjectSapId;
            project_netxlix_card.ProjectsClient = project.ProjectsClient;
            project_netxlix_card.ProjectStatus = project.ProjectStatus;
            project_netxlix_card.StartDate = project.StartDate;
            project_netxlix_card.Thumbnail = project.Thumbnail;
            project_netxlix_card.EstEndDate = project.EstEndDate != null ? project.EstEndDate.Value.ToString("MMMM, yyyy") : "n/a";
            project_netxlix_card.Department = project.Department;
            project_netxlix_card.CompletionPercentage = project.CompletionPercentage;
            project_netxlix_card.Factory = project.Factory;

            return project_netxlix_card;
        }

        [HttpGet]
        public IEnumerable<Employe> Get()
        {
            return _context.Employe.ToList();
        }

        [HttpGet("{id}", Name = "GetEmploye")]
        public IActionResult GetById(string id)
        {
            var employee = _context.Employe.FirstOrDefault(t => t.IdSAP == id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {               
                EmployeeNetflixDetail employee_detail_netflix = new EmployeeNetflixDetail()
                {
                    Id = employee.Id,
                    Department = employee.Department,
                    Factory = employee.Factory,
                    HiredDate = employee.HiredDate.ToString("MMMM, yyyy"),
                    LeclercEmail = employee.LeclercEmail,
                    Name = employee.Name,
                    Picture = employee.Picture,
                    O365Id = employee.O365Id,
                    ProjectWorkRatio = employee.ProjectWorkRatio,
                    SuperiorId = employee.SuperiorId,
                    Title = employee.Title,
                    Workload = employee.Workload,
                    IdSAP = employee.IdSAP
                };
                return new ObjectResult(employee_detail_netflix);
            }

        }

        //    while (StartDate < latestDate)
        //    {
        //        if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
        //        {
        //            ++potentielDaysOfWork;
        //        }
        //        StartDate = StartDate.AddDays(1);
        //    }
        //    var availableTimeForProjects = potentielDaysOfWork * (employee.Workload / 5) * employee.ProjectWorkRatio / 100;
        //    var timeAvailable = availableTimeForProjects - totalEffort;
        //    //var occupancyRate = totalEffort / availableTimeForProjects * 100;

        //    ProjectNetflixTaskStatusEffort TimeAvailable = new ProjectNetflixTaskStatusEffort() { Name = "Time Available", Effort = timeAvailable };

        //    task_serie_netflix.Add(lateSerie);
        //    task_serie_netflix.Add(notstartedSerie);
        //    task_serie_netflix.Add(inProgressSerie);
        //    task_serie_netflix.Add(completedSerie);

        //    task_effort_netflix.Add(lateEffort);
        //    task_effort_netflix.Add(notStartedEffort);
        //    task_effort_netflix.Add(inProgressEffort);
        //    task_effort_netflix.Add(TimeAvailable);

        [HttpGet("{id}/ratio", Name = "GetEmployeRatio")]
        public IActionResult GetEmployeeRatio(long id)
        {
            var employee = _context.Employe.FirstOrDefault(t => t.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                return new ObjectResult(employee.ProjectWorkRatio);
            }
        }

        [HttpGet("Project-manager", Name = "GetProjectManager")]
        public IEnumerable<Employe> GetProjectManager()
        {
            var projectManagerIds = (from p in _context.Project
                                     select p.ProjectManagerId).Distinct().ToList();
            List<Employe> projectManagers = new List<Employe>();
            foreach (int? Id in projectManagerIds)
            {
                var projectManager = (from p in _context.Employe
                                      where p.Id == Id
                                      select p).FirstOrDefault();
                if (projectManager == null)
                {
                    continue;
                }
                projectManagers.Add(projectManager);
            }
            return projectManagers;
        } 

        //[HttpGet("Project-manager/Overloaded", Name = "GetOverloadedProjectManager")]
        //public IEnumerable<ProjectNetflixContributor> GetOverloadedProjectManager()
        //{
        //    List<ProjectNetflixContributor> overloadedProjectManagers = new List<ProjectNetflixContributor>();
        //    IEnumerable<Employe> projectManagers = GetProjectManager();

        //    foreach (Employe Manager in projectManagers)
        //    {
        //        int totalEffort = 0;
        //        var potentielDaysOfWork = 0;
        //        DateTime latestDate = DateTime.Today;
        //        DateTime StartDate = DateTime.Today;

        //        var tasks = (from p in _context.TaskOwner
        //                     join t in _context.Task on p.TaskId equals t.Id
        //                     where p.EmployeId == Manager.Id
        //                     select new { taskOwner = p, task = t }).ToList();

        //        foreach (var task in tasks)
        //        {
        //            if (task.task.Status != "Completed")
        //            {
        //                totalEffort = totalEffort + task.task.EstEffort;
        //                if (latestDate < task.task.EstEnd)
        //                {
        //                    latestDate = task.task.EstEnd;
        //                }
        //            }
        //        }

        //        while (StartDate <= latestDate)
        //        {
        //            if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
        //            {
        //                ++potentielDaysOfWork;
        //            }
        //            StartDate = StartDate.AddDays(1);
        //        }
        //        var availableTimeForProjects = potentielDaysOfWork * (Manager.Workload / 5) * Manager.ProjectWorkRatio / 100;
        //        float occupancyRate = Convert.ToSingle(Math.Round((totalEffort / availableTimeForProjects * 100), 2));

        //        if (occupancyRate > 100)
        //        {
        //            ProjectNetflixContributor contributor_netflix = new ProjectNetflixContributor()
        //            {
        //                Id = Manager.Id,
        //                Department = Manager.Department,
        //                Name = Manager.Name,
        //                EmployeeId = Manager.Id,
        //                Picture = Manager.Picture,
        //                Title = Manager.Title,
        //                OccupancyRate = occupancyRate
        //            };
        //            overloadedProjectManagers.Add(contributor_netflix);
        //        }
        //    }
        //    return overloadedProjectManagers;
        //}

        [HttpPost]
        public async Task<IActionResult> PostEmployees()
        {
            IEnumerable<EmployeeSAP> Employees = new List<EmployeeSAP>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = await client.GetAsync(string.Concat("http://api.dev.gbl/v3/", "employees"));
                data.EnsureSuccessStatusCode();
                var stringResult = await data.Content.ReadAsStringAsync();
                Employees = JsonConvert.DeserializeObject<IEnumerable<EmployeeSAP>>(stringResult);

                foreach (EmployeeSAP employeeSAP in Employees)
                {
                    Employe employee = new Employe();
                    employee.Department = employeeSAP.department;
                    employee.Factory = employeeSAP.factory;
                    employee.HiredDate = Convert.ToDateTime(employeeSAP.hiredDate);
                    employee.IdSAP = employeeSAP.id_SAP.TrimStart('0');
                    employee.LeclercEmail = employeeSAP.leclercEmail;
                    employee.Name = employeeSAP.name;
                    employee.O365Id = employeeSAP.o365;
                    employee.Picture = employeeSAP.picture;
                    employee.Workload = Convert.ToInt32(double.Parse(employeeSAP.workload, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
                    employee.Title = employeeSAP.title;
                    //employee.SuperiorId = string.IsNullOrEmpty(employeeSAP.superior) ? (int?)null : int.Parse(employeeSAP.superior);
                    employee.ProjectWorkRatio = 100;

                    Employe employeeExists = _context.Employe.FirstOrDefault(x => x.IdSAP == employee.IdSAP);

                    if (employeeExists != null)
                    {
                        if (employeeExists.Department == employee.Department &&
                            employeeExists.Factory == employee.Factory &&
                            employeeExists.HiredDate == employee.HiredDate &&
                            employeeExists.IdSAP == employee.IdSAP &&
                            employeeExists.LeclercEmail == employee.LeclercEmail &&
                            employeeExists.Name == employee.Name &&
                            employeeExists.O365Id == employee.O365Id &&
                            employeeExists.Picture == employee.Picture &&
                            employeeExists.Workload == employee.Workload &&
                            employeeExists.Title == employee.Title &&
                            //employeeExists.SuperiorId == employee.SuperiorId &&
                            employeeExists.ProjectWorkRatio == employee.ProjectWorkRatio)
                        {
                            continue;
                        }
                        else
                        {
                            employeeExists.Department = employee.Department;
                            employeeExists.Factory = employee.Factory;
                            employeeExists.HiredDate = employee.HiredDate;
                            employeeExists.IdSAP = employee.IdSAP;
                            employeeExists.LeclercEmail = employee.LeclercEmail;
                            employeeExists.Name = employee.Name;
                            employeeExists.O365Id = employee.O365Id;
                            employeeExists.Picture = employee.Picture;
                            employeeExists.Workload = employee.Workload;
                            employeeExists.Title = employee.Title;
                            //employeeExists.SuperiorId = employee.SuperiorId;
                            employeeExists.ProjectWorkRatio = employee.ProjectWorkRatio;
                            _context.Employe.Update(employeeExists);
                            continue;
                        }
                    }
                    else
                    {
                        _context.Add(employee);
                    }
                }
                _context.SaveChanges();
                return new ObjectResult("Successfully added to data base...");
            }
        }

        [HttpPost("{id}/ratio", Name = "GetEmployeRatio")]
        public IActionResult PostEmployeeRatio(long id, string ratio)
        {
            Console.WriteLine(id);
            Console.WriteLine(ratio);
            var employee = _context.Employe.FirstOrDefault(t => t.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            else
            {
                employee.ProjectWorkRatio = Int32.Parse(ratio);
                _context.Employe.Update(employee);
                _context.SaveChanges();
                return new ObjectResult(employee.ProjectWorkRatio);
            }
        }
    }
}
