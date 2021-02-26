using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AdminLte.Data.Migrations.PostgreMigrations
{
    public partial class AssesmentDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assesments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assesments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CompanyFunctions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFunctions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Divitions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divitions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeGroup",
                columns: table => new
                {
                    GroupID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeGroup", x => x.GroupID);
                });

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    ParentEntityID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Entities_Entities_ParentEntityID",
                        column: x => x.ParentEntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionPackages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AssesmentID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionPackages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionPackages_Assesments_AssesmentID",
                        column: x => x.AssesmentID,
                        principalTable: "Assesments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AssesmentID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    IsRandom = table.Column<bool>(nullable: false),
                    Construct = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Sections_Assesments_AssesmentID",
                        column: x => x.AssesmentID,
                        principalTable: "Assesments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Position = table.Column<string>(nullable: false),
                    JobTitle = table.Column<string>(nullable: false),
                    DepartmentID = table.Column<int>(nullable: false),
                    GroupID = table.Column<int>(nullable: false),
                    Salary = table.Column<double>(nullable: false),
                    DateJoined = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeeGroup_GroupID",
                        column: x => x.GroupID,
                        principalTable: "EmployeeGroup",
                        principalColumn: "GroupID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BackendUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    EntityID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackendUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_BackendUsers_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BackendUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubPeriods",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    PeriodID = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPeriods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubPeriods_Periods_PeriodID",
                        column: x => x.PeriodID,
                        principalTable: "SubPeriods",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantUsers",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    EmployeeNumber = table.Column<string>(nullable: true),
                    Sex = table.Column<bool>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    PositionID = table.Column<int>(nullable: true),
                    CompanyFunctionID = table.Column<int>(nullable: true),
                    DivitionID = table.Column<int>(nullable: true),
                    DepartmentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_CompanyFunctions_CompanyFunctionID",
                        column: x => x.CompanyFunctionID,
                        principalTable: "CompanyFunctions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_Divitions_DivitionID",
                        column: x => x.DivitionID,
                        principalTable: "Divitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_Position_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Position",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionPackagePeriods",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    QuestionPackageID = table.Column<int>(nullable: true),
                    PeriodID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionPackagePeriods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionPackagePeriods_Periods_PeriodID",
                        column: x => x.PeriodID,
                        principalTable: "SubPeriods",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionPackagePeriods_QuestionPackages_QuestionPackageID",
                        column: x => x.QuestionPackageID,
                        principalTable: "QuestionPackages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HorizontalDimentions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    SituationEvpDimention = table.Column<int>(nullable: false),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorizontalDimentions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HorizontalDimentions_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    SectionID = table.Column<int>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    IsRandom = table.Column<bool>(nullable: false),
                    QuestionType = table.Column<int>(nullable: false),
                    MatrixSubType = table.Column<int>(nullable: false),
                    IsMandatory = table.Column<bool>(nullable: false),
                    IsRandomAnswer = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Questions_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerticalDimentions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    ValueDriverDimention = table.Column<int>(nullable: false),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerticalDimentions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VerticalDimentions_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: false),
                    PeriodID = table.Column<int>(nullable: true),
                    SubPeriodID = table.Column<int>(nullable: true),
                    EntityID = table.Column<int>(nullable: true),
                    AssesmentID = table.Column<int>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Schedules_Assesments_AssesmentID",
                        column: x => x.AssesmentID,
                        principalTable: "Assesments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_Periods_PeriodID",
                        column: x => x.PeriodID,
                        principalTable: "SubPeriods",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedules_SubPeriods_SubPeriodID",
                        column: x => x.SubPeriodID,
                        principalTable: "SubPeriods",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionPackageLines",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    QuestionPackageID = table.Column<int>(nullable: true),
                    QuestionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionPackageLines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionPackageLines_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionPackageLines_QuestionPackages_QuestionPackageID",
                        column: x => x.QuestionPackageID,
                        principalTable: "QuestionPackages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubVerticalDimentions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    ValueDriverDimention = table.Column<int>(nullable: false),
                    SectionID = table.Column<int>(nullable: true),
                    VerticalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubVerticalDimentions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubVerticalDimentions_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubVerticalDimentions_VerticalDimentions_VerticalDimentionID",
                        column: x => x.VerticalDimentionID,
                        principalTable: "VerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityHorizontalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    ScheduleID = table.Column<int>(nullable: true),
                    HorizontalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityHorizontalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntityHorizontalDimentionScores_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityHorizontalDimentionScores_HorizontalDimentions_Horizo~",
                        column: x => x.HorizontalDimentionID,
                        principalTable: "HorizontalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityHorizontalDimentionScores_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntitySectionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    ScheduleID = table.Column<int>(nullable: true),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySectionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntitySectionScores_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySectionScores_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySectionScores_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityVerticalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    ScheduleID = table.Column<int>(nullable: true),
                    VerticalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityVerticalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntityVerticalDimentionScores_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityVerticalDimentionScores_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntityVerticalDimentionScores_VerticalDimentions_VerticalDi~",
                        column: x => x.VerticalDimentionID,
                        principalTable: "VerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ScheduleID = table.Column<int>(nullable: true),
                    ParticipantUserUserId = table.Column<string>(nullable: true),
                    QuestionPackageID = table.Column<int>(nullable: true),
                    FinishedAt = table.Column<DateTime>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    PositionID = table.Column<int>(nullable: true),
                    CompanyFunctionID = table.Column<int>(nullable: true),
                    DivitionID = table.Column<int>(nullable: true),
                    DepartmentID = table.Column<int>(nullable: true),
                    IsCanRetake = table.Column<bool>(nullable: false),
                    MaxRetake = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Participants_CompanyFunctions_CompanyFunctionID",
                        column: x => x.CompanyFunctionID,
                        principalTable: "CompanyFunctions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Divitions_DivitionID",
                        column: x => x.DivitionID,
                        principalTable: "Divitions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_ParticipantUsers_ParticipantUserUserId",
                        column: x => x.ParticipantUserUserId,
                        principalTable: "ParticipantUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Position_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Position",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_QuestionPackages_QuestionPackageID",
                        column: x => x.QuestionPackageID,
                        principalTable: "QuestionPackages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Participants_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntitySubVerticalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    ZScore = table.Column<float>(nullable: false),
                    TScore = table.Column<float>(nullable: false),
                    EntityID = table.Column<int>(nullable: true),
                    ScheduleID = table.Column<int>(nullable: true),
                    SubVerticalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySubVerticalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EntitySubVerticalDimentionScores_Entities_EntityID",
                        column: x => x.EntityID,
                        principalTable: "Entities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubVerticalDimentionScores_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EntitySubVerticalDimentionScores_SubVerticalDimentions_SubV~",
                        column: x => x.SubVerticalDimentionID,
                        principalTable: "SubVerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswer",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    QuestionID = table.Column<int>(nullable: true),
                    MatrixQuestionID = table.Column<int>(nullable: true),
                    MatrixValue = table.Column<int>(nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    Weight = table.Column<float>(nullable: false),
                    AnswerScore = table.Column<float>(nullable: false),
                    IsCorrect = table.Column<bool>(nullable: false),
                    VerticalDimentionID = table.Column<int>(nullable: true),
                    SubVerticalDimentionID = table.Column<int>(nullable: true),
                    HorizontalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswer", x => x.ID);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_HorizontalDimentions_HorizontalDimentionID",
                        column: x => x.HorizontalDimentionID,
                        principalTable: "HorizontalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_Questions_MatrixQuestionID",
                        column: x => x.MatrixQuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_SubVerticalDimentions_SubVerticalDimentionID",
                        column: x => x.SubVerticalDimentionID,
                        principalTable: "SubVerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuestionAnswer_VerticalDimentions_VerticalDimentionID",
                        column: x => x.VerticalDimentionID,
                        principalTable: "VerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantAnswerSheets",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ParticipantID = table.Column<int>(nullable: true),
                    ScheduleID = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantAnswerSheets", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantAnswerSheets_Participants_ParticipantID",
                        column: x => x.ParticipantID,
                        principalTable: "Participants",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantAnswerSheets_Schedules_ScheduleID",
                        column: x => x.ScheduleID,
                        principalTable: "Schedules",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantAnswerSheetLines",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ParticipantAnswerSheetID = table.Column<int>(nullable: true),
                    SuggestedAnswerID = table.Column<int>(nullable: false),
                    MatrixRowAnserID = table.Column<int>(nullable: false),
                    QuestionSquence = table.Column<int>(nullable: false),
                    IsSkipped = table.Column<bool>(nullable: false),
                    AnswerType = table.Column<int>(nullable: false),
                    CharBoxValue = table.Column<string>(nullable: true),
                    FreeTextValue = table.Column<string>(nullable: true),
                    NumericalBoxValue = table.Column<float>(nullable: false),
                    AnswerWeight = table.Column<float>(nullable: false),
                    AnswerScore = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantAnswerSheetLines", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantAnswerSheetLines_ParticipantAnswerSheets_Partici~",
                        column: x => x.ParticipantAnswerSheetID,
                        principalTable: "ParticipantAnswerSheets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantHorizontalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    ParticipantAnswerSheetLineID = table.Column<int>(nullable: true),
                    HorizontalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantHorizontalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantHorizontalDimentionScores_HorizontalDimentions_H~",
                        column: x => x.HorizontalDimentionID,
                        principalTable: "HorizontalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantHorizontalDimentionScores_ParticipantAnswerSheet~",
                        column: x => x.ParticipantAnswerSheetLineID,
                        principalTable: "ParticipantAnswerSheetLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantSectionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    ParticipantAnswerSheetLineID = table.Column<int>(nullable: true),
                    SectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantSectionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantSectionScores_ParticipantAnswerSheetLines_Partic~",
                        column: x => x.ParticipantAnswerSheetLineID,
                        principalTable: "ParticipantAnswerSheetLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantSectionScores_Sections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "Sections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantSubVerticalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    ZScore = table.Column<float>(nullable: false),
                    TScore = table.Column<float>(nullable: false),
                    ParticipantAnswerSheetLineID = table.Column<int>(nullable: true),
                    SubVerticalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantSubVerticalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantSubVerticalDimentionScores_ParticipantAnswerShee~",
                        column: x => x.ParticipantAnswerSheetLineID,
                        principalTable: "ParticipantAnswerSheetLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantSubVerticalDimentionScores_SubVerticalDimentions~",
                        column: x => x.SubVerticalDimentionID,
                        principalTable: "SubVerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantVerticalDimentionScores",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Score = table.Column<float>(nullable: false),
                    ParticipantAnswerSheetLineID = table.Column<int>(nullable: true),
                    VerticalDimentionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantVerticalDimentionScores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ParticipantVerticalDimentionScores_ParticipantAnswerSheetLi~",
                        column: x => x.ParticipantAnswerSheetLineID,
                        principalTable: "ParticipantAnswerSheetLines",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantVerticalDimentionScores_VerticalDimentions_Verti~",
                        column: x => x.VerticalDimentionID,
                        principalTable: "VerticalDimentions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackendUsers_EntityID",
                table: "BackendUsers",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentID",
                table: "Employees",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_GroupID",
                table: "Employees",
                column: "GroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_ParentEntityID",
                table: "Entities",
                column: "ParentEntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHorizontalDimentionScores_EntityID",
                table: "EntityHorizontalDimentionScores",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHorizontalDimentionScores_HorizontalDimentionID",
                table: "EntityHorizontalDimentionScores",
                column: "HorizontalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityHorizontalDimentionScores_ScheduleID",
                table: "EntityHorizontalDimentionScores",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySectionScores_EntityID",
                table: "EntitySectionScores",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySectionScores_ScheduleID",
                table: "EntitySectionScores",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySectionScores_SectionID",
                table: "EntitySectionScores",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubVerticalDimentionScores_EntityID",
                table: "EntitySubVerticalDimentionScores",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubVerticalDimentionScores_ScheduleID",
                table: "EntitySubVerticalDimentionScores",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySubVerticalDimentionScores_SubVerticalDimentionID",
                table: "EntitySubVerticalDimentionScores",
                column: "SubVerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityVerticalDimentionScores_EntityID",
                table: "EntityVerticalDimentionScores",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityVerticalDimentionScores_ScheduleID",
                table: "EntityVerticalDimentionScores",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_EntityVerticalDimentionScores_VerticalDimentionID",
                table: "EntityVerticalDimentionScores",
                column: "VerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_HorizontalDimentions_SectionID",
                table: "HorizontalDimentions",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAnswerSheetLines_ParticipantAnswerSheetID",
                table: "ParticipantAnswerSheetLines",
                column: "ParticipantAnswerSheetID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAnswerSheets_ParticipantID",
                table: "ParticipantAnswerSheets",
                column: "ParticipantID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantAnswerSheets_ScheduleID",
                table: "ParticipantAnswerSheets",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantHorizontalDimentionScores_HorizontalDimentionID",
                table: "ParticipantHorizontalDimentionScores",
                column: "HorizontalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantHorizontalDimentionScores_ParticipantAnswerSheet~",
                table: "ParticipantHorizontalDimentionScores",
                column: "ParticipantAnswerSheetLineID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_CompanyFunctionID",
                table: "Participants",
                column: "CompanyFunctionID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_DepartmentID",
                table: "Participants",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_DivitionID",
                table: "Participants",
                column: "DivitionID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_EntityID",
                table: "Participants",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ParticipantUserUserId",
                table: "Participants",
                column: "ParticipantUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_PositionID",
                table: "Participants",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_QuestionPackageID",
                table: "Participants",
                column: "QuestionPackageID");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_ScheduleID",
                table: "Participants",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantSectionScores_ParticipantAnswerSheetLineID",
                table: "ParticipantSectionScores",
                column: "ParticipantAnswerSheetLineID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantSectionScores_SectionID",
                table: "ParticipantSectionScores",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantSubVerticalDimentionScores_ParticipantAnswerShee~",
                table: "ParticipantSubVerticalDimentionScores",
                column: "ParticipantAnswerSheetLineID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantSubVerticalDimentionScores_SubVerticalDimentionID",
                table: "ParticipantSubVerticalDimentionScores",
                column: "SubVerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantUsers_CompanyFunctionID",
                table: "ParticipantUsers",
                column: "CompanyFunctionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantUsers_DepartmentID",
                table: "ParticipantUsers",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantUsers_DivitionID",
                table: "ParticipantUsers",
                column: "DivitionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantUsers_EntityID",
                table: "ParticipantUsers",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantUsers_PositionID",
                table: "ParticipantUsers",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantVerticalDimentionScores_ParticipantAnswerSheetLi~",
                table: "ParticipantVerticalDimentionScores",
                column: "ParticipantAnswerSheetLineID");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantVerticalDimentionScores_VerticalDimentionID",
                table: "ParticipantVerticalDimentionScores",
                column: "VerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_HorizontalDimentionID",
                table: "QuestionAnswer",
                column: "HorizontalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_MatrixQuestionID",
                table: "QuestionAnswer",
                column: "MatrixQuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_QuestionID",
                table: "QuestionAnswer",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_SubVerticalDimentionID",
                table: "QuestionAnswer",
                column: "SubVerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswer_VerticalDimentionID",
                table: "QuestionAnswer",
                column: "VerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackageLines_QuestionID",
                table: "QuestionPackageLines",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackageLines_QuestionPackageID",
                table: "QuestionPackageLines",
                column: "QuestionPackageID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackagePeriods_PeriodID",
                table: "QuestionPackagePeriods",
                column: "PeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackagePeriods_QuestionPackageID",
                table: "QuestionPackagePeriods",
                column: "QuestionPackageID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionPackages_AssesmentID",
                table: "QuestionPackages",
                column: "AssesmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SectionID",
                table: "Questions",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_AssesmentID",
                table: "Schedules",
                column: "AssesmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EntityID",
                table: "Schedules",
                column: "EntityID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_PeriodID",
                table: "Schedules",
                column: "PeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SubPeriodID",
                table: "Schedules",
                column: "SubPeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AssesmentID",
                table: "Sections",
                column: "AssesmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubPeriods_PeriodID",
                table: "SubPeriods",
                column: "PeriodID");

            migrationBuilder.CreateIndex(
                name: "IX_SubVerticalDimentions_SectionID",
                table: "SubVerticalDimentions",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubVerticalDimentions_VerticalDimentionID",
                table: "SubVerticalDimentions",
                column: "VerticalDimentionID");

            migrationBuilder.CreateIndex(
                name: "IX_VerticalDimentions_SectionID",
                table: "VerticalDimentions",
                column: "SectionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BackendUsers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "EntityHorizontalDimentionScores");

            migrationBuilder.DropTable(
                name: "EntitySectionScores");

            migrationBuilder.DropTable(
                name: "EntitySubVerticalDimentionScores");

            migrationBuilder.DropTable(
                name: "EntityVerticalDimentionScores");

            migrationBuilder.DropTable(
                name: "ParticipantHorizontalDimentionScores");

            migrationBuilder.DropTable(
                name: "ParticipantSectionScores");

            migrationBuilder.DropTable(
                name: "ParticipantSubVerticalDimentionScores");

            migrationBuilder.DropTable(
                name: "ParticipantVerticalDimentionScores");

            migrationBuilder.DropTable(
                name: "QuestionAnswer");

            migrationBuilder.DropTable(
                name: "QuestionPackageLines");

            migrationBuilder.DropTable(
                name: "QuestionPackagePeriods");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "EmployeeGroup");

            migrationBuilder.DropTable(
                name: "ParticipantAnswerSheetLines");

            migrationBuilder.DropTable(
                name: "HorizontalDimentions");

            migrationBuilder.DropTable(
                name: "SubVerticalDimentions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "ParticipantAnswerSheets");

            migrationBuilder.DropTable(
                name: "VerticalDimentions");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "ParticipantUsers");

            migrationBuilder.DropTable(
                name: "QuestionPackages");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "CompanyFunctions");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Divitions");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Assesments");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropTable(
                name: "SubPeriods");

            migrationBuilder.DropTable(
                name: "SubPeriods");
        }
    }
}
