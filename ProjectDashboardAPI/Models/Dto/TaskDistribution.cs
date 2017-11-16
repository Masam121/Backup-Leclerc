using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Models.Dto
{
    public class TaskDistribution
    {
        public ICollection<NetflixCategory> Categories { get; set; }
        public ICollection<NetflixSerie> Series { get; set; }
    }
}
