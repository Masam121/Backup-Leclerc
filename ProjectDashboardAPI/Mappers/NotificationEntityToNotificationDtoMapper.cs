using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Models.Dto;
using ProjectDashboardAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectDashboardAPI.Mappers
{
    public class NotificationEntityToNotificationDtoMapper : IMapper<netflix_prContext, Notification, NotificationDto>
    {
        private IEmployeeRepository _emplopyeeRepository;
        private IProjectRepository _projectRepository;
        private INotificationPartnerRepository _notificationPartnerRepository;

        DateTime nullDate = new DateTime(0001, 01, 01, 0, 0, 0);

        public NotificationEntityToNotificationDtoMapper(IEmployeeRepository emplopyeeRepository, 
                                                         IProjectRepository projectRepository, 
                                                         INotificationPartnerRepository notificationPartnerRepository)
        {
            _emplopyeeRepository = emplopyeeRepository ?? throw new ArgumentNullException(nameof(emplopyeeRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _notificationPartnerRepository = notificationPartnerRepository ?? throw new ArgumentNullException(nameof(notificationPartnerRepository));
        }

        public NotificationDto Map(netflix_prContext context, Notification entity)
        {
            NotificationDto notificationDto = new NotificationDto();

            notificationDto.projectName = _projectRepository.ReadOneAsyncById(context ,entity.ProjectId).Result.ProjectName;
            notificationDto.Id = entity.NotificationSapId;
            notificationDto.description = entity.Description;
            notificationDto.creationDate = entity.CreationDate.ToString("yyyy-MM-dd");
            if (entity.EstEndDate == nullDate)
            {
                notificationDto.endDate = null;
            }
            else
            {
                notificationDto.endDate = entity.EstEndDate.ToString("yyyy-MM-dd");
            }
            notificationDto.status = entity.Status;
            notificationDto.partners = _notificationPartnerRepository.CreateNotificationPartnersDto(context, entity).Result.ToList();
            notificationDto.actualEffort = entity.ActualEffort.ToString();
            notificationDto.estimatedEffort = entity.EstEffort.ToString();


            return notificationDto;
        }
    }
}
