using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetflixAPI.Models;
using Microsoft.Exchange.WebServices.Data;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Services;

namespace NetflixAPI.Controllers
{
    [Route("api/[controller]")]
    public class OutlookController : Controller
    {
        private readonly IOutlookService _outlookService;

        public OutlookController(IOutlookService outlookService)
        {
            _outlookService = outlookService ?? throw new ArgumentNullException(nameof(outlookService));
        }

        [HttpGet("Refresh", Name = "RefreshOutlookTask")]
        public async Task<IActionResult> RefreshOutlookTask()
        {
            try
            {
                var response = await _outlookService.RefreshOutlookTask();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        string[] _Users = new string[] { "christopherb@leclerc.ca", "marcelsm@leclerc.ca", "yanickm@leclerc.ca" };
        //List<int> projectIds = new List<int>();
        //"valeriec@leclerc.ca", "marcelsm@leclerc.ca", "christopherb@leclerc.ca", "martinL@leclerc.ca", "antoineBo@leclerc.ca", "annej@leclerc.ca", "Lemieux@leclerc.ca", "Jean-Francois@leclerc.ca" 

        //protected void RecalculatCompletionPercentage()
        //{
        //    foreach (var id in projectIds)
        //    {
        //        var project = (from p in _context.Project
        //                       where p.Id == id
        //                       select p).FirstOrDefault();
        //        List<ProjectDashboardAPI.Task> tasks = (from p in _context.Task
        //                                   where p.ProjectId == id
        //                                   select p).ToList();
        //        float CompletedTasksCharge = 0;
        //        float TasksTotalCharge = 0;
        //        int CompletionPercentage = 0;
        //        foreach (var task in tasks)
        //        {
        //            switch (task.Status)
        //            {
        //                case "Late":
        //                    TasksTotalCharge = TasksTotalCharge + task.EstEffort;
        //                    break;
        //                case "Not Started":
        //                    TasksTotalCharge = TasksTotalCharge + task.EstEffort;
        //                    break;
        //                case "In Progress":
        //                    TasksTotalCharge = TasksTotalCharge + task.EstEffort;
        //                    break;
        //                case "Completed":
        //                    TasksTotalCharge = TasksTotalCharge + task.EstEffort;
        //                    CompletedTasksCharge = CompletedTasksCharge + task.EstEffort;
        //                    break;
        //                default:
        //                    //Exception : The status is not supported...
        //                    break;
        //            }
        //        }
        //        CompletionPercentage = (int)Math.Ceiling((CompletedTasksCharge / TasksTotalCharge) * 100);
        //        project.CompletionPercentage = CompletionPercentage;
        //        _context.Project.Update(project);
        //    }
        //    _context.SaveChanges();
        //}

        protected void SendModificationEmail(Employe employee, List<WrongProject> tasksWithWrongProjectId)
        {
            var email = employee.LeclercEmail;
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("sharedtasks@leclerc.ca", "sharedtasks@leclerc.ca"));
            message.To.Add(new MailboxAddress(employee.Name, email));
            message.Subject = "Task Modification";
            string wrongProjectID = string.Join("\r\n- ", (from projectID in tasksWithWrongProjectId select projectID.projectId).ToArray());
            string wrongProjectIDSubject = string.Join("\r\n- ", (from projectSubject in tasksWithWrongProjectId select projectSubject.Subject).ToArray());

            message.Body = new TextPart("plain")
            {
                Text = @" Hey " + employee.Name + ",\r\n I just wanted to let you know that you have assigned a none existant project Id to your task(s) :\r\n \r\n" +
                "- " + wrongProjectIDSubject +
                "\r\n \r\n There is no project with this/those id(s) : \r\n \r\n" +
                "- " + wrongProjectID
                + "\r\n \r\n Please do the necessary changes in the Billing Information field(s). \r\n" +
                " \r\n-- Leclerc Sharedtasks Team"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.office365.com", 587, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("sharedtasks@leclerc.ca", "N3ljcJ2d1HJtfQn0YJjS");

                Console.WriteLine("The mail has been sent successfully...");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        //protected List<WrongProject> CreateTask(Task<FindItemsResults<Item>> userItems, Employe employee)
        //{

        //    List<WrongProject> tasksWithWrongProjectId = new List<WrongProject>();
        //    foreach (Item item in userItems.Result)
        //    {
        //        if (item is Microsoft.Exchange.WebServices.Data.Task)
        //        {
        //            Microsoft.Exchange.WebServices.Data.Task tde = (Microsoft.Exchange.WebServices.Data.Task)item;

        //            if (tde.BillingInformation == null)
        //            {
        //                continue;
        //            }

        //            else if (!(from p in _context.Project
        //                       where p.ProjectSapId == Convert.ToInt32(tde.BillingInformation)
        //                       select p.Id).Any())
        //            {
        //                WrongProject wrongProject = new WrongProject()
        //                {
        //                    projectId = tde.BillingInformation,
        //                    Subject = tde.Subject
        //                };
        //                tasksWithWrongProjectId.Add(wrongProject);
        //                continue;
        //            }

        //            ProjectDashboardAPI.Task task = new ProjectDashboardAPI.Task();

        //            task.TaskOutlookId = tde.Id.UniqueId;
        //            task.Action = tde.Subject.ToString();
        //            task.IsComplete = tde.IsComplete;
        //            task.EstEnd = tde.DueDate.GetValueOrDefault(DateTime.Now);
        //            task.AssignationDate = tde.DateTimeCreated.Date;
        //            task.ActualEffort = Convert.ToInt32(tde.ActualWork) / 60;
        //            task.EstEffort = Convert.ToInt32(tde.TotalWork) / 60;
        //            task.ProjectId = (from p in _context.Project
        //                              where p.ProjectSapId == Convert.ToInt32(tde.BillingInformation)
        //                              select p.Id).First();

        //            switch (tde.Status.ToString())
        //            {
        //                case "NotStarted":
        //                    if (DateTime.Today > tde.DueDate.GetValueOrDefault(DateTime.Now))
        //                    {
        //                        task.Status = "Late";
        //                    }
        //                    else
        //                    {
        //                        task.Status = "Not Started";
        //                    }
        //                    break;
        //                case "InProgress":
        //                    if (DateTime.Today > tde.DueDate.GetValueOrDefault(DateTime.Now))
        //                    {
        //                        task.Status = "Late";
        //                    }
        //                    else
        //                    {
        //                        task.Status = "In Progress";
        //                    }
        //                    break;
        //                case "Completed":
        //                    task.Status = "Completed";
        //                    break;
        //            }
        //            if (_context.Task.Any(t => t.TaskOutlookId == task.TaskOutlookId))
        //            {
        //                Models.Task taskOutlook = (from p in _context.Task
        //                                           where p.TaskOutlookId == task.TaskOutlookId
        //                                           select p).FirstOrDefault();
        //                if (taskOutlook.Action == task.Action &&
        //                    taskOutlook.AssignationDate == task.AssignationDate &&
        //                    taskOutlook.EstEnd == task.EstEnd &&
        //                    taskOutlook.EstEffort == task.EstEffort &&
        //                    taskOutlook.ActualEffort == task.ActualEffort &&
        //                    taskOutlook.Status == task.Status &&
        //                    taskOutlook.IsComplete == task.IsComplete)
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    taskOutlook.Action = task.Action;
        //                    taskOutlook.AssignationDate = task.AssignationDate;
        //                    taskOutlook.EstEnd = task.EstEnd;
        //                    taskOutlook.EstEffort = task.EstEffort;
        //                    taskOutlook.ActualEffort = task.ActualEffort;
        //                    taskOutlook.Status = task.Status;
        //                    taskOutlook.IsComplete = task.IsComplete;
        //                    _context.Task.Update(taskOutlook);
        //                    _context.SaveChanges();
        //                    if (!projectIds.Contains(task.ProjectId))
        //                    {
        //                        projectIds.Add(task.ProjectId);
        //                    }
        //                    continue;
        //                }
        //            }
        //            else
        //            {
        //                _context.Task.Add(task);
        //                //if (_context.ProjectContributor.Any(t => t.EmployeId != employee.Id && t.ProjectId != task.ProjectId))
        //                //{
        //                Models.TaskOwner taskOwner = new Models.TaskOwner()
        //                {
        //                    EmployeId = employee.Id,
        //                    Task = task
        //                };
        //                _context.TaskOwner.Add(taskOwner);
        //                Models.ProjectContributor projectContributor = new Models.ProjectContributor()
        //                {
        //                    EmployeId = employee.Id,
        //                    ProjectId = task.ProjectId
        //                };
        //                _context.ProjectContributor.Add(projectContributor);
        //                //}
        //                _context.SaveChanges();

        //                if (!projectIds.Contains(task.ProjectId))
        //                {
        //                    projectIds.Add(task.ProjectId);
        //                }
        //            }
        //        }
        //    }
        //    return tasksWithWrongProjectId;
        //}

    }
}
