using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class WatchlistContent
    {
        public WatchlistContent()
        {
            Watchlist = new HashSet<Watchlist>();
        }

        public int Id { get; set; }
        public int EmployeId { get; set; }
        public int ProjectId { get; set; }
        public int WatchlistId { get; set; }

        public virtual ICollection<Watchlist> Watchlist { get; set; }
        public virtual Employe Employe { get; set; }
        public virtual Project Project { get; set; }
    }
}
