using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Expense
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public int Budgetid { get; set; }
        public string ExpenseName { get; set; }

        public virtual Budget Budget { get; set; }
    }
}
