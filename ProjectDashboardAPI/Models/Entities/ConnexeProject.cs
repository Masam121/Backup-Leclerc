using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class ConnexeProject
    {
        public int Id { get; set; }
        public string ConnexeProjectSapid { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
