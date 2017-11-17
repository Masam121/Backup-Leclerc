using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;
using ProjectDashboardAPI.Mappers;

namespace ProjectDashboardAPI.Services.Mapping
{
    public class ProjectCardMappingService : IProjectCardMappingService
    {
        private readonly IMapper<Project, ProjectNetflixCard> _ProjectEntityToProjectNetflixCardMapper;

        public ProjectCardMappingService(IMapper<Project, ProjectNetflixCard> projectEntityToProjectNetflixCardMapper)
        {
            _ProjectEntityToProjectNetflixCardMapper = projectEntityToProjectNetflixCardMapper ?? throw new ArgumentNullException(nameof(projectEntityToProjectNetflixCardMapper));
        }
        
        public ProjectNetflixCard Map(Project entity)
        {
            return _ProjectEntityToProjectNetflixCardMapper.Map(entity);
        }
    }
}
