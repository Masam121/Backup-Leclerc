using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class ProjectNetflixContributor
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public float? OccupancyRate { get; set; }
    }
}
