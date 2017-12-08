using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public Task<int> ReadOneRoleIdByRoleSigle(netflix_prContext context, string sigle)
        {
            var roleId = (from p in context.Role where p.RoleSigle == sigle select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(roleId);
        }
    }
}
