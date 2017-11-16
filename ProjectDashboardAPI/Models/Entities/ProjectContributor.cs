using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class ProjectContributor
    {
        public int Id { get; set; }
        public int EmployeId { get; set; }
        public DateTime? EndDateContribution { get; set; }
        public int ProjectId { get; set; }
        public int? RoleId { get; set; }
        public DateTime? StartDateContribution { get; set; }

        public virtual Employe Employe { get; set; }
        public virtual Project Project { get; set; }
        public virtual Role Role { get; set; }
    }
}
