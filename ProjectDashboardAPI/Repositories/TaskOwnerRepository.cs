using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Repositories
{
    public class TaskOwnerRepository : ITaskOwnerRepository
    {
        protected string TrimZerosFromSAPId(string id)
        {
            string trimedId = id.TrimStart('0');
            return trimedId;
        }

        public void AddTaskOwner(netflix_prContext context, TaskOwner taskOwner)
        {
            context.TaskOwner.Add(taskOwner);
        }

        public Task<TaskOwner> CreateTaskOwner(netflix_prContext context, string employeeId, Task task)
        {
            if (employeeId == "" || employeeId == null)
            {
                throw new System.ArgumentException("A TaskOwner needs to be affilated to a valid employeeId", "employeeSAPId :" + employeeId);
            }
            else
            {
                int id = (from p in context.Employe
                                  where p.IdSAP == TrimZerosFromSAPId(employeeId)
                                  select p.Id).FirstOrDefault();

                if (id == 0)
                {
                    throw new System.ArgumentException("The id needs to belongs to an existing employee in the database", "employeeSAPId :" + employeeId);
                }
                TaskOwner taskOwner = new TaskOwner();
                taskOwner.Task = task;
                taskOwner.EmployeId = id;

                return System.Threading.Tasks.Task.FromResult(taskOwner);
            }
        }

        public Task<TaskOwner> ReadOneAsyncTaskOwnerByTaskId(netflix_prContext context, int taskId)
        {
            TaskOwner taskOwner = (from p in context.TaskOwner
                                        where p.TaskId == taskId
                                        select p).FirstOrDefault();

            return System.Threading.Tasks.Task.FromResult(taskOwner);
        }

        public void DeleteTaskOwner(netflix_prContext context, TaskOwner taskOwner)
        {
            context.TaskOwner.Remove(taskOwner);
        }
    }
}
