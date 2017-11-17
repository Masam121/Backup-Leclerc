using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using ProjectDashboardAPI.Services;
using ProjectDashboardAPI.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly netflix_prContext _context;
        private readonly IProjectMappingService _projectMappingService;
        private readonly IProjectCardMappingService _projectCardMappingService;

        public ProjectRepository(netflix_prContext context, IProjectMappingService projectMappingService, IProjectCardMappingService projectCardMappingService)
        {
            _context = context;
            _projectMappingService = projectMappingService ?? throw new ArgumentNullException(nameof(projectMappingService));
            _projectCardMappingService = projectCardMappingService ?? throw new ArgumentNullException(nameof(projectCardMappingService));
        }

        public Task<Project> CreateProject(ProjectSAP projectSAP, Budget budget)
        {
            var project = _projectMappingService.Map(projectSAP);
            project.Budget = budget;

            return System.Threading.Tasks.Task.FromResult(project);            
        }

        public Task<IEnumerable<Project>> ReadManyAsync()
        {
            IEnumerable<Project> projects = (from p in _context.Project select p).ToList();
            return System.Threading.Tasks.Task.FromResult(projects);
        }

        public Task<ProjectNetflixCard> CreateProjectNetflixCard(Project project)
        {
            ProjectNetflixCard project_netxlix_card = _projectCardMappingService.Map(project);

            return System.Threading.Tasks.Task.FromResult(project_netxlix_card);
        }

        public Task<Project> ReadOneAsyncBySAPId(long id)
        {
            var project = _context.Project.FirstOrDefault(t => t.ProjectSapId == id.ToString());
            return System.Threading.Tasks.Task.FromResult(project);
        }

        public Task<Project> ReadOneAsyncById(long id)
        {
            var project = _context.Project.FirstOrDefault(t => t.Id == id);
            return System.Threading.Tasks.Task.FromResult(project);
        }

        public Task<ProjectNetflix> CreateProjectNetflix(Project project)
        {        
            ProjectNetflix project_netxlix = _projectMappingService.Map(project);

            return System.Threading.Tasks.Task.FromResult(project_netxlix);
        }

        public Task<IEnumerable<Notification>> ReadManyAsyncNotificationByProjectId(long id)
        {
            int? projectId = (from p in _context.Project
                              where p.ProjectSapId == id.ToString()
                              select p.Id).FirstOrDefault();

            IEnumerable<Notification> projectNotificaitions = (from p in _context.Notification
                                                        where p.ProjectId == projectId
                                                        select p).ToList();

            return System.Threading.Tasks.Task.FromResult(projectNotificaitions);
        }

        public Task<IEnumerable<Project>> ReadManyAsyncByFacilityId(string id)
        {
            IEnumerable<Project> facilityProjects = (from p in _context.Project
                                              where p.Factory.Substring(0, 4) == id
                                              select p).ToList();
            return System.Threading.Tasks.Task.FromResult(facilityProjects);
        }

        public Task<IEnumerable<string>> ReadManyAsyncProjectSAPId()
        {
            IEnumerable<string> facilityProjects = (from p in _context.Project
                                                     select p.ProjectSapId).ToList();

            return System.Threading.Tasks.Task.FromResult(facilityProjects);
        }

        public Task<bool> VerifiyIfProjectExists(Project project)
        {
            Project ProjectExists = _context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);
            if(ProjectExists == null)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        public Task<bool> VerifiyIfProjectAsBeenUpdated(Project project)
        {
            Project ProjectExists = _context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);

            if (
                    project.ProjectOwnerId == ProjectExists.ProjectOwnerId &&
                    project.ProjectManagerId == ProjectExists.ProjectManagerId &&
                    project.ProjectName == ProjectExists.ProjectName &&
                    project.Department == ProjectExists.Department &&
                    project.Factory == ProjectExists.Factory &&
                    project.StartDate == ProjectExists.StartDate &&
                    project.EstEndDate == ProjectExists.EstEndDate &&
                    project.ProjectStatus == ProjectExists.ProjectStatus &&
                    project.ProjectsClient == ProjectExists.ProjectsClient &&
                    project.Thumbnail == ProjectExists.Thumbnail &&
                    project.Priority == ProjectExists.Priority &&
                    project.Description == ProjectExists.Description &&
                    project.EstWorkDay == ProjectExists.EstWorkDay)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        public void UpdateProject(Project project)
        {
            Project ProjectExists = _context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);

            ProjectExists.ProjectOwnerId = project.ProjectOwnerId;
            ProjectExists.ProjectManagerId = project.ProjectManagerId;
            ProjectExists.ProjectName = project.ProjectName;
            ProjectExists.Department = project.Department;
            ProjectExists.Factory = project.Factory;
            ProjectExists.StartDate = project.StartDate;
            ProjectExists.EstEndDate = project.EstEndDate;
            ProjectExists.ProjectStatus = project.ProjectStatus;
            ProjectExists.ProjectsClient = project.ProjectsClient;
            ProjectExists.Thumbnail = project.Thumbnail;
            ProjectExists.Priority = project.Priority;
            ProjectExists.Description = project.Description;

            _context.Project.Update(ProjectExists);
        }

        public void SaveProject(Project project)
        {
            _context.Project.Add(project);
        }

        public void DeleteProject(Project project)
        {
            _context.Project.Remove(project);           
        }

        public void SaveData()
        {
            _context.SaveChanges();
        }
    }
}
