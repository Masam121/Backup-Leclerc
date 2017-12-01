using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{   
    public interface IProjectService
    {
        Task<Project> CreateProject(ProjectSAP projectSAP, Budget budget);
        Task<IEnumerable<ProjectNetflixCard>> GetAllProjectNetflixCard();
        Task<Project> GetProjectById(string id);
        Task<ProjectNetflix> CreateProjectNetflix(Project project);
        Task<IEnumerable<ProjectNetflixContributor>> GetProjectContributors(long id);
        Task<IEnumerable<ProjectNetflixCard>> GetEmployeeAllWorkingProjects(string id);
        Task<IEnumerable<ProjectNetflixCard>> GetAllExceedingBudgetProjects();
        Task<IEnumerable<ProjectNetflixCard>> GetOverdueProjects();
        Task<IEnumerable<ProjectNetflix>> GetFiltredProject(string facility, string sorting, string department);
        Task<IActionResult> RefreshProjectsData();
    }
}
