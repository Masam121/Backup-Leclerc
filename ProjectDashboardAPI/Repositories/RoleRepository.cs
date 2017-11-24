using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly netflix_prContext _context;

        public RoleRepository(netflix_prContext context)
        {
            _context = context;
        }

        public Task<int> ReadOneRoleIdByRoleSigle(string sigle)
        {
            var roleId = (from p in _context.Role where p.RoleSigle == sigle select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(roleId);
        }
    }
}
