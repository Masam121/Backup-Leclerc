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
            List<NotificationDto> notificationList = await _notificationService.GetAllActiveNotifications();

            return Ok(notificationList);
        }

        [HttpGet("Active", Name = "getActiveNotifications")]
        public async Task<IActionResult> GetActiveNotifications()
        {
            List<NotificationDto> notificationList = await _notificationService.GetAllNotifications();

            return Ok(notificationList);
        }

        [HttpGet("{id}", Name = "getByIdNotification")]
        public async Task<IActionResult> GetNotification(string id)
        {
            NotificationDto notification = await _notificationService.GetNotification(id);

            return Ok(notification);
        }

        [HttpGet("department/{departmentId}", Name = "getByDepartementalIdNotifications")]
        public async Task<IActionResult> GetDepartementalNotifications(string departmentId)
        {
            if(departmentId.Substring(0, 3) == "All")
            {
                List<NotificationDto> notificationList = await _notificationService.GetAllNotifications();
                return Ok(notificationList);
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

        [HttpGet("employee/{id}/{month}", Name = "getByEmployeeIdNotificationsForSpecifiedMonth")]
        public async Task<IActionResult> getByEmployeeIdNotifications(string id, string month)
        {
            return Ok(await _notificationService.GetEmployeeNotificationsForSpecifiedMonth(id, month));
        }

        [HttpGet("employee/{id}/workload", Name = "getByEmployeeIdNotificationsWorkload")]
        public async Task<IActionResult> getByEmployeeIdNotificationsWorkload(string id)
        {
            return Ok(await _notificationService.GetEmployeeNotificationsWorkload(id));
        }

        [HttpGet("employee/workload", Name = "getByEmployeesNotificationsWorkload")]
        public async Task<IActionResult> getEmployessNotificationsWorkload()
        {
            return Ok(await _notificationService.GetEmployeesNotificationsWorkload());
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
