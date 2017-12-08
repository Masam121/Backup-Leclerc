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
        private readonly IProjectMappingService _projectMappingService;
        private readonly IProjectCardMappingService _projectCardMappingService;

        public ProjectRepository(IProjectMappingService projectMappingService, IProjectCardMappingService projectCardMappingService)
        {
            _projectMappingService = projectMappingService ?? throw new ArgumentNullException(nameof(projectMappingService));
            _projectCardMappingService = projectCardMappingService ?? throw new ArgumentNullException(nameof(projectCardMappingService));
        }

        public Task<Project> CreateProject(netflix_prContext context, ProjectSAP projectSAP, Budget budget)
        {
            var project = _projectMappingService.Map(context, projectSAP);
            project.Budget = budget;

            return System.Threading.Tasks.Task.FromResult(project);            
        }

        public Task<IEnumerable<Project>> ReadManyAsync(netflix_prContext context)
        {
            IEnumerable<Project> projects = (from p in context.Project select p).ToList();
            return System.Threading.Tasks.Task.FromResult(projects);
        }

        public Task<ProjectNetflixCard> CreateProjectNetflixCard(netflix_prContext context, Project project)
        {
            ProjectNetflixCard project_netxlix_card = _projectCardMappingService.Map(context, project);

            return System.Threading.Tasks.Task.FromResult(project_netxlix_card);
        }

        public Task<Project> ReadOneAsyncBySAPId(netflix_prContext context, string id)
        {
            var project = context.Project.FirstOrDefault(t => t.ProjectSapId == id);
            return System.Threading.Tasks.Task.FromResult(project);
        }

        public Task<Project> ReadOneAsyncById(netflix_prContext context, long id)
        {
            var project = context.Project.FirstOrDefault(t => t.Id == id);
            return System.Threading.Tasks.Task.FromResult(project);
        }

        public Task<ProjectNetflix> CreateProjectNetflix(netflix_prContext context, Project project)
        {        
            ProjectNetflix project_netxlix = _projectMappingService.Map(context, project);

            return System.Threading.Tasks.Task.FromResult(project_netxlix);
        }

        public Task<IEnumerable<Notification>> ReadManyAsyncNotificationByProjectId(netflix_prContext context, long id)
        {
            int? projectId = (from p in context.Project
                              where p.ProjectSapId == id.ToString()
                              select p.Id).FirstOrDefault();

            IEnumerable<Notification> projectNotificaitions = (from p in context.Notification
                                                        where p.ProjectId == projectId
                                                        select p).ToList();

            return System.Threading.Tasks.Task.FromResult(projectNotificaitions);
        }

        public Task<IEnumerable<Project>> ReadManyAsyncByFacilityId(netflix_prContext context, string id)
        {
            IEnumerable<Project> facilityProjects = (from p in context.Project
                                              where p.Factory.Substring(0, 4) == id
                                              select p).ToList();
            return System.Threading.Tasks.Task.FromResult(facilityProjects);
        }

        public Task<List<string>> ReadManyAsyncProjectSAPId(netflix_prContext context)
        {
            List<string> ProjectsSAPIds = (from p in context.Project
                                           select p.ProjectSapId).ToList();

            return System.Threading.Tasks.Task.FromResult(ProjectsSAPIds);
        }

        public Task<bool> VerifiyIfProjectExists(netflix_prContext context, Project project)
        {
            Project ProjectExists = context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);
            if(ProjectExists == null)
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        public Task<bool> VerifiyIfProjectAsBeenUpdated(netflix_prContext context, Project project)
        {
            Project ProjectExists = context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);

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

        public void UpdateProject(netflix_prContext context, Project project)
        {
            Project ProjectExists = context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);

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

            context.Project.Update(ProjectExists);
        }

        public void AddProject(netflix_prContext context, Project project)
        {
            context.Project.Add(project);
        }

        public void DeleteProject(netflix_prContext context, Project project)
        {
            context.Project.Remove(project);           
        }

        public void SaveData(netflix_prContext context)
        {
            context.SaveChanges();
        }
    }
}
