using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Document
    {
        public int Id { get; set; }
        public string DocumentDescription { get; set; }
        public string DocumentLink { get; set; }
        public int ProjectId { get; set; }

        public virtual Project Project { get; set; }
    }
}
