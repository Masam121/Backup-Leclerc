using NetflixAPI.Models;
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
        Task<Employe> ReadOneAsyncBySAPId(string id);
        Task<bool> VerifyIfEmployeeExistsBySapId(string id);
        Task<bool> VerifyIfEmployeeAsBeenModified(Employe employeeEntity);
        Task<Employe> CreateEmployee(EmployeeSAP employeeSAP);
        void AddEmployee(Employe employeeEntity);
        void UpdateEmployee(Employe employeeEntity);
    }
}
