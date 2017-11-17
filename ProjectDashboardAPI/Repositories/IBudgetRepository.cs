using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IBudgetRepository
    {
        Task<Budget> CreateBudget(BudgetSAP budgetSAP);
        Task<Budget> ReadOneAsyncById(int id);
    }
}
