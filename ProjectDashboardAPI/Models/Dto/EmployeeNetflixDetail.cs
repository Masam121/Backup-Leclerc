using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class EmployeeNetflixDetail
    {
        public int Id { get; set; }
        public string Department { get; set; }
        public string Factory { get; set; }
        public string HiredDate { get; set; }
        public string LeclercEmail { get; set; }
        public string Name { get; set; }
        public string O365Id { get; set; }
        public string Picture { get; set; }
        public int? SuperiorId { get; set; }
        public string Title { get; set; }
        public int? Workload { get; set; }
        public float ProjectWorkRatio { get; set; }
        public String IdSAP { get; set; }  
    }
}
