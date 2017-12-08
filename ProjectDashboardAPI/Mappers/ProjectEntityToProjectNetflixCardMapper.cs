using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class ProjectEntityToProjectNetflixCardMapper : IMapper<netflix_prContext, Project, ProjectNetflixCard>
    {
        private IEmployeeRepository _emplopyeeRepository;

        public ProjectEntityToProjectNetflixCardMapper(IEmployeeRepository emplopyeeRepository)
        {
            _emplopyeeRepository = emplopyeeRepository ?? throw new ArgumentNullException(nameof(emplopyeeRepository));
        }

        public ProjectNetflixCard Map(netflix_prContext context, Project entity)
        {
            ProjectNetflixCard project_netxlix_card = new ProjectNetflixCard();

            int managerId = entity.ProjectManagerId ?? default(int);
            var manager = _emplopyeeRepository.ReadOneAsyncById(context, managerId).Result;

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

            project_netxlix_card.Id = entity.Id;
            project_netxlix_card.ProjectManagerId = entity.ProjectManagerId;
            project_netxlix_card.ProjectName = entity.ProjectName;
            project_netxlix_card.ProjectOwnerId = entity.ProjectOwnerId;
            project_netxlix_card.ProjectSapId = entity.ProjectSapId;
            project_netxlix_card.ProjectsClient = entity.ProjectsClient;
            project_netxlix_card.ProjectStatus = entity.ProjectStatus;
            project_netxlix_card.StartDate = entity.StartDate;
            project_netxlix_card.Thumbnail = entity.Thumbnail;
            project_netxlix_card.EstEndDate = entity.EstEndDate != null ? entity.EstEndDate.Value.ToString("MMMM, yyyy") : "n/a";
            project_netxlix_card.Department = entity.Department;
            project_netxlix_card.CompletionPercentage = entity.CompletionPercentage;
            project_netxlix_card.Factory = entity.Factory;

            return project_netxlix_card;
        }
    }
}
