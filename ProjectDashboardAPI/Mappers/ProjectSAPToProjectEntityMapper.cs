using NetflixAPI.Models;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class ProjectSAPToProjectEntityMapper : IMapper<ProjectSAP, Project>
    {
        private IEmployeeRepository _employeeRepository;

        public ProjectSAPToProjectEntityMapper(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public string SetProjectStatus(string SAPStatus, DateTime? startDate, DateTime? endDate)
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

        public String RemoveUnusedDigitFromSAPProjectId(String projectSAPidWithDigit)
        {
            String projectSAPIdwithoutUnusedDigit = projectSAPidWithDigit.Remove(projectSAPidWithDigit.Length - 5);
            projectSAPIdwithoutUnusedDigit = projectSAPIdwithoutUnusedDigit.Substring(23, projectSAPIdwithoutUnusedDigit.Length - 23);

            return projectSAPIdwithoutUnusedDigit;
        }

        public Project Map(ProjectSAP project)
        {
            var entity = new Project();
            entity.ProjectSapId = RemoveUnusedDigitFromSAPProjectId(project.id_SAP);

            String projectOwnerID;
            if (string.IsNullOrEmpty(project.projectOwnerId))
            {
                entity.ProjectOwnerId = null;
            }
            else
            {
                projectOwnerID = project.projectOwnerId.TrimStart('0');
                entity.ProjectOwnerId = _employeeRepository.ReadAsyncEmployeeId(projectOwnerID).Result;
            }

            String managerID;
            if (string.IsNullOrEmpty(project.projectManagerId))
            {
                entity.ProjectManagerId = null;
            }
            else
            {
                managerID = project.projectManagerId.TrimStart('0');
                entity.ProjectManagerId = _employeeRepository.ReadAsyncEmployeeId(managerID).Result;
            }

            entity.ProjectName = project.projectNameFr;
            entity.Department = project.department;
            entity.Factory = project.factories;

            if (project.startDate != "0")
            {
                string d1 = project.startDate.Insert(4, "/");
                string d2 = d1.Insert(7, "/");
                entity.StartDate = Convert.ToDateTime(d2);
            }
            else
            {
                entity.StartDate = null;
            }
            if (project.estEnd != "0")
            {
                string d3 = project.estEnd.Insert(4, "/");
                string d4 = d3.Insert(7, "/");
                entity.EstEndDate = Convert.ToDateTime(d4);
            }
            else
            {
                entity.EstEndDate = null;
            }
            entity.CompletionPercentage = 0;

            entity.ProjectStatus = SetProjectStatus(project.projectStatus, entity.StartDate, entity.EstEndDate);
            entity.ProjectsClient = project.clients;
            entity.Thumbnail = project.thumbnails;
            entity.Priority = project.priority;
            entity.Description = project.description;

            return entity;
        }
    }
}
