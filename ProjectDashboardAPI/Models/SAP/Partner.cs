using Newtonsoft.Json;

namespace ProjectDashboardAPI.Models.Dto
{
    public class Partner
    {
        [JsonProperty(PropertyName = "partnerId")]
        public string EmployeId { get; set; }
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
        [JsonProperty(PropertyName = "est_Effort")]
        public string EstimatedEffort { get; set; }
        [JsonProperty(PropertyName = "actual_effort")]
        public string ActualEffort { get; set; }
    }
}