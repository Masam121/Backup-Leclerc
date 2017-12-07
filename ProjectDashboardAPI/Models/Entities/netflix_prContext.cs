using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectDashboardAPI
{
    public partial class netflix_prContext : DbContext
    {
        public virtual DbSet<Budget> Budget { get; set; }
        public virtual DbSet<ConnexeProject> ConnexeProject { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Employe> Employe { get; set; }
        public virtual DbSet<Expense> Expense { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<NotificationPartner> NotificationPartner { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectContributor> ProjectContributor { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskOwner> TaskOwner { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Watchlist> Watchlist { get; set; }
        public virtual DbSet<WatchlistContent> WatchlistContent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"server=ca010webtst02.leclerc.local;database=netflix_pr;uid=netflix_pr;pwd=crucUS2WruZu;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.BudgetLeft).HasColumnName("budget_left");

                entity.Property(e => e.BudgetSapId)
                    .HasColumnName("budget_SAP_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.BudgetSpent).HasColumnName("budget_spent");

                entity.Property(e => e.InitialBudget).HasColumnName("initial_budget");
            });

            modelBuilder.Entity<ConnexeProject>(entity =>
            {
                entity.ToTable("Connexe_Project");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FKConnexe_Pr649210");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ConnexeProjectSapid)
                    .HasColumnName("connexe_project_SAPid")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("project_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ConnexeProject)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKConnexe_Pr649210");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.ProjectId)
                    .HasName("FKDocument741063");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.DocumentDescription)
                    .HasColumnName("document_description")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.DocumentLink)
                    .HasColumnName("document_link")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("project_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKDocument741063");
            });

            modelBuilder.Entity<Employe>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Factory)
                    .HasColumnName("factory")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.HiredDate).HasColumnName("hired_date");

                entity.Property(e => e.IdSAP)
                    .HasColumnName("id_SAP")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LeclercEmail)
                    .HasColumnName("leclerc_email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.O365Id)
                    .HasColumnName("O365_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Picture)
                    .HasColumnName("picture")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectWorkRatio).HasColumnName("projectWorkRatio");

                entity.Property(e => e.SuperiorId)
                    .HasColumnName("superior_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Workload)
                    .HasColumnName("workload")
                    .HasColumnType("int(10)");
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasIndex(e => e.Budgetid)
                    .HasName("FKExpense231618");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Budgetid).HasColumnType("int(10)");

                entity.Property(e => e.ExpenseName)
                    .IsRequired()
                    .HasColumnName("expense_name")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.Expense)
                    .HasForeignKey(d => d.Budgetid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FKExpense231618");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.ProjectId)
                    .HasName("FKNotificati929200");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ActualEffort)
                    .HasColumnName("actual_effort")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CompletedDate).HasColumnName("Completed_date");

                entity.Property(e => e.CreationDate).HasColumnName("creation_date");

                entity.Property(e => e.Department)
                    .IsRequired()
                    .HasColumnName("department")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.EstEffort)
                    .HasColumnName("est_effort")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EstEndDate).HasColumnName("est_end_date");

                entity.Property(e => e.IsCompleted)
                    .HasColumnName("isCompleted")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.NotificationSapId)
                    .HasColumnName("notificationSAP_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasColumnName("priority")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("project_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKNotificati929200");
            });

            modelBuilder.Entity<NotificationPartner>(entity =>
            {
                entity.ToTable("Notification_Partner");

                entity.HasIndex(e => e.NotificationId)
                    .HasName("FKNotificati724990");

                entity.HasIndex(e => e.EmployeId)
                   .HasName("FKNotificati724992");

                entity.HasIndex(e => e.RoleId)
                   .HasName("FKNotificati724991");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ConcatenatedId).
                    HasColumnName("concatenated_id").
                    HasColumnType("varchar(255)");

                entity.Property(e => e.EmployeId)
                    .HasColumnName("employe_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.NotificationId)
                    .HasColumnName("notification_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EstEffort)
                    .HasColumnName("est_effort")
                    .HasColumnType("double");

                entity.Property(e => e.actualEffort)
                    .HasColumnName("actual_effort")
                    .HasColumnType("double");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.NotificationPartner)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKNotificati724990");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.NotificationPartner)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKNotificati724991");

                entity.HasOne(d => d.Employe)
                    .WithMany(p => p.NotificationPartner)
                    .HasForeignKey(d => d.EmployeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKNotificati724992");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(e => e.BudgetId)
                    .HasName("FKProject189077");

                entity.HasIndex(e => e.ProjectManagerId)
                    .HasName("FKProject297312");

                entity.HasIndex(e => e.ProjectOwnerId)
                    .HasName("FKProject342278");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.BudgetId)
                    .HasColumnName("budget_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.CompletionPercentage)
                    .HasColumnName("completion_percentage")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.EstEndDate).HasColumnName("est_end_date");

                entity.Property(e => e.EstWorkDay).HasColumnName("est_work_day");

                entity.Property(e => e.Factory)
                    .HasColumnName("factory")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Priority)
                    .HasColumnName("priority")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectManagerId)
                    .HasColumnName("project_manager_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("project_name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectOwnerId)
                    .HasColumnName("project_owner_Id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ProjectSapId)
                    .HasColumnName("project_SAP_Id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectStatus)
                    .HasColumnName("project_status")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ProjectsClient)
                    .IsRequired()
                    .HasColumnName("projects_client")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.Thumbnail)
                    .HasColumnName("thumbnail")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Budget)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKProject189077");

                entity.HasOne(d => d.ProjectManager)
                    .WithMany(p => p.ProjectProjectManager)
                    .HasForeignKey(d => d.ProjectManagerId)
                    .HasConstraintName("FKProject297312");

                entity.HasOne(d => d.ProjectOwner)
                    .WithMany(p => p.ProjectProjectOwner)
                    .HasForeignKey(d => d.ProjectOwnerId)
                    .HasConstraintName("FKProject342278");
            });

            modelBuilder.Entity<ProjectContributor>(entity =>
            {
                entity.ToTable("Project_Contributor");

                entity.HasIndex(e => e.EmployeId)
                    .HasName("FKProject_Co530508");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FKProject_Co771843");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FKProject_Co294822");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EmployeId)
                    .HasColumnName("employe_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EndDateContribution).HasColumnName("end_date_contribution");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("project_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.RoleId)
                    .HasColumnName("role_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.StartDateContribution).HasColumnName("start_date_contribution");

                entity.HasOne(d => d.Employe)
                    .WithMany(p => p.ProjectContributor)
                    .HasForeignKey(d => d.EmployeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKProject_Co530508");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectContributor)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKProject_Co771843");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.ProjectContributor)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FKProject_Co294822");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("role_name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.RoleSigle)
                    .IsRequired()
                    .HasColumnName("role_sigle")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasIndex(e => e.NotificationId)
                    .HasName("FKTask322753");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ConcatenatedId)
                   .IsRequired()
                   .HasColumnName("concatenatedId")
                   .HasColumnType("varchar(255)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ActualEffort)
                    .HasColumnName("actual_effort")
                    .HasColumnType("int(10)");

                entity.Property(e => e.TaskSAPId)
                    .IsRequired()
                    .HasColumnName("task_SAP_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.AssignationDate).HasColumnName("assignation_date");

                entity.Property(e => e.EstEffort)
                    .HasColumnName("est_effort")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EstEnd).HasColumnName("est_end");

                entity.Property(e => e.IsComplete).HasColumnType("tinyint(1)");

                entity.Property(e => e.NotificationId)
                    .HasColumnName("notification_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKTask322753");
            });

            modelBuilder.Entity<TaskOwner>(entity =>
            {
                entity.ToTable("Task_Owner");

                entity.HasIndex(e => e.EmployeId)
                    .HasName("FKTask_Owner320287");

                entity.HasIndex(e => e.TaskId)
                    .HasName("FKTask_Owner395416");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EmployeId)
                    .HasColumnName("employe_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.TaskId)
                    .HasColumnName("task_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Employe)
                    .WithMany(p => p.TaskOwner)
                    .HasForeignKey(d => d.EmployeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKTask_Owner320287");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskOwner)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKTask_Owner395416");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.EmployeId)
                    .HasName("FKUser262483");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EmployeId)
                    .HasColumnName("employe_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Employe)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.EmployeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKUser262483");
            });

            modelBuilder.Entity<Watchlist>(entity =>
            {
                entity.HasIndex(e => e.ContentId)
                    .HasName("FKWatchlist615832");

                entity.HasIndex(e => e.UserId)
                    .HasName("FKWatchlist546381");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ContentId)
                    .HasColumnName("content_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Content)
                    .WithMany(p => p.Watchlist)
                    .HasForeignKey(d => d.ContentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKWatchlist615832");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Watchlist)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKWatchlist546381");
            });

            modelBuilder.Entity<WatchlistContent>(entity =>
            {
                entity.ToTable("Watchlist_Content");

                entity.HasIndex(e => e.EmployeId)
                    .HasName("FKWatchlist_386438");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("FKWatchlist_145103");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.EmployeId)
                    .HasColumnName("employe_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.ProjectId)
                    .HasColumnName("project_id")
                    .HasColumnType("int(10)");

                entity.Property(e => e.WatchlistId)
                    .HasColumnName("watchlist_id")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.Employe)
                    .WithMany(p => p.WatchlistContent)
                    .HasForeignKey(d => d.EmployeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKWatchlist_386438");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.WatchlistContent)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FKWatchlist_145103");
            });
        }
    }
}