using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class User
    {
        public User()
        {
            Watchlist = new HashSet<Watchlist>();
        }

        public int Id { get; set; }
        public int EmployeId { get; set; }

        public virtual ICollection<Watchlist> Watchlist { get; set; }
        public virtual Employe Employe { get; set; }
    }
}
