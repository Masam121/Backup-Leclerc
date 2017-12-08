using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateProject(netflix_prContext context, ProjectSAP projectSAP, Budget budget);
        Task<ProjectNetflixCard> CreateProjectNetflixCard(netflix_prContext context, Project project);
        Task<IEnumerable<Project>> ReadManyAsync(netflix_prContext context);
        Task<Project> ReadOneAsyncBySAPId(netflix_prContext context, string id);
        Task<Project> ReadOneAsyncById(netflix_prContext context, long id);
        Task<ProjectNetflix> CreateProjectNetflix(netflix_prContext context, Project project);
        Task<IEnumerable<Notification>> ReadManyAsyncNotificationByProjectId(netflix_prContext context, long id);
        Task<IEnumerable<Project>> ReadManyAsyncByFacilityId(netflix_prContext context, string id);
        Task<List<string>> ReadManyAsyncProjectSAPId(netflix_prContext context);
        Task<Boolean> VerifiyIfProjectExists(netflix_prContext context, Project project);
        Task<Boolean> VerifiyIfProjectAsBeenUpdated(netflix_prContext context, Project project);
        void UpdateProject(netflix_prContext context, Project project);
        void AddProject(netflix_prContext context, Project project);
        void DeleteProject(netflix_prContext context, Project project);
        void SaveData(netflix_prContext context);
    }
}
