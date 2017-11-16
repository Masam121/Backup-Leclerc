using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class NotificationPartner
    {
        public int Id { get; set; }
        public int? EmployeId { get; set; }
        public int NotificationId { get; set; }
        public int? RoleId { get; set; }
        public string ConcatenatedId { get; set; }

        public virtual Role Role { get; set; }
        public virtual Notification Notification { get; set; }
        public virtual Employe Employe { get; set; }
    }
}
