using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeNetflixDetail> GetEmployeeById(string id);
        Task<IActionResult> RefreshEmployee();
    }
}
