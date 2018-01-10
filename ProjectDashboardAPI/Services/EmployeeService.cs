using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;

namespace ProjectDashboardAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeRepository _employeeRepository;

        private ISapService _sapService;

        public EmployeeService(IEmployeeRepository employeeRepository, ISapService sapService)
        {  
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));

            _sapService = sapService ?? throw new ArgumentNullException(nameof(sapService));
        }
       
        public async Task<EmployeeNetflixDetail> GetEmployeeById(string id)
        {
            using (var context = new netflix_prContext())
            {
                Employe employeeEntity = await _employeeRepository.ReadOneAsyncBySAPId(context, id);

                EmployeeNetflixDetail employee_detail_netflix = new EmployeeNetflixDetail();

                employee_detail_netflix.Id = employeeEntity.Id;
                employee_detail_netflix.Department = employeeEntity.Department;
                employee_detail_netflix.Factory = employeeEntity.Factory;
                employee_detail_netflix.HiredDate = employeeEntity.HiredDate.ToString("MMMM, yyyy");
                employee_detail_netflix.LeclercEmail = employeeEntity.LeclercEmail;
                employee_detail_netflix.Name = employeeEntity.Name;
                employee_detail_netflix.Picture = employeeEntity.Picture;
                employee_detail_netflix.O365Id = employeeEntity.O365Id;
                employee_detail_netflix.ProjectWorkRatio = employeeEntity.ProjectWorkRatio;
                employee_detail_netflix.SuperiorId = employeeEntity.SuperiorId;
                employee_detail_netflix.Title = employeeEntity.Title;
                employee_detail_netflix.Workload = employeeEntity.Workload;
                employee_detail_netflix.IdSAP = employeeEntity.IdSAP;


                return employee_detail_netflix;
            }               
        }

        public async Task<IActionResult> PostEmployeeRatio(long id, string ratio)
        {
            using (var context = new netflix_prContext())
            {
                var employee = context.Employe.FirstOrDefault(t => t.IdSAP == id.ToString());
                employee.ProjectWorkRatio = Int32.Parse(ratio);
                context.Employe.Update(employee);
                context.SaveChanges();
                return new ObjectResult(employee.ProjectWorkRatio);
            }           
        }

        public async Task<string> GetEmployeeRatio(long id)
        {
            using (var context = new netflix_prContext())
            {
                var employee = context.Employe.FirstOrDefault(t => t.IdSAP == id.ToString());
                
                return employee.ProjectWorkRatio.ToString();
            }
        }

        public async Task<IActionResult> RefreshEmployee()
        {
            using (var context = new netflix_prContext())
            {
                try
                {
                    IEnumerable<EmployeeSAP> employees = await _sapService.GetSapEmployee();
                    foreach (EmployeeSAP employeeSAP in employees)
                    {
                        if (await _employeeRepository.VerifyIfEmployeeExistsBySapId(context, employeeSAP.id_SAP.TrimStart('0')))
                        {
                            var employeeEntity = await _employeeRepository.CreateEmployee(context, employeeSAP);
                            if (await _employeeRepository.VerifyIfEmployeeAsBeenModified(context, employeeEntity))
                            {
                                _employeeRepository.UpdateEmployee(context, employeeEntity);
                            }
                        }
                        else
                        {
                            var employeeEntity = await _employeeRepository.CreateEmployee(context, employeeSAP);
                            _employeeRepository.AddEmployee(context, employeeEntity);
                        }
                    }
                    context.SaveChanges();
                    return new ObjectResult("Successfully refreshed Employees...");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }                
        }
    }
}
