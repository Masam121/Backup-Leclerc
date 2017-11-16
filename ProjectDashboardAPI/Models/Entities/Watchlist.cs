using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Watchlist
    {
        public int Id { get; set; }
        public int ContentId { get; set; }
        public int UserId { get; set; }

        public virtual WatchlistContent Content { get; set; }
        public virtual User User { get; set; }
    }
}
