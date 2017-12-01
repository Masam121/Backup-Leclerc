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
            Employe employeeEntity = await _employeeRepository.ReadOneAsyncBySAPId(id);

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

        public async Task<IActionResult> RefreshEmployee()
        {
            try
            {
                IEnumerable<EmployeeSAP> employees = await _sapService.GetSapEmployee();
                foreach (EmployeeSAP employeeSAP in employees)
                {
                    if(await _employeeRepository.VerifyIfEmployeeExistsBySapId(employeeSAP.id_SAP))
                    {
                        var employeeEntity = await _employeeRepository.CreateEmployee(employeeSAP);
                        if (await _employeeRepository.VerifyIfEmployeeAsBeenModified(employeeEntity))
                        {
                            _employeeRepository.UpdateEmployee(employeeEntity);
                        }
                    }
                    else
                    {
                        var employeeEntity = await _employeeRepository.CreateEmployee(employeeSAP);
                        _employeeRepository.AddEmployee(employeeEntity);
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
