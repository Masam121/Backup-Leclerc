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
using ProjectDashboardAPI.Services;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeController(netflix_prContext context, IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }      

        [HttpGet("{id}", Name = "GetEmploye")]
        public async Task<IActionResult> GetById(string id)
        {
            EmployeeNetflixDetail employee = await _employeeService.GetEmployeeById(id);

            return Ok(employee);
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

        //[HttpGet("{id}/ratio", Name = "GetEmployeRatio")]
        //public IActionResult GetEmployeeRatio(long id)
        //{
        //    var employee = _context.Employe.FirstOrDefault(t => t.Id == id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        return new ObjectResult(employee.ProjectWorkRatio);
        //    }
        //}

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

        [HttpGet("Refresh", Name = "RefreshEmployees")]
        public async Task<IActionResult> RefreshEmployees()
        {
            try
            {
                var response = await _employeeService.RefreshEmployee();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        //[HttpPost("{id}/ratio", Name = "GetEmployeRatio")]
        //public IActionResult PostEmployeeRatio(long id, string ratio)
        //{
        //    Console.WriteLine(id);
        //    Console.WriteLine(ratio);
        //    var employee = _context.Employe.FirstOrDefault(t => t.Id == id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        employee.ProjectWorkRatio = Int32.Parse(ratio);
        //        _context.Employe.Update(employee);
        //        _context.SaveChanges();
        //        return new ObjectResult(employee.ProjectWorkRatio);
        //    }
        //}
    }
}
