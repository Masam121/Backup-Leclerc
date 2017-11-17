using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;
using ProjectDashboardAPI.Mappers;

namespace ProjectDashboardAPI.Services
{
    public class ProjectMappingService : IProjectMappingService
    {
        private readonly IMapper<ProjectSAP, Project> _projectSAPToProjectEntityMapper;
        private readonly IMapper<Project, ProjectNetflix> _projectEntityToProjectNetflixMapper;
 
        public ProjectMappingService(
            IMapper<ProjectSAP, Project> projectSAPToProjectEntityMapper,
            IMapper<Project, ProjectNetflix> projectEntityToProjectNetflixMapper
        )
        {
            _projectSAPToProjectEntityMapper = projectSAPToProjectEntityMapper ?? throw new ArgumentNullException(nameof(projectSAPToProjectEntityMapper));
            _projectEntityToProjectNetflixMapper = projectEntityToProjectNetflixMapper ?? throw new ArgumentNullException(nameof(projectEntityToProjectNetflixMapper));            
        }

        public Project Map(ProjectSAP entity)
        {
            return _projectSAPToProjectEntityMapper.Map(entity);
        }

        public ProjectNetflix Map(Project entity)
        {
            return _projectEntityToProjectNetflixMapper.Map(entity);
        }
    }
}
