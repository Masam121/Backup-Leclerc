using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IRoleRepository
    {
        Task<int> ReadOneRoleIdByRoleSigle(string sigle);
    }
}
