using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class ProjectNetflixCard
    {
        public int Id { get; set; }
        public string ProjectsClient { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Department { get; set; }
        public string EstEndDate { get; set; }
        public string Factory { get; set; }
        public int? ProjectManagerId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectOwnerId { get; set; }
        public String ProjectSapId { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public string Thumbnail { get; set; }
        public string ManagerName { get; set; }
        public string ManagerPicture { get; set; }
    }
}
