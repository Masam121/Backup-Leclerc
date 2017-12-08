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
        private readonly IMapper<netflix_prContext, Project, ProjectNetflixCard> _ProjectEntityToProjectNetflixCardMapper;

        public ProjectCardMappingService(IMapper<netflix_prContext ,Project, ProjectNetflixCard> projectEntityToProjectNetflixCardMapper)
        {
            _ProjectEntityToProjectNetflixCardMapper = projectEntityToProjectNetflixCardMapper ?? throw new ArgumentNullException(nameof(projectEntityToProjectNetflixCardMapper));
        }
        
        public ProjectNetflixCard Map(netflix_prContext context, Project entity)
        {
            return _ProjectEntityToProjectNetflixCardMapper.Map(context, entity);
        }
    }
}
