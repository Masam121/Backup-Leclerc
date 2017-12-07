using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectDashboardAPI;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ProjectDashboardAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using ProjectDashboardAPI.Services;

namespace ProjectDashboardAPI.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            List<NotificationDto> notificationList = await _notificationService.GetAllNotifications();

            return Ok(notificationList);
        }

        [HttpGet("{departmentId}", Name = "getByDepartementalIdNotifications")]
        public async Task<IActionResult> GetDepartementalNotifications(string departmentId)
        {
            if(departmentId.Substring(0, 3) == "All")
            {
                return Ok(await GetAllNotifications());
            }
            else
            {
                return Ok(await _notificationService.GetDepartementalNotifications(departmentId));
            }            
        }

        [HttpGet("project/{projectId}", Name = "getByProjectIdNotification")]
        public async Task<IActionResult> getNotificationByProjectId(string projectId)
        {
            return Ok(await _notificationService.GetProjectNotification(projectId));           
        }

        [HttpGet("employee/{id}", Name = "getByEmployeeIdNotifications")]
        public async Task<IActionResult> getByEmployeeIdNotifications(string id)
        {
            return Ok(await _notificationService.GetEmployeeNotifications(id));           
        }

        [HttpGet("employee/{id}/workload", Name = "getByEmployeeIdNotificationsWorkload")]
        public async Task<IActionResult> getByEmployeeIdNotificationsWorkload(string id)
        {
            return Ok(await _notificationService.GetEmployeeNotificationsWorkload(id));
        }

        [HttpGet("Refresh", Name = "RefreshNotifications")]
        public async Task<IActionResult> RefreshNotifications()
        {
            try
            {
                var response = await _notificationService.RefreshNotificationsData();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }           
        }
    }
}
