using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using ProjectDashboardAPI;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly netflix_prContext _context;
        public ProjectController(netflix_prContext context)
        {
            _context = context;
        }

        //Sorting Lists
        public List<string> allFacilitySorting = new List<string> { "1", "undefined" };
        public List<string> exceedingBudgetSorting = new List<string> { "En dépassement de budget", "Exceeding budget", "Exceder el persupuesto" };
        public List<string> exceedingXAmountSorting = new List<string> { "Coutant plus de 100 000$", "Cost more than 100 000$", "Coste de más de 100 000 $" };
        public List<string> lateSorting = new List<string> { "En retard", "Late", "Tarde" };

        protected Budget CreateBudget(BudgetSAP budgetSAP)
        {
            //string.IsNullOrEmpty(budgetSAP.id_SAP) ? 0 : int.Parse(budgetSAP.id_SAP);
            Budget budget = new Budget();
            budget.BudgetSapId = string.IsNullOrEmpty(budgetSAP.id_SAP) ? "" : budgetSAP.id_SAP;
            budget.BudgetLeft = string.IsNullOrEmpty(budgetSAP.budgetLeft) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.budgetLeft, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            budget.BudgetSpent = string.IsNullOrEmpty(budgetSAP.budgetSpent) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.budgetSpent, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));
            budget.InitialBudget = string.IsNullOrEmpty(budgetSAP.initialBudget) ? 0 : Convert.ToInt32(double.Parse(budgetSAP.initialBudget, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));

            Budget BudgetExists = _context.Budget.FirstOrDefault(x => x.BudgetSapId == budget.BudgetSapId);

            if (BudgetExists != null)
            {
                if (budget.BudgetLeft == BudgetExists.BudgetLeft ||
                budget.BudgetSpent == BudgetExists.BudgetSpent ||
                budget.InitialBudget == BudgetExists.InitialBudget)
                {
                    return BudgetExists;
                }
                else
                {
                    BudgetExists.BudgetLeft = budget.BudgetLeft;
                    BudgetExists.BudgetSpent = budget.BudgetSpent;
                    BudgetExists.InitialBudget = budget.InitialBudget;
                    _context.Budget.Update(BudgetExists);
                    return BudgetExists;
                }
            }
            else
            {
                _context.Budget.Add(budget);
                return budget;
            }
        }

        protected void CreateExpense(ExpenseSAP expenseSAP, Budget budget)
        {
            Expense expense = new Expense();
            expense.Budget = budget;
            expense.ExpenseName = expenseSAP.expenseName;
            expense.Amount = string.IsNullOrEmpty(expenseSAP.amount) ? 0 : Convert.ToInt32(double.Parse(expenseSAP.amount, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo));

            _context.Expense.Add(expense);
        }

        protected string SetProjectStatus(string SAPStatus, DateTime? startDate, DateTime? endDate)
        {
            String status;
            if (SAPStatus == "4-Completed")
            {
                status = "Completed";
            }
            else
            {
                if (endDate == null)
                {
                    status = "Not Started";
                }
                else if (endDate < System.DateTime.Today)
                {
                    status = "Late";
                }                
                else
                {
                    status = "In Progress";
                }
            }
            return status;
        }

        protected void CreateProject(ProjectSAP projectSAP, Budget budget)
        {
            Project project = new Project();
            project.ProjectSapId = RemoveUnusedDigitFromSAPProjectId(projectSAP.id_SAP);

            String projectOwnerID;
            if (string.IsNullOrEmpty(projectSAP.projectOwnerId))
            {
                project.ProjectOwnerId = null;
            }
            else
            {
                projectOwnerID = projectSAP.projectOwnerId.TrimStart('0');
                project.ProjectOwnerId = (from emp in _context.Employe
                                          where emp.IdSAP == projectOwnerID
                                          select emp.Id).FirstOrDefault();
            }

            String managerID;
            if (string.IsNullOrEmpty(projectSAP.projectManagerId))
            {
                project.ProjectManagerId = null;
            }
            else
            {
                managerID = projectSAP.projectManagerId.TrimStart('0');
                project.ProjectManagerId = (from emp in _context.Employe
                                            where emp.IdSAP == managerID
                                            select emp.Id).FirstOrDefault();
            }

            project.ProjectName = projectSAP.projectNameFr;
            project.Department = projectSAP.department;
            project.Factory = projectSAP.factories;

            if (projectSAP.startDate != "0")
            {
                string d1 = projectSAP.startDate.Insert(4, "/");
                string d2 = d1.Insert(7, "/");
                project.StartDate = Convert.ToDateTime(d2);
            }
            else
            {
                project.StartDate = null;
            }
            if (projectSAP.estEnd != "0")
            {
                string d3 = projectSAP.estEnd.Insert(4, "/");
                string d4 = d3.Insert(7, "/");
                project.EstEndDate = Convert.ToDateTime(d4);
            }
            else
            {
                project.EstEndDate = null;
            }
            project.Budget = budget;
            project.CompletionPercentage = 0;

            project.ProjectStatus = SetProjectStatus(projectSAP.projectStatus, project.StartDate, project.EstEndDate);
            project.ProjectsClient = projectSAP.clients;
            project.Thumbnail = projectSAP.thumbnails;
            project.Priority = projectSAP.priority;
            project.Description = projectSAP.description;

            Project ProjectExists = _context.Project.FirstOrDefault(x => x.ProjectSapId == project.ProjectSapId);

            if (ProjectExists != null)
            {
                if (
                    project.ProjectOwnerId == ProjectExists.ProjectOwnerId &&
                    project.ProjectManagerId == ProjectExists.ProjectManagerId &&
                    project.ProjectName == ProjectExists.ProjectName &&
                    project.Department == ProjectExists.Department &&
                    project.Factory == ProjectExists.Factory &&
                    project.StartDate == ProjectExists.StartDate &&
                    project.EstEndDate == ProjectExists.EstEndDate &&
                    project.ProjectStatus == ProjectExists.ProjectStatus &&
                    project.ProjectsClient == ProjectExists.ProjectsClient &&
                    project.Thumbnail == ProjectExists.Thumbnail &&
                    project.Priority == ProjectExists.Priority &&
                    project.Description == ProjectExists.Description &&
                    project.EstWorkDay == ProjectExists.EstWorkDay)
                {
                    return;
                }
                else
                {
                    //ProjectExists.ProjectOwnerId = project.ProjectOwnerId;
                    //ProjectExists.ProjectManagerId = project.ProjectManagerId;
                    ProjectExists.ProjectName = project.ProjectName;
                    ProjectExists.Department = project.Department;
                    ProjectExists.Factory = project.Factory;
                    ProjectExists.StartDate = project.StartDate;
                    ProjectExists.EstEndDate = project.EstEndDate;
                    ProjectExists.ProjectStatus = project.ProjectStatus;
                    ProjectExists.ProjectsClient = project.ProjectsClient;
                    ProjectExists.Thumbnail = project.Thumbnail;
                    ProjectExists.Priority = project.Priority;
                    ProjectExists.Description = project.Description;
                    _context.Project.Update(ProjectExists);
                    return;
                }
            }
            else
            {
                _context.Project.Add(project);
            }
        }

        protected ProjectNetflix CreateProjectNetflix(Project project)
        {
            ProjectNetflix project_netxlix = new ProjectNetflix();

            var manager = (from p in _context.Employe
                           where p.Id == project.ProjectManagerId
                           select p).FirstOrDefault();

            var budget = (from p in _context.Budget
                          where p.Id == project.BudgetId
                          select p).First();

            //var expenses = (from p in _context.Expense
            //                where p.Budgetid == budget.Id
            //                select p).ToList();

            var connexeProjectIds = (from p in _context.ConnexeProject
                                     where p.ProjectId == project.Id
                                     select p.ConnexeProjectSapid).ToList();

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

            List<ProjectNetflixCard> connexeProjects = new List<ProjectNetflixCard>();
            foreach (String connexeProjectid in connexeProjectIds)
            {
                var pro = _context.Project.FirstOrDefault(t => t.Id == project.Id);
                if (pro == null)
                {
                    continue;
                }
                else
                {
                    var man = (from p in _context.Employe
                               where p.Id == pro.ProjectManagerId
                               select p).FirstOrDefault();

                    ProjectNetflixCard connexeProject = new ProjectNetflixCard()
                    {
                        Id = pro.Id,
                        ProjectName = pro.ProjectName,
                        ProjectOwnerId = project.ProjectOwnerId,
                        ProjectSapId = project.ProjectSapId,
                        ProjectsClient = project.ProjectsClient,
                        ProjectStatus = project.ProjectStatus,
                        StartDate = project.StartDate,
                        Thumbnail = project.Thumbnail,
                        EstEndDate = project.EstEndDate != null ? project.EstEndDate.Value.ToString("MMMM, yyyy") : "n/a",
                        Department = project.Department,
                        CompletionPercentage = project.CompletionPercentage,
                        Factory = project.Factory,
                        ManagerName = man.Name,
                        ManagerPicture = man.Picture,
                    };
                    connexeProjects.Add(connexeProject);
                }
            }
            if (manager == null)
            {
                project_netxlix.ManagerName = "Unknown";
                project_netxlix.ManagerPicture = "http://www.getsmartcontent.com/content/uploads/2014/08/shutterstock_149293433.jpg";
            }
            else
            {
                project_netxlix.ManagerName = manager.Name;
                project_netxlix.ManagerPicture = manager.Picture;
            }

            project_netxlix.Id = project.Id;
            project_netxlix.Priority = project.Priority;
            project_netxlix.ProjectManagerId = project.ProjectManagerId;
            project_netxlix.ProjectName = project.ProjectName;
            project_netxlix.ProjectOwnerId = project.ProjectOwnerId;
            project_netxlix.ProjectSapId = project.ProjectSapId;
            project_netxlix.ProjectsClient = project.ProjectsClient;
            project_netxlix.ProjectStatus = project.ProjectStatus;
            project_netxlix.StartDate = project.StartDate != null ? project.StartDate.Value.ToString("dd MMMM, yyyy") : "n/a";
            project_netxlix.Thumbnail = project.Thumbnail;
            project_netxlix.EstEndDate = project.EstEndDate != null ? project.EstEndDate.Value.ToString("dd MMMM, yyyy") : "n/a";
            project_netxlix.Description = project.Description;
            project_netxlix.Department = project.Department;
            project_netxlix.CompletionPercentage = project.CompletionPercentage;
            project_netxlix.Factory = project.Factory;
            project_netxlix.InitialBudget = budget.InitialBudget;
            project_netxlix.BudgetSpent = budget.BudgetSpent;
            project_netxlix.BudgetLeft = budget.BudgetLeft;
            project_netxlix.EstWorkDay = project.EstWorkDay;
            project_netxlix.Expenses = expenses_netflix;
            project_netxlix.ConnexeProject = connexeProjects;

            return project_netxlix;
        }

        protected ProjectNetflixContributor CreateNetflixProjectContributor(NotificationPartner partner)
        {
            int totalEffort = 0;
            var potentielDaysOfWork = 0;
            DateTime latestDate = DateTime.Today;
            DateTime StartDate = DateTime.Today;

            var employee = (from p in _context.Employe
                            where p.Id == partner.EmployeId
                            select p).First();

            //var tasks = (from p in _context.TaskOwner
            //             join t in _context.Task on p.TaskId equals t.Id
            //             where p.EmployeId == partner.EmployeId
            //             select new { taskOwner = p, task = t }).ToList();

            //foreach (var task in tasks)
            //{
            //    if (task.task.Status != "Completed")
            //    {
            //        totalEffort = totalEffort + task.task.EstEffort;
            //        if (latestDate < task.task.EstEnd)
            //        {
            //            latestDate = task.task.EstEnd;
            //        }
            //    }
            //}

            //while (StartDate <= latestDate)
            //{
            //    if (StartDate.DayOfWeek != DayOfWeek.Saturday && StartDate.DayOfWeek != DayOfWeek.Sunday)
            //    {
            //        ++potentielDaysOfWork;
            //    }
            //    StartDate = StartDate.AddDays(1);
            //}
            //var availableTimeForProjects = potentielDaysOfWork * (employee.Workload / 5) * employee.ProjectWorkRatio / 100;
            //var occupancyRate = totalEffort / availableTimeForProjects * 100;

            ProjectNetflixContributor contributor_netflix = new ProjectNetflixContributor()
            {
                Id = partner.Id,
                Department = employee.Department,
                Name = employee.Name,
                EmployeeId = employee.IdSAP,
                Picture = employee.Picture,
                Title = employee.Title,
                OccupancyRate = 0
            };
            return contributor_netflix;
        }

        protected ProjectNetflixCard CreateNetflixProjectCard(Project project)
        {
            ProjectNetflixCard project_netxlix_card = new ProjectNetflixCard();
            var manager = (from p in _context.Employe
                           where p.Id == project.ProjectManagerId
                           select p).FirstOrDefault();
            if (manager == null)
            {
                project_netxlix_card.ManagerName = "Unknown";
                project_netxlix_card.ManagerPicture = "http://www.getsmartcontent.com/content/uploads/2014/08/shutterstock_149293433.jpg";
            }
            else
            {
                project_netxlix_card.ManagerName = manager.Name;
                project_netxlix_card.ManagerPicture = manager.Picture;
            }

            project_netxlix_card.Id = project.Id;
            project_netxlix_card.ProjectManagerId = project.ProjectManagerId;
            project_netxlix_card.ProjectName = project.ProjectName;
            project_netxlix_card.ProjectOwnerId = project.ProjectOwnerId;
            project_netxlix_card.ProjectSapId = project.ProjectSapId;
            project_netxlix_card.ProjectsClient = project.ProjectsClient;
            project_netxlix_card.ProjectStatus = project.ProjectStatus;
            project_netxlix_card.StartDate = project.StartDate;
            project_netxlix_card.Thumbnail = project.Thumbnail;
            project_netxlix_card.EstEndDate = project.EstEndDate != null ? project.EstEndDate.Value.ToString("MMMM, yyyy") : "n/a";
            project_netxlix_card.Department = project.Department;
            project_netxlix_card.CompletionPercentage = project.CompletionPercentage;
            project_netxlix_card.Factory = project.Factory;

            return project_netxlix_card;
        }

        protected String RemoveUnusedDigitFromSAPProjectId(String projectSAPidWithDigit)
        {
            String projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        protected string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        protected List<Project> VerifySorting(List<Project> projectToBeSorted, string sorting)
        {
            List<Project> projectSorted = new List<Project>();
            foreach (Project project in projectToBeSorted)
            {

                ProjectNetflix project_netxlix = CreateProjectNetflix(project);

                if (exceedingBudgetSorting.Contains(sorting))
                {
                    if (project_netxlix.BudgetLeft < 0)
                    {
                        projectSorted.Add(project);
                        continue;
                    }
                }
                else if (exceedingXAmountSorting.Contains(sorting))
                {
                    if (project_netxlix.InitialBudget > 100000)
                    {
                        projectSorted.Add(project);
                        continue;
                    }
                }
                else if (lateSorting.Contains(sorting))
                {
                    if (project.EstEndDate < System.DateTime.Today)
                    {
                        projectSorted.Add(project);
                        continue;
                    }
                }
                else if (sorting == "undefined")
                {
                    projectSorted.Add(project);
                    continue;
                }
            }
            return projectSorted;
        }

        protected List<Notification> getProjectNotifications(long id)
        {
            int? projectId = (from p in _context.Project
                              where p.ProjectSapId == id.ToString()
                              select p.Id).FirstOrDefault();

            List<Notification> projectNotificaitions = (from p in _context.Notification
                                                  where p.ProjectId == projectId
                                                  select p).ToList();

            return projectNotificaitions;
        }

        protected List<NotificationPartner> getNotificationPartner(Notification notification)
        {
            List<NotificationPartner> notificaitionPartners = (from p in _context.NotificationPartner
                                                               where p.NotificationId == notification.Id
                                                               select p).ToList();

            return notificaitionPartners;
        }

        protected List<NotificationPartner> getAllPartnersFromNotifications(List<Notification> notifications)
        {
            List<NotificationPartner> partners = new List<NotificationPartner>();
            foreach (Notification notification in notifications)
            {
                List<NotificationPartner> p = getNotificationPartner(notification);
                foreach(NotificationPartner partner in p)
                {
                    partners.Add(partner);
                }
            }
            List<NotificationPartner> noDupesPartners = partners.GroupBy(x => x.EmployeId).Select(y => y.First()).ToList();
            return noDupesPartners;
        }

        [HttpGet]
        public IEnumerable<Models.ProjectNetflixCard> Get()
        {
            List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();

            foreach (Project project in _context.Project)
            {
                ProjectNetflixCard project_netxlix_card = CreateNetflixProjectCard(project);
                projects.Add(project_netxlix_card);
            }
            return projects;
        }

        [HttpGet("{id}", Name = "GetProject")]
        public IActionResult GetById(long id)
        {
            var project = _context.Project.FirstOrDefault(t => t.ProjectSapId == id.ToString());
            if (project == null)
            {
                return NotFound();
            }
            else
            {
                ProjectNetflix project_netxlix = CreateProjectNetflix(project);
                return new ObjectResult(project_netxlix);
            }
        }

        [HttpGet("{id}/Contributor", Name = "GetByProjectIdContributors")]
        public IEnumerable<Models.ProjectNetflixContributor> GetProjectContributor(long id)
        {
            List<ProjectNetflixContributor> projectContributor = new List<ProjectNetflixContributor>();
            List<Notification> projectNotifications = getProjectNotifications(id);
            List<NotificationPartner> partners = getAllPartnersFromNotifications(projectNotifications);
           
            foreach (NotificationPartner partner in partners)
            {
                ProjectNetflixContributor contributor_netflix = CreateNetflixProjectContributor(partner);
                projectContributor.Add(contributor_netflix);
            }
            return projectContributor;
        }

        [HttpGet("employee/{id}", Name = "GetByEmployeeIdAllWorkingProjects")]
        public IEnumerable<ProjectNetflixCard> GetEmployeeAllWorkingProjects(string id)
        {
            List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();

            int employeeId = (from p in _context.Employe
                              where p.IdSAP == id
                              select p.Id).FirstOrDefault();

            List<NotificationPartner> partners = (from p in _context.NotificationPartner
                                                  where p.EmployeId == employeeId
                                                  select p).ToList();

            List<int> partnerIdAlreadyAdded = new List<int>();
            List<Notification> notifications = new List<Notification>();

            foreach (var partner in partners)
            {
                if (!partnerIdAlreadyAdded.Contains(partner.NotificationId))
                {
                    Notification notification = (from p in _context.Notification
                                                 where p.Id == partner.NotificationId
                                                 select p).FirstOrDefault();
                    notifications.Add(notification);
                    partnerIdAlreadyAdded.Add(partner.NotificationId);
                }
            }

            List<int> projectAlreadyAdded = new List<int>();
            foreach (Notification notification in notifications)
            {
                var project = (from p in _context.Project
                               where p.Id == notification.ProjectId
                               select p).FirstOrDefault();

                if (!projectAlreadyAdded.Contains(project.Id))
                {
                    var p = CreateNetflixProjectCard(project);

                    projects.Add(p);
                    projectAlreadyAdded.Add(p.Id);
                }
            }

            return projects;
        }

        [HttpGet("Exceeding-budget", Name = "GetExceedingBudgetProject")]
        public IEnumerable<Models.ProjectNetflixCard> GetExceedingBudgetProject()
        {
            List<ProjectNetflixCard> exceedingBudgetProjects = new List<ProjectNetflixCard>();

            foreach (Project project in _context.Project)
            {
                var budget = (from p in _context.Budget
                              where p.Id == project.BudgetId
                              select p).First();

                if (budget.BudgetLeft < 0)
                {
                    ProjectNetflixCard project_netxlix_card = CreateNetflixProjectCard(project);
                    exceedingBudgetProjects.Add(project_netxlix_card);
                }
            }
            return exceedingBudgetProjects;
        }

        [HttpGet("Overdue", Name = "GetOverdueProject")]
        public IEnumerable<Models.ProjectNetflixCard> GetOverdueProject()
        {
            List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();

            foreach (Project project in _context.Project)
            {
                if (project.ProjectStatus == "Late")
                {
                    ProjectNetflixCard project_netxlix = CreateNetflixProjectCard(project);
                    projects.Add(project_netxlix);
                }
            }
            return projects;
        }

        [HttpGet("Filter", Name = "GetFilteredProject")]
        public IEnumerable<Models.ProjectNetflix> GetFilteredProject(string facility, string sorting, string department)
        {
            List<ProjectNetflix> netflixProjectsFiltred = new List<ProjectNetflix>();
            List<Project> projects = new List<Project>();
            projects = (_context.Project).ToList();
            if (!allFacilitySorting.Contains(facility))
            {
                string facilityId = facility.Substring(0, 4);
                List<Project> facilityProjects = (from p in _context.Project
                                                  where p.Factory.Substring(0, 4) == facility
                                                  select p).ToList();
                projects = facilityProjects;
            }
            if (department != "undefined")
            {
                string departmentId = department.Substring(0, 3);
                if (departmentId != "All")
                {
                    List<Project> departamentalProject = new List<Project>();
                    foreach (Project project in projects)
                    {
                        if (project.Department != "")
                        {
                            if (project.Department.Substring(0, 3) == departmentId)
                            {
                                departamentalProject.Add(project);
                            }
                        }
                    }
                    projects = departamentalProject;
                }
            }
            if (sorting != "undefined")
            {
                projects = VerifySorting(projects, sorting);
            }
            foreach (Project project in projects)
            {
                netflixProjectsFiltred.Add(CreateProjectNetflix(project));
            }

            return netflixProjectsFiltred;
        }

        [HttpGet("Refresh", Name = "RefreshProjects")]
        public async Task<IActionResult> RefreshProjects()
        {
            IEnumerable<ProjectSAP> Projects = new List<ProjectSAP>();
            List<String> ExistingProjects = new List<String>();
            ExistingProjects = (from p in _context.Project select p.ProjectSapId).ToList();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = await client.GetAsync(string.Concat("http://api.dev.gbl/v3/", "projects"));
                data.EnsureSuccessStatusCode();
                var stringResult = await data.Content.ReadAsStringAsync();
                Projects = JsonConvert.DeserializeObject<IEnumerable<ProjectSAP>>(stringResult);

                foreach (ProjectSAP projectSAP in Projects)
                {
                    if (projectSAP.projectStatus == "1-En attente" ||
                        projectSAP.projectStatus == "2-En cours" ||
                        projectSAP.projectStatus == "3-Terminé")
                    {
                        Budget budget = new Budget();
                        budget = CreateBudget(projectSAP.budget[0]);

                        //foreach (ExpenseSAP exp in projectSAP.budget[0].expensesBudget)
                        //{
                        //    //ToDO verifier si expense exists déja ds la bd
                        //    CreateExpense(exp, budget);
                        //}
                        CreateProject(projectSAP, budget);
                        ExistingProjects.Remove(RemoveUnusedDigitFromSAPProjectId(projectSAP.id_SAP));
                    }
                    else
                    {
                        continue;
                    }
                }
                if (ExistingProjects.Any())
                {
                    foreach (String projectSAPId in ExistingProjects)
                    {
                        Project projectToBeDeleted = (from p in _context.Project where p.ProjectSapId == projectSAPId select p).FirstOrDefault();
                        if (projectToBeDeleted != null)
                        {
                            _context.Project.Remove(projectToBeDeleted);
                        }
                    }
                }
                _context.SaveChanges();
                return new ObjectResult("Successfully added to data base...");
            }
        }
    }
}
