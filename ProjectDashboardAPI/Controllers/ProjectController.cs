using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Services;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }     

        [HttpGet("test")]
        public async Task<IActionResult> GetTest()
        {
            ProjectSAP project = new ProjectSAP();
            Budget budget = new Budget();
            var p = await _projectService.CreateProject(project, budget);
            return Ok(p);
        }

        [HttpGet]
        public async Task<IEnumerable<Models.ProjectNetflixCard>> Get()
        {
            IEnumerable<ProjectNetflixCard> projects = await _projectService.GetAllProjectNetflixCard();
           
            return projects;
        }

        [HttpGet("{id}", Name = "GetProject")]
        public async Task<IActionResult> GetById(long id)
        {
            Project project = await _projectService.GetProjectById(id.ToString());

            if (project == null)
            {
                return NotFound();
            }
            else
            {
                ProjectNetflix project_netxlix = await _projectService.CreateProjectNetflix(project);
                return Ok(project_netxlix);
            }
        }

        [HttpGet("{id}/Contributor", Name = "GetByProjectIdContributors")]
        public async Task<IActionResult> GetProjectContributor(long id)
        {
            IEnumerable<ProjectNetflixContributor> projectContributor = await _projectService.GetProjectContributors(id);

            return Ok(projectContributor);                       
        }

        [HttpGet("employee/{id}", Name = "GetByEmployeeIdAllWorkingProjects")]
        public async Task<IEnumerable<ProjectNetflixCard>> GetEmployeeAllWorkingProjects(string id)
        {
            IEnumerable<ProjectNetflixCard> employeeProjects= await _projectService.GetEmployeeAllWorkingProjects(id);

            return employeeProjects;
        }

        /*[HttpGet("Exceeding-budget", Name = "GetExceedingBudgetProject")]
        public IEnumerable<Models.ProjectNetflixCard> GetExceedingBudgetProject()
        {
            List<ProjectNetflixCard> exceedingBudgetProjects = new List<ProjectNetflixCard>();

            exceedingBudgetProjects = await _projectService.GetAllExceedingBudgetProjects();

            return exceedingBudgetProjects;
        }*/

        [HttpGet("Overdue", Name = "GetOverdueProject")]
        public async Task<IActionResult> GetOverdueProject()
        {
            IEnumerable<ProjectNetflixCard> projects = await _projectService.GetOverdueProjects();

            return Ok(projects);           
        }

        [HttpGet("Filter", Name = "GetFilteredProject")]
        public async Task<IEnumerable<Models.ProjectNetflix>> GetFilteredProject(string facility, string sorting, string department)
        {
            IEnumerable<ProjectNetflix> netflixProjectsFiltred = await _projectService.GetFiltredProject(facility, sorting, department);
           
            return netflixProjectsFiltred;
        }

        [HttpGet("Refresh", Name = "RefreshProjects")]
        public async Task<IActionResult> RefreshProjects()
        {
            try
            {
                var response = await _projectService.RefreshProjectsData();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
