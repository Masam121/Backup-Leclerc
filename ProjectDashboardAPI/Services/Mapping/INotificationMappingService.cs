using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Services
{
    public interface INotificationMappingService : IMapper<Notification, NotificationDto>, IMapper<NotificationSAP, Notification>
    {
    }
}
