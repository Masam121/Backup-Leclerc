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
        private readonly IMapper<netflix_prContext, ProjectSAP, Project> _projectSAPToProjectEntityMapper;
        private readonly IMapper<netflix_prContext, Project, ProjectNetflix> _projectEntityToProjectNetflixMapper;
 
        public ProjectMappingService(
            IMapper<netflix_prContext, ProjectSAP, Project> projectSAPToProjectEntityMapper,
            IMapper<netflix_prContext, Project, ProjectNetflix> projectEntityToProjectNetflixMapper
        )
        {
            _projectSAPToProjectEntityMapper = projectSAPToProjectEntityMapper ?? throw new ArgumentNullException(nameof(projectSAPToProjectEntityMapper));
            _projectEntityToProjectNetflixMapper = projectEntityToProjectNetflixMapper ?? throw new ArgumentNullException(nameof(projectEntityToProjectNetflixMapper));            
        }

        public Project Map(netflix_prContext context, ProjectSAP entity)
        {
            return _projectSAPToProjectEntityMapper.Map(context, entity);
        }

        public ProjectNetflix Map(netflix_prContext context, Project entity)
        {
            return _projectEntityToProjectNetflixMapper.Map(context, entity);
        }
    }
}
