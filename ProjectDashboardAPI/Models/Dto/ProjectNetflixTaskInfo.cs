using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;

namespace NetflixAPI.Models
{
    public class ProjectNetflixTaskInfo
    {
        public ICollection<TaskDto> Tasks { get; set; }
        public ICollection<NetflixCategory> Categories { get; set; }
        public ICollection<NetflixSerie> Series { get; set; }
        public ICollection<ProjectNetflixTaskStatusEffort> Effort { get; set; }
    }
}
