using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Employe
    {
        public Employe()
        {
            ProjectProjectManager = new HashSet<Project>();
            ProjectProjectOwner = new HashSet<Project>();
            ProjectContributor = new HashSet<ProjectContributor>();
            TaskOwner = new HashSet<TaskOwner>();
            User = new HashSet<User>();
            WatchlistContent = new HashSet<WatchlistContent>();
            NotificationPartner = new HashSet<NotificationPartner>();
        }

        public int Id { get; set; }
        public string Department { get; set; }
        public string Factory { get; set; }
        public DateTime HiredDate { get; set; }
        public string IdSAP { get; set; }
        public string LeclercEmail { get; set; }
        public string Name { get; set; }
        public string O365Id { get; set; }
        public string Picture { get; set; }
        public float ProjectWorkRatio { get; set; }
        public int? SuperiorId { get; set; }
        public string Title { get; set; }
        public int? Workload { get; set; }

        public virtual ICollection<Project> ProjectProjectManager { get; set; }
        public virtual ICollection<Project> ProjectProjectOwner { get; set; }
        public virtual ICollection<ProjectContributor> ProjectContributor { get; set; }
        public virtual ICollection<TaskOwner> TaskOwner { get; set; }
        public virtual ICollection<User> User { get; set; }
        public virtual ICollection<WatchlistContent> WatchlistContent { get; set; }
        public virtual ICollection<NotificationPartner> NotificationPartner { get; set; }
    }
}
