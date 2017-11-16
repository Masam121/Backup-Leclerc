using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Project
    {
        public Project()
        {
            ConnexeProject = new HashSet<ConnexeProject>();
            Document = new HashSet<Document>();
            Notification = new HashSet<Notification>();
            ProjectContributor = new HashSet<ProjectContributor>();
            WatchlistContent = new HashSet<WatchlistContent>();
        }

        public int Id { get; set; }
        public int BudgetId { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public DateTime? EstEndDate { get; set; }
        public float? EstWorkDay { get; set; }
        public string Factory { get; set; }
        public string Priority { get; set; }
        public int? ProjectManagerId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectOwnerId { get; set; }
        public string ProjectSapId { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectsClient { get; set; }
        public DateTime? StartDate { get; set; }
        public string Thumbnail { get; set; }

        public virtual ICollection<ConnexeProject> ConnexeProject { get; set; }
        public virtual ICollection<Document> Document { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public virtual ICollection<ProjectContributor> ProjectContributor { get; set; }
        public virtual ICollection<WatchlistContent> WatchlistContent { get; set; }
        public virtual Budget Budget { get; set; }
        public virtual Employe ProjectManager { get; set; }
        public virtual Employe ProjectOwner { get; set; }
    }
}
