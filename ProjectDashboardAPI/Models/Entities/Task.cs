using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Task
    {
        public Task()
        {
            TaskOwner = new HashSet<TaskOwner>();
        }

        public int Id { get; set; }
        public string TaskSAPId { get; set; }
        public string ConcatenatedId { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int? ActualEffort { get; set; }
        public DateTime AssignationDate { get; set; }
        public int EstEffort { get; set; }
        public DateTime EstEnd { get; set; }
        public bool IsComplete { get; set; }
        public int NotificationId { get; set; }
        public string Status { get; set; }

        public virtual ICollection<TaskOwner> TaskOwner { get; set; }
        public virtual Notification Notification { get; set; }
    }
}
