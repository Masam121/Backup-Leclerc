using Newtonsoft.Json;

namespace ProjectDashboardAPI.Models.Dto
{
    public class Partner
    {
        [JsonProperty(PropertyName = "partnerId")]
        public string EmployeId { get; set; }
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
    }
}