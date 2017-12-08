using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IEmployeeRepository
    {
        Task<int> ReadAsyncEmployeeId(netflix_prContext context, string id);
        Task<Employe> ReadOneAsyncById(netflix_prContext context, int id);
        Task<Employe> ReadOneAsyncBySAPId(netflix_prContext context, string id);
        Task<bool> VerifyIfEmployeeExistsBySapId(netflix_prContext context, string id);
        Task<bool> VerifyIfEmployeeAsBeenModified(netflix_prContext context, Employe employeeEntity);
        Task<Employe> CreateEmployee(netflix_prContext context, EmployeeSAP employeeSAP);
        void AddEmployee(netflix_prContext context, Employe employeeEntity);
        void UpdateEmployee(netflix_prContext context, Employe employeeEntity);
    }
}
