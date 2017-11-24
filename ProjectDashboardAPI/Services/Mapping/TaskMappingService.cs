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
        private readonly IMapper<Tuple<NotificationTask, Notification>, Task> _TaskSAPToTaskEntityMapper;

        public TaskMappingService(
            IMapper<Tuple<NotificationTask, Notification>, Task> taskSAPToTaskEntityMapper
        )
        {
            _TaskSAPToTaskEntityMapper = taskSAPToTaskEntityMapper ?? throw new ArgumentNullException(nameof(taskSAPToTaskEntityMapper));
        }

        public Task Map(Tuple<NotificationTask, Notification> entity)
        {
            return _TaskSAPToTaskEntityMapper.Map(entity);
        }
    }
}
