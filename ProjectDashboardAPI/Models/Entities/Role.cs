using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Role
    {
        public Role()
        {
            ProjectContributor = new HashSet<ProjectContributor>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public string RoleSigle { get; set; }

        public virtual ICollection<ProjectContributor> ProjectContributor { get; set; }
        public virtual ICollection<NotificationPartner> NotificationPartner { get; set; }
    }
}
