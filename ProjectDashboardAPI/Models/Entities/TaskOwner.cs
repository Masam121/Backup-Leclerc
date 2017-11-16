using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class TaskOwner
    {
        public int Id { get; set; }
        public int EmployeId { get; set; }
        public int TaskId { get; set; }

        public virtual Employe Employe { get; set; }
        public virtual Task Task { get; set; }
    }
}
