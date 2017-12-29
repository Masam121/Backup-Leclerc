using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;

namespace ProjectDashboardAPI.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public void AddEmployee(netflix_prContext context, Employe employeeEntity)
        {
            context.Add(employeeEntity);
        }

        public Task<Employe> CreateEmployee(netflix_prContext context, EmployeeSAP employeeSAP)
        {
            Employe employee = new Employe();

            employee.Department = employeeSAP.departement;
            employee.Factory = employeeSAP.factory;
            employee.HiredDate = Convert.ToDateTime(employeeSAP.hiredate);
            employee.IdSAP = employeeSAP.id_SAP.TrimStart('0');
            employee.LeclercEmail = employeeSAP.leclercEmail;
            employee.Name = employeeSAP.name;
            employee.O365Id = employeeSAP.o365;
            employee.Picture = employeeSAP.picture;
            employee.Workload = Convert.ToInt32(double.Parse(employeeSAP.workload, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            employee.Title = employeeSAP.title;
            employee.SuperiorId = string.IsNullOrEmpty(employeeSAP.superior) ? (int?)null : int.Parse(employeeSAP.superior);
            employee.ProjectWorkRatio = 100;

            return System.Threading.Tasks.Task.FromResult(employee);
        }

        public Task<int> ReadAsyncEmployeeId(netflix_prContext context, string id)
        {
            int employeeId = (from p in context.Employe
                              where p.IdSAP == id
                              select p.Id).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employeeId);
        }

        public Task<List<Employe>> ReadManyAsyncEmployee(netflix_prContext context)
        {
            List<Employe> employees = (from p in context.Employe
                              select p).ToList();
            return System.Threading.Tasks.Task.FromResult(employees);
        }

        public Task<Employe> ReadOneAsyncById(netflix_prContext context, int id)
        {
            Employe employee = (from p in context.Employe
                              where p.Id == id
                              select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employee);
        }

        public Task<Employe> ReadOneAsyncBySAPId(netflix_prContext context, string id)
        {
            Employe employee = (from p in context.Employe
                                where p.IdSAP == id
                                select p).FirstOrDefault();

            string dep = (from p in context.Employe
                                where p.IdSAP == id
                                select p.Department).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(employee);
        }

        public void UpdateEmployee(netflix_prContext context, Employe employeeEntity)
        {
            Employe existingEmployee = (from p in context.Employe
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
            //existingEmployee.ProjectWorkRatio = employeeEntity.ProjectWorkRatio;

            context.Employe.Update(existingEmployee);
        }

        public async Task<bool> VerifyIfEmployeeAsBeenModified(netflix_prContext context, Employe employeeEntity)
        {
            Employe existingEmploye = await ReadOneAsyncBySAPId(context, employeeEntity.IdSAP);

            if (existingEmploye.Department == employeeEntity.Department &&
                            existingEmploye.Factory == employeeEntity.Factory &&
                            existingEmploye.HiredDate == employeeEntity.HiredDate &&
                            existingEmploye.IdSAP == employeeEntity.IdSAP &&
                            existingEmploye.LeclercEmail == employeeEntity.LeclercEmail &&
                            existingEmploye.Name == employeeEntity.Name &&
                            existingEmploye.O365Id == employeeEntity.O365Id &&
                            existingEmploye.Picture == employeeEntity.Picture &&
                            existingEmploye.Workload == employeeEntity.Workload &&
                            existingEmploye.Title == employeeEntity.Title
                            //existingEmploye.SuperiorId == employeeEntity.SuperiorId &&
                            //existingEmploye.ProjectWorkRatio == employeeEntity.ProjectWorkRatio
                            )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Task<bool> VerifyIfEmployeeExistsBySapId(netflix_prContext context, string id)
        {
            Employe employeeExists = context.Employe.FirstOrDefault(x => x.IdSAP == id);
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
