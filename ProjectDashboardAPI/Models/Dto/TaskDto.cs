using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class TaskDto
    {
        public string IdSAP { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? ActualEffort { get; set; }
        public string AssignationDate { get; set; }
        public int? EstEffort { get; set; }
        public string EstEnd { get; set; }
        public bool IsComplete { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Status { get; set; }
        public string InChargeName { get; set; }
        public string InchargeId { get; set; }
    }
}
