using Newtonsoft.Json;
using System;

namespace ProjectDashboardAPI.Models.Dto
{
    public class NotificationTask
    {
        [JsonProperty(PropertyName = "Task_SAP_id")]
        public string SAPid { get; set; }

        [JsonProperty(PropertyName = "task_key")]
        public string TaskKey { get; set; }

        [JsonProperty(PropertyName = "employeeId")]
        public string EmployeeId { get; set; }

        [JsonProperty(PropertyName = "taskNotificationId")]
        public string NotificationSAPId { get; set; }

        [JsonProperty(PropertyName = "assignation_date")]
        public DateTime AssignationDate { get; set; }

        [JsonProperty(PropertyName = "est_end")]
        public DateTime EstEnd { get; set; }

        [JsonProperty(PropertyName = "actual_effort")]
        public string ActualEffort { get; set; }  
        
        [JsonProperty(PropertyName = "isComplete")]
        public string IsComplete { get; set; }

        [JsonProperty(PropertyName = "status_task")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "task_Type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "taskDescription")]
        public string Description { get; set; }
    }
}