using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;

namespace ProjectDashboardAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly netflix_prContext _context;

        public EmployeeRepository(netflix_prContext context)
        {
            _context = context;
        }

        public void AddEmployee(Employe employeeEntity)
        {
            _context.Add(employeeEntity);
        }

        public Task<Employe> CreateEmployee(EmployeeSAP employeeSAP)
        {
            Employe employee = new Employe();

            employee.Department = employeeSAP.department;
            employee.Factory = employeeSAP.factory;
            employee.HiredDate = Convert.ToDateTime(employeeSAP.hiredDate);
            employee.IdSAP = employeeSAP.id_SAP.TrimStart('0');
            employee.LeclercEmail = employeeSAP.leclercEmail;
            employee.Name = employeeSAP.name;
            employee.O365Id = employeeSAP.o365;
            employee.Picture = employeeSAP.picture;
            employee.Workload = Convert.ToInt32(double.Parse(employeeSAP.workload, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            employee.Title = employeeSAP.title;
            //employee.SuperiorId = string.IsNullOrEmpty(employeeSAP.superior) ? (int?)null : int.Parse(employeeSAP.superior);
            employee.ProjectWorkRatio = 100;

            return System.Threading.Tasks.Task.FromResult(employee);
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

        public Task<Employe> ReadOneAsyncBySAPId(string id)
        {
            Employe employee = (from p in _context.Employe
                                where p.IdSAP == id
                                select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employee);
        }

        public void UpdateEmployee(Employe employeeEntity)
        {
            Employe existingEmployee = (from p in _context.Employe
                                where p.Id == employeeEntity.Id
                                select p).FirstOrDefault();

            existingEmployee.Department = employeeEntity.Department;
            existingEmployee.Factory = employeeEntity.Factory;
            existingEmployee.HiredDate = employeeEntity.HiredDate;
            existingEmployee.IdSAP = employeeEntity.IdSAP;
            existingEmployee.LeclercEmail = employeeEntity.LeclercEmail;
            existingEmployee.Name = employeeEntity.Name;
            existingEmployee.O365Id = employeeEntity.O365Id;
            existingEmployee.Picture = employeeEntity.Picture;
            existingEmployee.Workload = employeeEntity.Workload;
            existingEmployee.Title = employeeEntity.Title;
            //existingEmployee.SuperiorId = employeeEntity.SuperiorId;
            existingEmployee.ProjectWorkRatio = employeeEntity.ProjectWorkRatio;

            _context.Employe.Update(existingEmployee);
        }

        public async Task<bool> VerifyIfEmployeeAsBeenModified(Employe employeeEntity)
        {
            Employe existingEmploye = await ReadOneAsyncBySAPId(employeeEntity.IdSAP);

            if (existingEmploye.Department == employeeEntity.Department &&
                            existingEmploye.Factory == employeeEntity.Factory &&
                            existingEmploye.HiredDate == employeeEntity.HiredDate &&
                            existingEmploye.IdSAP == employeeEntity.IdSAP &&
                            existingEmploye.LeclercEmail == employeeEntity.LeclercEmail &&
                            existingEmploye.Name == employeeEntity.Name &&
                            existingEmploye.O365Id == employeeEntity.O365Id &&
                            existingEmploye.Picture == employeeEntity.Picture &&
                            existingEmploye.Workload == employeeEntity.Workload &&
                            existingEmploye.Title == employeeEntity.Title &&
                            //existingEmploye.SuperiorId == employeeEntity.SuperiorId &&
                            existingEmploye.ProjectWorkRatio == employeeEntity.ProjectWorkRatio)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Task<bool> VerifyIfEmployeeExistsBySapId(string id)
        {
            Employe employeeExists = _context.Employe.FirstOrDefault(x => x.IdSAP == id);
            if(employeeExists != null)
            {
                return System.Threading.Tasks.Task.FromResult(true);
            }
            else
            {
                return System.Threading.Tasks.Task.FromResult(false);
            }
        }
    }
}
