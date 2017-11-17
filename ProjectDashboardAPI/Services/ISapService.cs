using NetflixAPI.Models;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface ISapService
    {
        Task<IEnumerable<ProjectSAP>> GetSapProject();
    }
}
