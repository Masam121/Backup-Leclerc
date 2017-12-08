using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public interface IBudgetRepository
    {
        Task<Budget> CreateBudget(netflix_prContext context, BudgetSAP budgetSAP);
        Task<Budget> ReadOneAsyncById(netflix_prContext context, int id);
    }
}
