using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IEmployeeRepository
    {
        Task<int> ReadAsyncEmployeeId(string id);
        Task<Employe> ReadOneAsyncById(int id);
    }
}
