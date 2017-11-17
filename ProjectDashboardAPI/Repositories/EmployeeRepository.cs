using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly netflix_prContext _context;

        public EmployeeRepository(netflix_prContext context)
        {
            _context = context;
        }

        public Task<int> ReadAsyncEmployeeId(string id)
        {
            int employeeId = (from p in _context.Employe
                              where p.IdSAP == id
                              select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employeeId);
        }

        public Task<Employe> ReadOneAsyncById(int id)
        {
            Employe employee = (from p in _context.Employe
                              where p.Id == id
                              select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employee);
        }
    }
}
