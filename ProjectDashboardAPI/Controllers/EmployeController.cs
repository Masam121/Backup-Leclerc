using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Services;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class EmployeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeController(netflix_prContext context, IEmployeeService employeeService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        }      

        [HttpGet("{id}", Name = "GetEmploye")]
        public async Task<IActionResult> GetById(string id)
        {
            EmployeeNetflixDetail employee = await _employeeService.GetEmployeeById(id);

            return Ok(employee);
        }

        [HttpGet("Refresh", Name = "RefreshEmployees")]
        public async Task<IActionResult> RefreshEmployees()
        {
            try
            {
                var response = await _employeeService.RefreshEmployee();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}/ratio", Name = "GetEmployeRatio")]
        public async Task<IActionResult> GetEmployeeWorkingRatio(long id)
        {
            string ratio = await _employeeService.GetEmployeeRatio(id);
            return Ok(ratio);
        }

        [HttpPost("{id}/ratio", Name = "PostEmployeRatio")]
        public IActionResult PostEmployeeWorkingRatio(long id, string ratio)
        {
            _employeeService.PostEmployeeRatio(id, ratio);
            return new ObjectResult(ratio);            
        }
    }
}
