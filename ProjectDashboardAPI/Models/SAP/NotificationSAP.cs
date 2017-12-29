using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Models.Dto
{
    public class NotificationSAP
    {
        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "notificationSAP_id")]
        public string NotificationSapId { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonProperty(PropertyName = "start_date")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "est_End")]
        public DateTime EstEndDate { get; set; }

        [JsonProperty(PropertyName = "completed_date")]
        public DateTime? CompletedDate { get; set; }

        [JsonProperty(PropertyName = "est_Effort")]
        public string EstEffort { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        [JsonProperty(PropertyName = "actual_effort")]
        public string ActualEffort { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public string Priority { get; set; }

        [JsonProperty(PropertyName = "department")]
        public string Department { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public string IsCompleted { get; set; }

        public List<NotificationTask> Tasks { get; set; }

        public List<Partner> Partners { get; set; }





    }
}
