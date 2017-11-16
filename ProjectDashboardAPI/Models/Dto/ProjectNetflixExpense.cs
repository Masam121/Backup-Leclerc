using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixAPI.Models
{
    public class ProjectNetflixExpense
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public int Budgetid { get; set; }
        public string ExpenseName { get; set; }
    }
}
