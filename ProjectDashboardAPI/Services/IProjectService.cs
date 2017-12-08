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
        Task<IEnumerable<ProjectNetflixCard>> GetAllProjectNetflixCard();
        Task<ProjectNetflix> GetProjectById(string id);
        Task<IEnumerable<ProjectNetflixContributor>> GetProjectContributors(long id);
        Task<IEnumerable<ProjectNetflixCard>> GetEmployeeAllWorkingProjects(string id);
        Task<IEnumerable<ProjectNetflixCard>> GetAllExceedingBudgetProjects();
        Task<IEnumerable<ProjectNetflixCard>> GetOverdueProjects();
        Task<IEnumerable<ProjectNetflix>> GetFiltredProject(string facility, string sorting, string department);
        Task<IActionResult> RefreshProjectsData();
    }
}
