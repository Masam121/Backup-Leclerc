using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class ProjectSAP
    {
        public string id_SAP { get; set; }
        public string projectNameFr { get; set; }
        public string projectNameEn { get; set; }
        public string department { get; set; }
        public string factories { get; set; }
        public string startDate { get; set; }
        public string estEnd { get; set; }
        public string projectOwnerId { get; set; }
        public string projectManagerId { get; set; }
        public string projectOwnerName { get; set; }
        public string projectOwnerPicture { get; set; }
        public string completionPercentage { get; set; }
        public string projectStatus { get; set; }
        public string clients { get; set; }
        public string priority { get; set; }
        public string thumbnails { get; set; }
        public string description { get; set; }      
        public List<BudgetSAP> budget { get; set; }
    }
}
