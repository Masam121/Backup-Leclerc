using NetflixAPI.Models;
using ProjectDashboardAPI.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services.Mapping
{
    public interface IProjectCardMappingService : IMapper<netflix_prContext, Project, ProjectNetflixCard>
    {
    }
}
