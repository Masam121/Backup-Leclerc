using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class ProjectEntityToProjectNetflixMapper : IMapper<netflix_prContext, Project, ProjectNetflix>
    {
        private IEmployeeRepository _emplopyeeRepository;
        private IBudgetRepository _budgetRepository;

        public ProjectEntityToProjectNetflixMapper(IEmployeeRepository emplopyeeRepository, IBudgetRepository budgetRepository)
        {
            _emplopyeeRepository = emplopyeeRepository ?? throw new ArgumentNullException(nameof(emplopyeeRepository));
            _budgetRepository = budgetRepository ?? throw new ArgumentNullException(nameof(budgetRepository));
        }

        public ProjectNetflix Map(netflix_prContext context, Project entity)
        {
            ProjectNetflix project_netxlix = new ProjectNetflix();

            int managerId = entity.ProjectManagerId ?? default(int);
            Employe manager = _emplopyeeRepository.ReadOneAsyncById(context, managerId).Result;

            Budget budget = _budgetRepository.ReadOneAsyncById(context, entity.BudgetId).Result;

            //var expenses = (from p in _context.Expense
            //                where p.Budgetid == budget.Id
            //                select p).ToList();

            //var connexeProjectIds = (from p in _context.ConnexeProject
            //                         where p.ProjectId == project.Id
            //                         select p.ConnexeProjectSapid).ToList();

            List<ProjectNetflixExpense> expenses_netflix = new List<ProjectNetflixExpense>();
            //foreach (Expense expense in expenses)
            //{
            //    ProjectNetflixExpense expense_netflix = new ProjectNetflixExpense()
            //    {
            //        Id = expense.Id,
            //        Amount = expense.Amount,
            //        Budgetid = expense.Budgetid,
            //        ExpenseName = expense.ExpenseName
            //    };
            //    expenses_netflix.Add(expense_netflix);
            //}

            //List<ProjectNetflixCard> connexeProjects = new List<ProjectNetflixCard>();
            //foreach (String connexeProjectid in connexeProjectIds)
            //{
            //    var pro = _context.Project.FirstOrDefault(t => t.Id == project.Id);
            //    if (pro == null)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        var man = (from p in _context.Employe
            //                   where p.Id == pro.ProjectManagerId
            //                   select p).FirstOrDefault();

            //        ProjectNetflixCard connexeProject = new ProjectNetflixCard()
            //        {
            //            Id = pro.Id,
            //            ProjectName = pro.ProjectName,
            //            ProjectOwnerId = project.ProjectOwnerId,
            //            ProjectSapId = project.ProjectSapId,
            //            ProjectsClient = project.ProjectsClient,
            //            ProjectStatus = project.ProjectStatus,
            //            StartDate = project.StartDate,
            //            Thumbnail = project.Thumbnail,
            //            EstEndDate = project.EstEndDate != null ? project.EstEndDate.Value.ToString("MMMM, yyyy") : "n/a",
            //            Department = project.Department,
            //            CompletionPercentage = project.CompletionPercentage,
            //            Factory = project.Factory,
            //            ManagerName = man.Name,
            //            ManagerPicture = man.Picture,
            //        };
            //        connexeProjects.Add(connexeProject);
            //    }
            //}
            if (manager == null)
            {
                project_netxlix.ManagerName = "Unknown";
                project_netxlix.ManagerPicture = "http://www.getsmartcontent.com/content/uploads/2014/08/shutterstock_149293433.jpg";
                project_netxlix.ProjectManagerId = null;

            }
            else
            {
                project_netxlix.ManagerName = manager.Name;
                project_netxlix.ManagerPicture = manager.Picture;
                project_netxlix.ProjectManagerId = Int32.Parse(manager.IdSAP);
            }
            //project_netxlix.ProjectOwnerId = entity.ProjectOwnerId;

            project_netxlix.Id = entity.Id;
            project_netxlix.Priority = entity.Priority;
            
            project_netxlix.ProjectName = entity.ProjectName;
            
            project_netxlix.ProjectSapId = entity.ProjectSapId;
            project_netxlix.ProjectsClient = entity.ProjectsClient;
            project_netxlix.ProjectStatus = entity.ProjectStatus;
            project_netxlix.StartDate = entity.StartDate != null ? entity.StartDate.Value.ToString("dd MMMM, yyyy") : "n/a";
            project_netxlix.Thumbnail = entity.Thumbnail;
            project_netxlix.EstEndDate = entity.EstEndDate != null ? entity.EstEndDate.Value.ToString("dd MMMM, yyyy") : "n/a";
            project_netxlix.Description = entity.Description;
            project_netxlix.Department = entity.Department;
            project_netxlix.CompletionPercentage = entity.CompletionPercentage;
            project_netxlix.Factory = entity.Factory;
            project_netxlix.InitialBudget = budget.InitialBudget;
            project_netxlix.BudgetSpent = budget.BudgetSpent;
            project_netxlix.BudgetLeft = budget.BudgetLeft;
            project_netxlix.EstWorkDay = entity.EstWorkDay;
            project_netxlix.Expenses = expenses_netflix;
            //project_netxlix.ConnexeProject = connexeProjects;

            return project_netxlix;
        }      
    }
}
