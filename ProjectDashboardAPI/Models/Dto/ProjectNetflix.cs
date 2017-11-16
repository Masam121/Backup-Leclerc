using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class ProjectNetflix
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public string ProjectsClient { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public string EstEndDate { get; set; }
        public string Factory { get; set; }
        public string Priority { get; set; }
        public int? ProjectManagerId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectOwnerId { get; set; }
        public String ProjectSapId { get; set; }
        public string ProjectStatus { get; set; }
        public string StartDate { get; set; }
        public string Thumbnail { get; set; }
        public string ManagerName { get; set; }
        public string ManagerPicture { get; set; }
        public float InitialBudget { get; set; }
        public float BudgetSpent { get; set; }
        public float BudgetLeft { get; set; }
        public float? EstWorkDay { get; set; }
        public ICollection<ProjectNetflixExpense> Expenses { get; set; }
        public ICollection<ProjectNetflixCard> ConnexeProject { get; set; }
    }
}
