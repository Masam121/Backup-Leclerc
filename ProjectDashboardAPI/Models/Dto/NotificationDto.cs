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
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string status { get; set; }
        public string estEffort { get; set; }
        public string actualEffort { get; set; }
        public string completion { get; set; }
        public string comparator { get; set; }
        public List<PartnerDto> partners { get; set; }
    }
}