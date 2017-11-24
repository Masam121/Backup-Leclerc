using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IProjectRepository
    {
        Task<Project> CreateProject(ProjectSAP projectSAP, Budget budget);
        Task<ProjectNetflixCard> CreateProjectNetflixCard(Project project);
        Task<IEnumerable<Project>> ReadManyAsync();
        Task<Project> ReadOneAsyncBySAPId(long id);
        Task<Project> ReadOneAsyncById(long id);
        Task<ProjectNetflix> CreateProjectNetflix(Project project);
        Task<IEnumerable<Notification>> ReadManyAsyncNotificationByProjectId(long id);
        Task<IEnumerable<Project>> ReadManyAsyncByFacilityId(string id);
        Task<List<string>> ReadManyAsyncProjectSAPId();
        Task<Boolean> VerifiyIfProjectExists(Project project);
        Task<Boolean> VerifiyIfProjectAsBeenUpdated(Project project);
        void UpdateProject(Project project);
        void AddProject(Project project);
        void DeleteProject(Project project);
        void SaveData();
    }
}
