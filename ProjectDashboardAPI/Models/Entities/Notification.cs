using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Notification
    {
        public Notification()
        {
            NotificationPartner = new HashSet<NotificationPartner>();
            Task = new HashSet<Task>();
        }

        public int Id { get; set; }
        public double? ActualEffort { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public double? EstEffort { get; set; }
        public DateTime EstEndDate { get; set; }
        public bool IsCompleted { get; set; }
        public string NotificationSapId { get; set; }
        public string Priority { get; set; }
        public int ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }

        public virtual ICollection<NotificationPartner> NotificationPartner { get; set; }
        public virtual ICollection<Task> Task { get; set; }
        public virtual Project Project { get; set; }
    }
}
