using NetflixAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        public Task<Budget> CreateBudget(netflix_prContext context, BudgetSAP budgetSAP)
        {
            //string.IsNullOrEmpty(budgetSAP.id_SAP) ? 0 : int.Parse(budgetSAP.id_SAP);
            Budget budget = new Budget();
            budget.BudgetSapId = string.IsNullOrEmpty(budgetSAP.id_SAP) ? "" : budgetSAP.id_SAP;
            budget.BudgetLeft = string.IsNullOrEmpty(budgetSAP.budgetLeft) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.budgetLeft, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            budget.BudgetSpent = string.IsNullOrEmpty(budgetSAP.budgetSpent) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.budgetSpent, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            budget.InitialBudget = string.IsNullOrEmpty(budgetSAP.initialBudget) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.initialBudget, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));

            Budget BudgetExists = context.Budget.FirstOrDefault(x => x.BudgetSapId == budget.BudgetSapId);

            if (BudgetExists != null)
            {
                if (budget.BudgetLeft == BudgetExists.BudgetLeft ||
                budget.BudgetSpent == BudgetExists.BudgetSpent ||
                budget.InitialBudget == BudgetExists.InitialBudget)
                {
                    return System.Threading.Tasks.Task.FromResult(BudgetExists);
                }
                else
                {
                    BudgetExists.BudgetLeft = budget.BudgetLeft;
                    BudgetExists.BudgetSpent = budget.BudgetSpent;
                    BudgetExists.InitialBudget = budget.InitialBudget;
                    context.Budget.Update(BudgetExists);

                    return System.Threading.Tasks.Task.FromResult(BudgetExists);
                }
            }
            else
            {
                context.Budget.Add(budget);
                return System.Threading.Tasks.Task.FromResult(budget);
            }
        }

        public Task<Budget> ReadOneAsyncById(netflix_prContext context, int id)
        {
            var budget = (from p in context.Budget
                          where p.Id == id
                          select p).First();

            return System.Threading.Tasks.Task.FromResult(budget);
        }
    }
}
