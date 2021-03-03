using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminLte.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminLte.Data
{
    public class PostgreDbContext : IdentityDbContext
    {
        public PostgreDbContext(DbContextOptions<PostgreDbContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseNpgsql(options);
    
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }

        // ORGANIZATION
        public DbSet<CompanyFunction> CompanyFunctions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Divition> Divitions { get; set; }
        public DbSet<Entity> Entities { get; set; }

        // QUESTION
        public DbSet<Assesment> Assesments { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<SubPeriod> SubPeriods { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<QuestionPackage> QuestionPackages { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswer { get; set; }
        public DbSet<QuestionPackagePeriod> QuestionPackagePeriods { get; set; }
        public DbSet<QuestionPackageLine> QuestionPackageLines { get; set; }
        public DbSet<HorizontalDimention> HorizontalDimentions { get; set; }
        public DbSet<VerticalDimention> VerticalDimentions { get; set; }
        public DbSet<SubVerticalDimention> SubVerticalDimentions { get; set; }

        // USER
        public DbSet<BackendUser> BackendUsers { get; set; }
        public DbSet<ParticipantUser> ParticipantUsers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ParticipantAnswerSheet> ParticipantAnswerSheets { get; set; }
        public DbSet<ParticipantAnswerSheetSection> ParticipantAnswerSheetSections { get; set; }
        public DbSet<ParticipantAnswerSheetLine> ParticipantAnswerSheetLines { get; set; }
        public DbSet<ParticipantSectionScore> ParticipantSectionScores { get; set; }
        public DbSet<ParticipantHorizontalDimentionScore> ParticipantHorizontalDimentionScores { get; set; }
        public DbSet<ParticipantVerticalDimentionScore> ParticipantVerticalDimentionScores { get; set; }
        public DbSet<ParticipantSubVerticalDimentionScore> ParticipantSubVerticalDimentionScores { get; set; }
        public DbSet<EntitySectionScore> EntitySectionScores { get; set; }
        public DbSet<EntityHorizontalDimentionScore> EntityHorizontalDimentionScores { get; set; }
        public DbSet<EntityVerticalDimentionScore> EntityVerticalDimentionScores { get; set; }
        public DbSet<EntitySubVerticalDimentionScore> EntitySubVerticalDimentionScores { get; set; }

    }
}
