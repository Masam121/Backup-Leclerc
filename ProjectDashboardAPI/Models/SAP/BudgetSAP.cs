using ProjectDashboardAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class BudgetSAP
    {
        public string budgetLeft { get; set; }
        public string id_SAP { get; set; }
        public string budgetSpent { get; set; }
        public string initialBudget { get; set; }
        public List<ExpenseSAP> expensesBudget { get; set; }
        public List<ConnexeProjectSAP> connexeProject { get; set; }
    }
}
