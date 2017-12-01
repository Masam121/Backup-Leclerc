using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ProjectDashboardAPI.Services
{
    public class ProjectService : IProjectService
    {
        private IProjectRepository _projectRepository;
        private INotificationRepository _notificationRepository;
        private INotificationPartnerRepository _notificaiotnPartnerRepository;
        private IEmployeeRepository _employeeRepository;
        private ISapService _sapService;
        private IBudgetRepository _budgetRepository;

        //Sorting Lists
        private List<string> allFacilitySorting = new List<string> { "1", "undefined" };
        private List<string> exceedingBudgetSorting = new List<string> { "En dépassement de budget", "Exceeding budget", "Exceder el persupuesto" };
        private List<string> exceedingXAmountSorting = new List<string> { "Coutant plus de 100 000$", "Cost more than 100 000$", "Coste de más de 100 000 $" };
        private List<string> lateSorting = new List<string> { "En retard", "Late", "Tarde" };

        public ProjectService(IProjectRepository projectRepository, 
                              INotificationRepository notificationRepository, 
                              INotificationPartnerRepository notificationPartnerRepository,
                              IEmployeeRepository employeeRepository,
                              IBudgetRepository budgetRepository,
                              ISapService sapService)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _notificaiotnPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _budgetRepository = budgetRepository ?? throw new ArgumentNullException(nameof(budgetRepository));

            _sapService = sapService ?? throw new ArgumentNullException(nameof(sapService));
        }

        public List<NotificationPartner> GetAllPartnersFromNotifications(IEnumerable<Notification> notifications)
        {
            List<NotificationPartner> partners = new List<NotificationPartner>();
            foreach (Notification notification in notifications)
            {
                var notificationPartners = _notificaiotnPartnerRepository.ReadManyPartnersByNotification(notification);
                foreach (NotificationPartner partner in notificationPartners.Result)
                {
                    partners.Add(partner);
                }
            }
            List<NotificationPartner> noDupesPartners = partners.GroupBy(x => x.EmployeId).Select(y => y.First()).ToList();
            return noDupesPartners;
        }

        public List<Project> VerifySorting(IEnumerable<Project> projectToBeSorted, string sorting)
        {
            List<Project> projectSorted = new List<Project>();
            foreach (Project project in projectToBeSorted)
            {

                ProjectNetflix project_netxlix = (_projectRepository.CreateProjectNetflix(project)).Result;

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

        public String RemoveUnusedDigitFromSAPProjectId(String projectSAPidWithDigit)
        {
            String projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        public async Task<Project> CreateProject(ProjectSAP projectSAP, Budget budget)
        {
            Project project = await _projectRepository.CreateProject(projectSAP, budget);
            return project;
        }

        public async Task<ProjectNetflix> CreateProjectNetflix(Project project)
        {
            ProjectNetflix projectNetflix = await _projectRepository.CreateProjectNetflix(project);
            return projectNetflix;
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetAllProjectNetflixCard()
        {
            var allProjects = _projectRepository.ReadManyAsync();
            List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();

            foreach (Project project in allProjects.Result)
            {
                ProjectNetflixCard project_netxlix_card = await _projectRepository.CreateProjectNetflixCard(project);
                projects.Add(project_netxlix_card);
            }

            return projects;
        }

        public async Task<Project> GetProjectById(string id)
        {
            var project = await _projectRepository.ReadOneAsyncBySAPId(id);
            return project;
        }

        public async Task<IEnumerable<ProjectNetflixContributor>> GetProjectContributors(long id)
        {
            List<ProjectNetflixContributor> projectContributor = new List<ProjectNetflixContributor>();

            var projectNotifications = await _projectRepository.ReadManyAsyncNotificationByProjectId(id);
            List<NotificationPartner> partners = GetAllPartnersFromNotifications(projectNotifications);

            foreach (NotificationPartner partner in partners)
            {
                ProjectNetflixContributor contributor_netflix = await _notificaiotnPartnerRepository.CreateProjectNetflixContributor(partner);
                projectContributor.Add(contributor_netflix);
            }

            return projectContributor;
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetEmployeeAllWorkingProjects(string id)
        {
            List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();
            var employeeId = _employeeRepository.ReadAsyncEmployeeId(id);

            List<NotificationPartner> partners = await _notificaiotnPartnerRepository.ReadAsyncPartnerByEmployeeId(employeeId.Result);           

            List<int> partnerIdAlreadyAdded = new List<int>();
            List<Notification> notifications = new List<Notification>();

            foreach (var partner in partners)
            {
                if (!partnerIdAlreadyAdded.Contains(partner.NotificationId))
                {
                    Notification notification = await _notificationRepository.ReadOneAsyncNotificationById(partner.NotificationId);
                    
                    notifications.Add(notification);
                    partnerIdAlreadyAdded.Add(partner.NotificationId);
                }
            }

            List<int> projectAlreadyAdded = new List<int>();
            foreach (Notification notification in notifications)
            {
                var project = _projectRepository.ReadOneAsyncById(notification.ProjectId);

                if (!projectAlreadyAdded.Contains(project.Id))
                {
                    var p = _projectRepository.CreateProjectNetflixCard(project.Result);

                    projects.Add(p.Result);
                    projectAlreadyAdded.Add(p.Id);
                }
            }

            return projects;
        }

        public Task<IEnumerable<ProjectNetflixCard>> GetAllExceedingBudgetProjects()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetOverdueProjects()
        {
            List<ProjectNetflixCard> projectNetflixCars = new List<ProjectNetflixCard>();

            var projects = await _projectRepository.ReadManyAsync();

            foreach (Project project in projects)
            {
                if (project.ProjectStatus == "Late")
                {
                    ProjectNetflixCard project_netxlix = await _projectRepository.CreateProjectNetflixCard(project);
                    projectNetflixCars.Add(project_netxlix);
                }
            }
            return projectNetflixCars;
        }

        public async Task<IEnumerable<ProjectNetflix>> GetFiltredProject(string facility, string sorting, string department)
        {
            List<ProjectNetflix> netflixProjectsFiltred = new List<ProjectNetflix>();
            IEnumerable<Project> projects = await _projectRepository.ReadManyAsync();

            if (!allFacilitySorting.Contains(facility))
            {
                IEnumerable<Project> facilityProjects = await _projectRepository.ReadManyAsyncByFacilityId(facility);
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
                netflixProjectsFiltred.Add((_projectRepository.CreateProjectNetflix(project)).Result);
            }
            return netflixProjectsFiltred;
        }

        public async Task<IActionResult> RefreshProjectsData()
        {                  
            try
            {
                IEnumerable<ProjectSAP> projectsSap =  await _sapService.GetSapProject();
                List<String> ExistingProjects = (await _projectRepository.ReadManyAsyncProjectSAPId()).ToList();

                foreach (ProjectSAP projectSAP in projectsSap)
                {
                    if (projectSAP.projectStatus == "1-En attente" ||
                        projectSAP.projectStatus == "2-En cours" ||
                        projectSAP.projectStatus == "3-Terminé")
                    {
                        Budget budget = await _budgetRepository.CreateBudget(projectSAP.budget[0]);

                        var project =  await CreateProject(projectSAP, budget);
                        if (await _projectRepository.VerifiyIfProjectExists(project))
                        {
                            if(await _projectRepository.VerifiyIfProjectAsBeenUpdated(project))
                            {
                                _projectRepository.UpdateProject(project);
                            }
                        }
                        else
                        {
                            _projectRepository.AddProject(project);
                        }
                        ExistingProjects.Remove(RemoveUnusedDigitFromSAPProjectId(projectSAP.id_SAP));
                    }
                }
                if (ExistingProjects.Any())
                {
                    foreach (String projectSAPId in ExistingProjects)
                    {
                        Project projectToBeDeleted = await _projectRepository.ReadOneAsyncBySAPId(projectSAPId);

                        if (projectToBeDeleted != null)
                        {
                            _projectRepository.DeleteProject(projectToBeDeleted);
                        }
                    }
                }
                _projectRepository.SaveData();
                return new ObjectResult("Successfully refreshed...");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /*public async Task<IEnumerable<ProjectNetflixCard>> GetAllExceedingBudgetProjects()
        {
            List<ProjectNetflixCard> blabla = new List<ProjectNetflixCard>();
            var projects = await _projectRepository.ReadAllAsync();

            foreach (Project project in projects.Result)
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

            return new NotImplementedException();
        }*/
    }
}
