using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NetflixAPI.Models;
using Microsoft.Extensions.Logging;
using ProjectDashboardAPI;
using ProjectDashboardAPI.Services;
using ProjectDashboardAPI.Repositories;
using ProjectDashboardAPI.Services.Mapping;
using ProjectDashboardAPI.Mappers;
using ProjectDashboardAPI.Controllers;
using ProjectDashboardAPI.Models.Dto;

namespace NetflixAPI
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<netflix_prContext>(options =>
            {
                String conString = Configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(conString);
            });
            services.AddMvc().AddJsonOptions(jsonOptions => { jsonOptions.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });

            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ISapService, SapService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();

            services.AddSingleton<IMapper<netflix_prContext, Notification, NotificationDto>, NotificationEntityToNotificationDtoMapper>();
            services.AddSingleton<IMapper<netflix_prContext, Tuple<Employe, Role>, PartnerDto>, NotificationPartnerToPartnerDtoMapper>();
            services.AddSingleton<IMapper<netflix_prContext, Project, ProjectNetflixCard>, ProjectEntityToProjectNetflixCardMapper>();
            services.AddSingleton<IMapper<netflix_prContext, Project, ProjectNetflix>, ProjectEntityToProjectNetflixMapper>();
            services.AddSingleton<IMapper<netflix_prContext, ProjectSAP, Project>, ProjectSAPToProjectEntityMapper>();
            services.AddSingleton<IMapper<netflix_prContext, Tuple<NotificationTask, Notification>, Task>, TaskSAPToTaskEntityMapper>();
            services.AddSingleton<IMapper<netflix_prContext, Tuple<Partner, Notification>, NotificationPartner>, NotificationPartnerSAPToNotificationPartnerEntityMapper>();
            services.AddSingleton<IMapper<netflix_prContext, NotificationSAP, Notification>, NotificationSAPToNotificationEntityMapper>();

            services.AddSingleton<IProjectMappingService, ProjectMappingService>();
            services.AddSingleton<INotificationMappingService, NotificationMappingService>();
            services.AddSingleton<INotificationPartnerMappingService, NotificationPartnerMappingService>();
            services.AddSingleton<IProjectCardMappingService, ProjectCardMappingService>();
            services.AddSingleton<ITaskMappingService, TaskMappingService>();

            services.AddSingleton<IProjectRepository, ProjectRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddSingleton<INotificationPartnerRepository, NotificationPartnerRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<IBudgetRepository, BudgetRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<ITaskOwnerRepository, TaskOwnerRepository>();
            services.AddSingleton<ITaskRepository, TaskRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "My API",
                    Version = "v1",
                    Description = "ASP.NET Core Web API for Netflix App",
                    TermsOfService = "None",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { Name = "Marcel Samson Morasse", Email = "", Url = "" },
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder => {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
            app.ApplicationServices.GetService<IDisposable>();
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
