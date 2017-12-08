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

                EmployeeNetflixDetail employee_detail_netflix = new EmployeeNetflixDetail()
                {
                    Id = employeeEntity.Id,
                    Department = employeeEntity.Department,
                    Factory = employeeEntity.Factory,
                    HiredDate = employeeEntity.HiredDate.ToString("MMMM, yyyy"),
                    LeclercEmail = employeeEntity.LeclercEmail,
                    Name = employeeEntity.Name,
                    Picture = employeeEntity.Picture,
                    O365Id = employeeEntity.O365Id,
                    ProjectWorkRatio = employeeEntity.ProjectWorkRatio,
                    SuperiorId = employeeEntity.SuperiorId,
                    Title = employeeEntity.Title,
                    Workload = employeeEntity.Workload,
                    IdSAP = employeeEntity.IdSAP
                };
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
                        if (await _employeeRepository.VerifyIfEmployeeExistsBySapId(context, employeeSAP.id_SAP))
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
                    return new ObjectResult("Successfully refreshed...");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }                
        }
    }
}
