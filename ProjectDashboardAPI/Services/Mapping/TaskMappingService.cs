using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Mappers;

namespace ProjectDashboardAPI.Services.Mapping
{
    public class TaskMappingService : ITaskMappingService
    {
        private readonly IMapper<netflix_prContext, Tuple<NotificationTask, Notification>, Task> _TaskSAPToTaskEntityMapper;

        public TaskMappingService(
            IMapper<netflix_prContext, Tuple<NotificationTask, Notification>, Task> taskSAPToTaskEntityMapper
        )
        {
            _TaskSAPToTaskEntityMapper = taskSAPToTaskEntityMapper ?? throw new ArgumentNullException(nameof(taskSAPToTaskEntityMapper));
        }

        public Task Map(netflix_prContext context, Tuple<NotificationTask, Notification> entity)
        {
            return _TaskSAPToTaskEntityMapper.Map(context, entity);
        }
    }
}
