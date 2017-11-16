using System;
using System.Collections.Generic;

namespace ProjectDashboardAPI
{
    public partial class Budget
    {
        public Budget()
        {
            Expense = new HashSet<Expense>();
            Project = new HashSet<Project>();
        }

        public int Id { get; set; }
        public float BudgetLeft { get; set; }
        public string BudgetSapId { get; set; }
        public float BudgetSpent { get; set; }
        public float InitialBudget { get; set; }

        public virtual ICollection<Expense> Expense { get; set; }
        public virtual ICollection<Project> Project { get; set; }
    }
}
