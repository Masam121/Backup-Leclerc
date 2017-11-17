using NetflixAPI.Models;
using ProjectDashboardAPI.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface IProjectMappingService : IMapper<ProjectSAP, Project>, IMapper<Project, ProjectNetflix>
    {
    }
}
