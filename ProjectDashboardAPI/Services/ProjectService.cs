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
        private ITaskRepository _taskRepository;
        private ITaskOwnerRepository _taskOwnerRepository;

        //Sorting Lists
        private List<string> allFacilitySorting = new List<string> { "1", "undefined" };

        private List<string> Administration = new List<string> { "050", "060", "070", "150", "160", "910", "950", "960", "970" };
        private List<string> Support = new List<string> { "370", "530", "550", "650", "670", "710", "750", "790" };
        private List<string> Logistique = new List<string> { "450", "820", "850" };
        private List<string> Ventes = new List<string> { "230", "250", "280", "290", "350"};

        private List<string> exceedingBudgetSorting = new List<string> { "En dépassement de budget", "Exceeding budget", "Exceder el persupuesto" };
        private List<string> exceedingXAmountSorting = new List<string> { "Coutant plus de 100 000$", "Cost more than 100 000$", "Coste de más de 100 000 $" };
        private List<string> lateSorting = new List<string> { "En retard", "Late", "Tarde" };

        public ProjectService(IProjectRepository projectRepository, 
                              INotificationRepository notificationRepository, 
                              INotificationPartnerRepository notificationPartnerRepository,
                              IEmployeeRepository employeeRepository,
                              IBudgetRepository budgetRepository,
                              ISapService sapService,
                              ITaskRepository taskRepository,
                              ITaskOwnerRepository taskOwnerRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _notificaiotnPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _budgetRepository = budgetRepository ?? throw new ArgumentNullException(nameof(budgetRepository));
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
            _taskOwnerRepository = taskOwnerRepository ?? throw new ArgumentNullException(nameof(taskOwnerRepository));

            _sapService = sapService ?? throw new ArgumentNullException(nameof(sapService));
        }

        public List<NotificationPartner> GetAllPartnersFromNotifications( netflix_prContext context, IEnumerable<Notification> notifications)
        {
            List<NotificationPartner> partners = new List<NotificationPartner>();
            foreach (Notification notification in notifications)
            {
                var notificationPartners = _notificaiotnPartnerRepository.ReadManyPartnersByNotification(context, notification);
                foreach (NotificationPartner partner in notificationPartners.Result)
                {
                    partners.Add(partner);
                }
            }
            List<NotificationPartner> noDupesPartners = partners.GroupBy(x => x.EmployeId).Select(y => y.First()).ToList();
            return noDupesPartners;           
        }

        public async Task<List<Project>> verifyAxeOrganisationnal(netflix_prContext context, IEnumerable<Project> projectToBeSorted, string axeOrganisationnal)
        {
            List<Project> projectSorted = new List<Project>();
            foreach (Project project in projectToBeSorted)
            {
                ProjectNetflix project_netxlix = await (_projectRepository.CreateProjectNetflix(context, project));
                if(project_netxlix.Department == "")
                {
                    continue;
                }
                string departmentId = project_netxlix.Department.Substring(0, 3);

                if (axeOrganisationnal == "Administration")
                {
                    if (Administration.Contains(departmentId))
                    {
                        projectSorted.Add(project);
                    }
                }
                if (axeOrganisationnal == "Ventes")
                {
                    if (Ventes.Contains(departmentId))
                    {
                        projectSorted.Add(project);
                    }
                }
                if (axeOrganisationnal == "Logistique")
                {
                    if (Logistique.Contains(departmentId))
                    {
                        projectSorted.Add(project);
                    }
                }
                if (axeOrganisationnal == "Support")
                {
                    if (Support.Contains(departmentId))
                    {
                        projectSorted.Add(project);
                    }
                }
            }
            return projectSorted;
        }

            //public List<Project> VerifySorting(netflix_prContext context, IEnumerable<Project> projectToBeSorted, string sorting)
            //{
            //    List<Project> projectSorted = new List<Project>();
            //    foreach (Project project in projectToBeSorted)
            //    {

            //        ProjectNetflix project_netxlix = (_projectRepository.CreateProjectNetflix(context, project)).Result;

            //        if (exceedingBudgetSorting.Contains(sorting))
            //        {
            //            if (project_netxlix.BudgetLeft < 0)
            //            {
            //                projectSorted.Add(project);
            //                continue;
            //            }
            //        }
            //        else if (exceedingXAmountSorting.Contains(sorting))
            //        {
            //            if (project_netxlix.InitialBudget > 100000)
            //            {
            //                projectSorted.Add(project);
            //                continue;
            //            }
            //        }
            //        else if (lateSorting.Contains(sorting))
            //        {
            //            if (project.EstEndDate < System.DateTime.Today)
            //            {
            //                projectSorted.Add(project);
            //                continue;
            //            }
            //        }
            //        else if (sorting == "undefined")
            //        {
            //            projectSorted.Add(project);
            //            continue;
            //        }
            //    }
            //    return projectSorted;              
            //}

        public String RemoveUnusedDigitFromSAPProjectId(String projectSAPidWithDigit)
        {
            String projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);
            return projectSAPIdwithoutUnusedDigit;
        }

        public async Task<Project> CreateProject(netflix_prContext context, ProjectSAP projectSAP, Budget budget)
        {
            Project project = await _projectRepository.CreateProject(context ,projectSAP, budget);
            return project;               
        }

        public async Task<ProjectNetflix> CreateProjectNetflix(netflix_prContext context, Project project)
        {
            ProjectNetflix projectNetflix = await _projectRepository.CreateProjectNetflix(context, project);
            return projectNetflix;
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetAllProjectNetflixCard()
        {
            using (var context = new netflix_prContext())
            {
                var allProjects = _projectRepository.ReadManyAsync(context);
                List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();

                foreach (Project project in allProjects.Result)
                {
                    ProjectNetflixCard project_netxlix_card = await _projectRepository.CreateProjectNetflixCard(context, project);
                    projects.Add(project_netxlix_card);
                }

                return projects;
            }              
        }

        public async Task<ProjectNetflix> GetProjectById(string id)
        {
            using (var context = new netflix_prContext())
            {
                var project = await _projectRepository.ReadOneAsyncBySAPId(context, id);

                var projectNetflixDto = await CreateProjectNetflix(context, project);
                return projectNetflixDto;
            }                
        }

        public async Task<IEnumerable<ProjectNetflixContributor>> GetProjectContributors(long id)
        {
            using (var context = new netflix_prContext())
            {
                List<ProjectNetflixContributor> projectContributor = new List<ProjectNetflixContributor>();

                var projectNotifications = await _projectRepository.ReadManyAsyncNotificationByProjectId(context, id);
                List<NotificationPartner> partners = GetAllPartnersFromNotifications(context, projectNotifications);

                foreach (NotificationPartner partner in partners)
                {
                    ProjectNetflixContributor contributor_netflix = await _notificaiotnPartnerRepository.CreateProjectNetflixContributor(context, partner);
                    projectContributor.Add(contributor_netflix);
                }

                return projectContributor;
            }                
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetEmployeeAllWorkingProjects(string id)
        {
            using (var context = new netflix_prContext())
            {
                List<ProjectNetflixCard> projects = new List<ProjectNetflixCard>();
                var employeeId = _employeeRepository.ReadAsyncEmployeeId(context, id);

                List<NotificationPartner> partners = await _notificaiotnPartnerRepository.ReadAsyncPartnerByEmployeeId(context, employeeId.Result);

                List<int> partnerIdAlreadyAdded = new List<int>();
                List<int> projectAlreadyAdded = new List<int>();

                foreach (var partner in partners)
                {
                    if (!partnerIdAlreadyAdded.Contains(partner.NotificationId))
                    {
                        Notification notification = await _notificationRepository.ReadOneAsyncNotificationById(context, partner.NotificationId);
                        partnerIdAlreadyAdded.Add(partner.NotificationId);

                        Project project = await _projectRepository.ReadOneAsyncById(context, notification.ProjectId);

                        if (!projectAlreadyAdded.Contains(project.Id))
                        {
                            ProjectNetflixCard p = await _projectRepository.CreateProjectNetflixCard(context, project);

                            projects.Add(p);
                            projectAlreadyAdded.Add(project.Id);
                        }
                    }
                }             
                return projects;
            }               
        }

        public Task<IEnumerable<ProjectNetflixCard>> GetAllExceedingBudgetProjects()
        {
            using (var context = new netflix_prContext())
            {
                throw new NotImplementedException();
            }                
        }

        public async Task<IEnumerable<ProjectNetflixCard>> GetOverdueProjects()
        {
            using (var context = new netflix_prContext())
            {
                List<ProjectNetflixCard> projectNetflixCars = new List<ProjectNetflixCard>();

                var projects = await _projectRepository.ReadManyAsync(context);

                foreach (Project project in projects)
                {
                    if (project.ProjectStatus == "Late")
                    {
                        ProjectNetflixCard project_netxlix = await _projectRepository.CreateProjectNetflixCard(context, project);
                        projectNetflixCars.Add(project_netxlix);
                    }
                }
                return projectNetflixCars;
            }
                
        }

        public async Task<IEnumerable<ProjectNetflix>> GetFiltredProject(string facility, string axeOrganisational, string department)
        {
            using (var context = new netflix_prContext())
            {
                List<ProjectNetflix> netflixProjectsFiltred = new List<ProjectNetflix>();
                IEnumerable<Project> projects = await _projectRepository.ReadManyAsync(context);

                if (!allFacilitySorting.Contains(facility))
                {
                    IEnumerable<Project> facilityProjects = await _projectRepository.ReadManyAsyncByFacilityId(context, facility);
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
                if (axeOrganisational != "undefined")
                {
                    projects = await verifyAxeOrganisationnal(context, projects, axeOrganisational);
                }
                foreach (Project project in projects)
                {
                    netflixProjectsFiltred.Add((await _projectRepository.CreateProjectNetflix(context, project)));
                }
                return netflixProjectsFiltred;
            }               
        }

        public async Task<IActionResult> RefreshProjectsData()
        {
            using (var context = new netflix_prContext())
            {
                try
                {
                    IEnumerable<ProjectSAP> projectsSap = await _sapService.GetSapProject();
                    List<String> ExistingProjects = (await _projectRepository.ReadManyAsyncProjectSAPId(context)).ToList();

                    foreach (ProjectSAP projectSAP in projectsSap)
                    {
                        if (projectSAP.projectStatus == "1-En attente" ||
                            projectSAP.projectStatus == "2-En cours" ||
                            projectSAP.projectStatus == "3-Terminé")
                        {
                            Budget budget = await _budgetRepository.CreateBudget(context, projectSAP.budget[0]);

                            var project = await CreateProject(context, projectSAP, budget);
                            if (await _projectRepository.VerifiyIfProjectExists(context, project))
                            {
                                if (await _projectRepository.VerifiyIfProjectAsBeenUpdated(context, project))
                                {
                                    _projectRepository.UpdateProject(context, project);
                                }
                            }
                            else
                            {
                                _projectRepository.AddProject(context, project);
                            }
                            ExistingProjects.Remove(RemoveUnusedDigitFromSAPProjectId(projectSAP.id_SAP));
                        }
                    }
                    if (ExistingProjects.Any())
                    {
                        foreach (String projectSAPId in ExistingProjects)
                        {
                            Project projectToBeDeleted = await _projectRepository.ReadOneAsyncBySAPId(context, projectSAPId);

                            if (projectToBeDeleted != null)
                            {
                                _projectRepository.DeleteProject(context, projectToBeDeleted);
                                List<Notification> notificationsToBeDeleted =  await _notificationRepository.ReadManyAsyncProjectNotification(context, projectToBeDeleted.Id);
                                foreach(Notification notification in notificationsToBeDeleted)
                                {
                                    _notificationRepository.DeleteNotification(context, notification);
                                    List<NotificationPartner> partnersToBeDeleted = await _notificaiotnPartnerRepository.ReadManyPartnersByNotification(context, notification);
                                    foreach (NotificationPartner partner in partnersToBeDeleted)
                                    {
                                        _notificaiotnPartnerRepository.DeletePartner(context, partner);
                                    }
                                    List<Task> taskToBeDeleted =  await _taskRepository.ReadManyAsyncTaskByNotificationId(context, notification.Id);
                                    foreach (Task task in taskToBeDeleted)
                                    {
                                        _taskRepository.DeleteTask(context, task);
                                        TaskOwner taskOnwerToBeDeleted = await _taskOwnerRepository.ReadOneAsyncTaskOwnerByTaskId(context, task.Id);
                                        if(taskOnwerToBeDeleted != null)
                                        {
                                            _taskOwnerRepository.DeleteTaskOwner(context, taskOnwerToBeDeleted);
                                        }                                       
                                    }
                                }                                
                            }
                        }
                    }
                    context.SaveChanges();
                    return new ObjectResult("Successfully refreshed Projects...");
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }               
        }
    }
}
