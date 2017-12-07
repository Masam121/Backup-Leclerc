using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI.Controllers
{
    public class NotificationDto
    {
        public string projectName { get; set; }
        public string Id { get; set; }
        public string description { get; set; }
        public string creationDate { get; set; }
        public string endDate { get; set; }
        public string status { get; set; }
        public string estimatedEffort { get; set; }
        public string actualEffort { get; set; }
        public List<PartnerDto> partners { get; set; }
    }
}