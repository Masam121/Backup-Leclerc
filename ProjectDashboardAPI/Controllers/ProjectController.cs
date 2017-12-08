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

        [HttpGet]
        public async Task<IEnumerable<Models.ProjectNetflixCard>> Get()
        {
            IEnumerable<ProjectNetflixCard> projects = await _projectService.GetAllProjectNetflixCard();
           
            return projects;
        }

        [HttpGet("{id}", Name = "GetProject")]
        public async Task<IActionResult> GetById(long id)
        {
            ProjectNetflix project = await _projectService.GetProjectById(id.ToString());
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);           
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
        public async Task<IEnumerable<Models.ProjectNetflix>> GetFilteredProject(string facility, string axeOrganisationnal, string department)
        {
            IEnumerable<ProjectNetflix> netflixProjectsFiltred = await _projectService.GetFiltredProject(facility, axeOrganisationnal, department);
           
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
