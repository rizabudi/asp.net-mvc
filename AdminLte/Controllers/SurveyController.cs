using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    public class SurveyController : Controller
    {
        private readonly PostgreDbContext _db;
        public SurveyController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("survey/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.QuestionPackages
                    .Include(x=>x.Assesment)
                    .Include(x=>x.QuestionPackageLines)
                    .Include(x => x.QuestionPackageEntities)
                    .OrderBy(x => x.Assesment.Name)
                    .ThenBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var questions = row.QuestionPackageLines.Count();
                    var entities = row.QuestionPackageEntities.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Assesment.Name,
                            row.Name,
                            "HTML:<a href='/survey-question/" + row.ID + "'><i class='fa fa-question'></i> " + questions + " Soal</a>",
                            "HTML:<a href='/survey-entity/" + row.ID + "'><i class='fa fa-building'></i> " + entities + " Soal</a>",
                            "HTML:<a href='/survey/download/" + row.ID + "/false'><i class='fa fa-file'></i> Hasil Survei</a><br/><a href='/survey/dashboard/" + row.ID + "'><i class='fa fa-chart-area'></i> Dashboard</a>"
                        }
                    });
                }

                ViewData["Rows"] = rows;
                ViewData["Page"] = page;

                return PartialView("~/Views/Shared/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.QuestionPackages.Count();
                ViewData["Total"] = total;
                ViewData["Page"] = page;

                return PartialView("~/Views/Shared/_PagingView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                QuestionPackage questionPackageFromDb = null;
                if(id != 0)
                {
                    questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == id);
                }
                var assesments = await _db.Assesments.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionPackageFromDb == null ? "0" : questionPackageFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Jenis Survei", Name = "Assesment", InputType = InputType.DROPDOWN, Options = assesments, Value = questionPackageFromDb == null ? "" : questionPackageFromDb.Assesment.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = questionPackageFromDb == null ? "" : questionPackageFromDb.Name, IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey")]
        [CustomAuthFilter("Access_MasterData_DaftarSurvei")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Daftar Survei";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Jenis Survei", Name = "Assesment", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Daftar Soal", Name = "Questions" });
            ColumnModels.Add(new ColumnModel { Label = "Pengaturan Entitas", Name = "Entities" });
            ColumnModels.Add(new ColumnModel { Label = "Hasil Survei", Name = "Result" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "survey.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("survey/save")]
        public async Task<IActionResult> Save(QuestionPackage questionPackage)
        {
            try
            {
                QuestionPackage questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackage.ID);

                if (questionPackageFromDb == null)
                {
                    questionPackage.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == questionPackage.Assesment.ID);

                    _db.QuestionPackages.Add(questionPackage);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionPackageFromDb.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == questionPackage.Assesment.ID);
                    questionPackageFromDb.Name = questionPackage.Name;
                    _db.QuestionPackages.Update(questionPackageFromDb);
                    _db.SaveChanges();
                    return Json(new { success = true, message = "Data berhasil diperbarui" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        [HttpPost("survey/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionPackageFromDb = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == id);

                if (questionPackageFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionPackages.Remove(questionPackageFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        [HttpGet("survey/result")]
        [Route("survey/result/{surveyID:int}/{tab:int?}")]
        [CustomAuthFilter("Access_MasterData_DaftarSurvei")]
        public async Task<IActionResult> ResultAsync(int surveyID, int tab = 0)
        {
            var questionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(x => x.ID == surveyID);

            if(questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            var participants = await _db.Participants
                .Include(x=>x.ParticipantUser)
                .ThenInclude(x=>x.User)
                .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID)
                .OrderBy(x=>x.ParticipantUser.Name)
                .Skip(0)
                .Take(10)
                .ToListAsync();

            if (tab == 1)
            {
                var questions = await _db.QuestionPackageLines
                    .Include(x => x.Question)
                    .ThenInclude(x => x.QuestionAnswerMatrixs)
                    .Include(x => x.Question.Section)
                    .Where(x => x.QuestionPackage.ID == surveyID && x.Question.Section.Construct == Construct.CULTURE)
                    .Select(x => x.Question)
                    .ToListAsync();

                var verticalDimentions = await _db.VerticalDimentions
                    .Include(x => x.Section)
                    .Include(x => x.SubVerticalDimentions)
                    .Where(x => x.Section.Construct == Construct.CULTURE)
                    .ToListAsync();

                var vwCulturePerRows = await _db.VwCulturePerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();

                var answerCultures = new Dictionary<int, List<VwCulturePerRow>>();
                foreach (VwCulturePerRow row in vwCulturePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerCultures.ContainsKey(id))
                    {
                        answerCultures.Add(id, new List<VwCulturePerRow>());
                    }
                    answerCultures[id].Add(row);
                }

                ViewData["VerticalDimentions"] = verticalDimentions;
                ViewData["Questions"] = questions;
                ViewData["AnswerCultures"] = answerCultures;
            }
            else if (tab == 2)
            {
                var questions = await _db.QuestionPackageLines
                    .Include(x => x.Question)
                    .ThenInclude(x => x.QuestionAnswerMatrixs)
                    .Include(x => x.Question.Section)
                    .Where(x => x.QuestionPackage.ID == surveyID && x.Question.Section.Construct == Construct.ENGAGEMENT)
                    .Select(x => x.Question)
                    .ToListAsync();

                var horizontalDimentions = await _db.HorizontalDimentions
                    .Include(x => x.Section)
                    .Where(x => x.Section.Construct == Construct.ENGAGEMENT)
                    .ToListAsync();

                var verticalDimentions = await _db.VerticalDimentions
                    .Include(x => x.Section)
                    .Include(x => x.SubVerticalDimentions)
                    .Where(x => x.Section.Construct == Construct.ENGAGEMENT)
                    .ToListAsync();

                var vwEngagementPerRows = await _db.VwEngagementPerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();

                var answerEngagements = new Dictionary<int, List<VwEngagementPerRow>>();
                foreach (VwEngagementPerRow row in vwEngagementPerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerEngagements.ContainsKey(id))
                    {
                        answerEngagements.Add(id, new List<VwEngagementPerRow>());
                    }
                    answerEngagements[id].Add(row);
                }

                ViewData["Questions"] = questions;
                ViewData["VerticalDimentions"] = verticalDimentions;
                ViewData["HorizontalDimentions"] = horizontalDimentions;
                ViewData["AnswerEngagements"] = answerEngagements;
            }
            else if (tab == 3)
            {
                var questions = await _db.QuestionPackageLines
                    .Include(x => x.Question)
                    .ThenInclude(x => x.QuestionAnswerMatrixs)
                    .Include(x => x.Question.Section)
                    .Where(x => x.QuestionPackage.ID == surveyID && x.Question.Section.Construct == Construct.PERFORMANCE)
                    .Select(x => x.Question)
                    .ToListAsync();

                var verticalDimentions = await _db.VerticalDimentions
                    .Include(x => x.Section)
                    .Include(x => x.SubVerticalDimentions)
                    .Where(x => x.Section.Construct == Construct.PERFORMANCE)
                    .ToListAsync();

                var vwPerformancePerRows = await _db.VwPerformancePerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();

                var answerPerformances = new Dictionary<int, List<VwPerformancePerRow>>();
                foreach (VwPerformancePerRow row in vwPerformancePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerPerformances.ContainsKey(id))
                    {
                        answerPerformances.Add(id, new List<VwPerformancePerRow>());
                    }
                    answerPerformances[id].Add(row);
                }

                ViewData["Questions"] = questions;
                ViewData["VerticalDimentions"] = verticalDimentions;
                ViewData["AnswerPerformances"] = answerPerformances;
            }

            ViewData["Survey"] = questionPackage;
            ViewData["Participants"] = participants;
            ViewData["Tab"] = tab;

            ViewData["SideBarCollapse"] = true;

            return View();
        }

        [HttpGet("survey/dashboard")]
        [Route("survey/dashboard/{surveyID:int}")]
        public async Task<IActionResult> DashboardAsync(int surveyID, int entity, int section)
        {
            var sections = await _db.Sections.OrderBy(x => x.Name).ToListAsync(); ;
            Section sectionData = null;

            int entityID = 0;
            byte[] bytes;
            if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
            {
                string value = Encoding.ASCII.GetString(bytes);
                int.TryParse(value, out entityID);
            }

            var entityList = await _db.Entities
                    .Where(x => x.Level <= 1 && (entityID == 0 ? true : x.ID == entityID))
                    .OrderBy(x => x.Name)
                    .ToListAsync();
            var entities = Entity.getEntities(entityList, 0, 0, true);
            var entityData = await _db.Entities.FirstOrDefaultAsync(x => x.ID == entity);

            if(entityID != 0)
            {
                entityData = await _db.Entities.FirstOrDefaultAsync(x => x.ID == entityID);
                entity = entityID;
            }

            var questionPackage = await _db.QuestionPackages
                .Include(x=>x.QuestionPackageEntities)
                .ThenInclude(x=>x.Entity)
                .FirstOrDefaultAsync(x => x.ID == surveyID);
            if (questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            if (section == 0)
            {
                sectionData = new Section
                {
                    ID = 0,
                    Name = "Peserta"
                };

                var participants = await _db.VwParticipant
                            .Include(x=>x.Entity)
                            .Include(x=>x.SubEntity)
                            .Where(x => x.QuestionPackageID == surveyID)
                            .OrderBy(x => x.EntityID)
                            .ToListAsync();
                var participantsPerEntities = participants
                    .Where(x=> entity == 0 ? x.Entity != null : x.Entity.ID == entity)
                    .GroupBy(x => x.Entity)
                    .ToDictionary(x => x.Key, y => new int[]
                    {
                        y.Count(),
                        y.Where(x => x.FinishedAt != null).Count(),
                        y.Where(x => x.FinishedAt == null && x.StartedAt != null).Count(),
                        y.Where(x => x.StartedAt == null).Count()
                    });

                var participantsPerSubEntities = participants
                    .Where(x=>x.SubEntity != null && (entity == 0 ? true : x.SubEntity.ParentEntity.ID == entity))
                    .GroupBy(x => x.SubEntity)
                    .ToDictionary(x => x.Key, y => new int[]
                    {
                        y.Count(),
                        y.Where(x => x.FinishedAt != null).Count(),
                        y.Where(x => x.FinishedAt == null && x.StartedAt != null).Count(),
                        y.Where(x => x.StartedAt == null).Count()
                    });

                ViewData["Participants"] = participantsPerEntities;
                ViewData["ParticipantSubs"] = participantsPerSubEntities;
            } 
            else
            {
                sectionData = await _db.Sections.FirstOrDefaultAsync(x => x.ID == section);
                if (sectionData == null)
                {
                    sectionData = sections.First();
                }

                if (sectionData.Construct == Construct.CULTURE)
                {
                    var cultureData = await _db.VwCulturePerVerticalDimention
                        .Include(x => x.VerticalDimention)
                        .Where(x =>
                            x.Participant.QuestionPackage.ID == surveyID &&
                            (entity == 0 || x.Participant.ParticipantUser.Entity.ID == entity)
                         )
                        .ToListAsync();

                    var dashboardCulture = cultureData
                        .GroupBy(x => x.VerticalDimention)
                        .ToDictionary(x => x.Key, y => y.Average(z => z.indexvaluesubject));

                    var cultureData1 = await _db.VwCulturePerSubVerticalDimention
                        .Include(x => x.SubVerticalDimention)
                        .Where(x =>
                            x.Participant.QuestionPackage.ID == surveyID &&
                            (entity == 0 || x.Participant.ParticipantUser.Entity.ID == entity)
                         )
                        .ToListAsync();

                    var dashboardCulture1 = cultureData1
                        .GroupBy(x => x.SubVerticalDimention.ValueDriverDimention)
                        .ToDictionary(x => x.Key, y => y.Average(z => z.indexsituasi));

                    ViewData["DashboardCulture"] = dashboardCulture;
                    ViewData["DashboardCulture1"] = dashboardCulture1;
                }
                else if (sectionData.Construct == Construct.ENGAGEMENT)
                {
                    var engagementData = await _db.VwEngagementPerHorizontalDimention
                        .Include(x => x.HorizontalDimention)
                        .Where(x =>
                            x.Participant.QuestionPackage.ID == surveyID &&
                            (entity == 0 || x.Participant.ParticipantUser.Entity.ID == entity)
                         )
                        .ToListAsync();

                    var dashboardEngagement = engagementData
                        .GroupBy(x => x.HorizontalDimention)
                        .ToDictionary(x => x.Key, y => y.Average(z => z.indexsituasi));

                    var engagementData1 = await _db.VwEngagementPerSubVerticalDimention
                        .Include(x => x.SubVerticalDimention)
                        .Where(x =>
                            x.Participant.QuestionPackage.ID == surveyID &&
                            (entity == 0 || x.Participant.ParticipantUser.Entity.ID == entity)
                         )
                        .ToListAsync();

                    var dashboardEngagement1 = engagementData1
                        .GroupBy(x => x.SubVerticalDimention)
                        .ToDictionary(x => x.Key, y => y.Average(z => z.indexsituasi));

                    ViewData["DashboardEngagement"] = dashboardEngagement;
                    ViewData["DashboardEngagement1"] = dashboardEngagement1;
                }
                else if (sectionData.Construct == Construct.PERFORMANCE)
                {
                    var performanceData = await _db.VwPerformancePerVerticalDimention
                        .Include(x => x.VerticalDimention)
                        .Where(x =>
                            x.Participant.QuestionPackage.ID == surveyID &&
                            (entity == 0 || x.Participant.ParticipantUser.Entity.ID == entity)
                         )
                        .ToListAsync();

                    var dashboardPerformance = performanceData
                        .GroupBy(x => x.VerticalDimention)
                        .ToDictionary(x => x.Key, y => y.Average(z => (z.indexvaluesubject * 100) / 6));

                    ViewData["DashboardPerformance"] = dashboardPerformance;
                }
            }

            ViewData["SideBarCollapse"] = true;
            ViewData["Entities"] = entities;
            ViewData["Entity"] = entityData;
            ViewData["Survey"] = questionPackage;
            ViewData["Sections"] = sections;
            ViewData["Section"] = sectionData;

            return View();
        }

        [HttpGet("survey/download")]
        [Route("survey/download/{surveyID:int}/{result:bool}")]
        [CustomAuthFilter("Access_MasterData_DaftarSurvei")]
        public async Task<IActionResult> DownloadAsync(int surveyID, int section, int entity = 0, bool result = false)
        {
            var questionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(x => x.ID == surveyID);

            if (questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            var sections = await _db.Sections.OrderBy(x => x.Name).ToListAsync(); ;
            Section sectionData = await _db.Sections.FirstOrDefaultAsync(x => x.ID == section);
            if (sectionData == null)
            {
                sectionData = sections.First();
            }
            var construct = sectionData.Construct;

            if (!result)
            {
                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var entityList = await _db.Entities
                        .Where(x => x.Level <= 1 && (entityID == 0 ? true : x.ID == entityID))
                        .OrderBy(x => x.Name)
                        .ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0, true);
                var entityData = await _db.Entities.FirstOrDefaultAsync(x => x.ID == entity);

                if (entityID != 0)
                {
                    entityData = await _db.Entities.FirstOrDefaultAsync(x => x.ID == entityID);
                    entity = entityID;
                }

                ViewData["Survey"] = questionPackage;
                ViewData["Sections"] = sections;
                ViewData["Section"] = sectionData;
                ViewData["Entities"] = entities;
                ViewData["Entity"] = entityData;
                return View();
            }

            var participants = new List<Participant>();
            var questions = new List<Question>();
            var verticalDimentions = new  List<VerticalDimention>();
            var horizontalDimentions = new List<HorizontalDimention>();

            if (section >= 0)
            {
                participants = await _db.Participants
                    .Include(x => x.ParticipantUser)
                    .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID && (entity == 0 ? true : x.ParticipantUser.Entity.ID == entity))
                    .OrderBy(x => x.ParticipantUser.Name)
                    .ToListAsync();

                questions = await _db.QuestionPackageLines
                    .Include(x => x.Question)
                    .ThenInclude(x => x.QuestionAnswerMatrixs)
                    .Include(x => x.Question.Section)
                    .Where(x => x.QuestionPackage.ID == surveyID && x.Question.Section.Construct == construct)
                    .Select(x => x.Question)
                    .ToListAsync();

                verticalDimentions = await _db.VerticalDimentions
                    .Include(x => x.Section)
                    .Include(x => x.SubVerticalDimentions)
                    .Where(x => x.Section.Construct == construct)
                    .ToListAsync();

                horizontalDimentions = await _db.HorizontalDimentions
                    .Include(x => x.Section)
                    .Where(x => x.Section.Construct == construct)
                    .ToListAsync();
            }
            else
            {
                participants = await _db.Participants
                    .Include(x => x.ParticipantUser)
                    .Include(x=>x.ParticipantUser.Entity)   
                    .Include(x => x.ParticipantUser.SubEntity)
                    .Include(x => x.ParticipantUser.JobLevel)
                    .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID && (entity == 0 ? true : x.ParticipantUser.Entity.ID == entity))
                    .OrderBy(x => x.ParticipantUser.Name)
                    .ToListAsync();

            }

            var stream = new MemoryStream();
            if (section == -1)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Demografi");
                    sheet1.Cells["A1"].Value = "No. Pekerja";
                    sheet1.Cells["B1"].Value = "Nama Pekerja";
                    sheet1.Cells["C1"].Value = "Holding/Subholding";
                    sheet1.Cells["D1"].Value = "Perusahaan";
                    sheet1.Cells["E1"].Value = "Jenis Kelamin";
                    sheet1.Cells["F1"].Value = "Usia";
                    sheet1.Cells["G1"].Value = "Masa Kerja";
                    sheet1.Cells["H1"].Value = "Level Jabatan";
                    sheet1.Cells["A1:H1"].Style.Font.Bold = true;

                    var row = 2;
                    foreach (var participant in participants)
                    {
                        sheet1.Cells[row, 1].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.Name;
                        sheet1.Cells[row, 3].Value = participant.ParticipantUser.Entity.Name;
                        sheet1.Cells[row, 4].Value = participant.ParticipantUser.SubEntity.Name;
                        sheet1.Cells[row, 5].Value = participant.ParticipantUser.Sex == 1 ? "Laki-laki" : (participant.ParticipantUser.Age == 0 ? "Perempuan" : "-");
                        sheet1.Cells[row, 6].Value = participant.ParticipantUser.Age;
                        sheet1.Cells[row, 7].Value = participant.ParticipantUser.WorkDuration;
                        sheet1.Cells[row, 8].Value = participant.ParticipantUser.JobLevel.Name;

                        row++;
                    }
                    package.Save();
                }
                stream.Position = 0;

                string excelName = $"Demografi-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName); // this will be the actual export.
            }
            else if (construct == Construct.CULTURE)
            {
                var vwCulturePerRows = await _db.VwCulturePerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();

                var answerCultures = new Dictionary<int, List<VwCulturePerRow>>();
                foreach (var row in vwCulturePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerCultures.ContainsKey(id))
                    {
                        answerCultures.Add(id, new List<VwCulturePerRow>());
                    }
                    answerCultures[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A3"].Merge = true;
                    sheet1.Cells["A1:A3"].Value = "No";
                    sheet1.Cells["B1:B3"].Merge = true;
                    sheet1.Cells["B1:B3"].Value = "Subject Number";
                    sheet1.Cells["C1:C3"].Merge = true;
                    sheet1.Cells["C1:C3"].Value = "Subject Name";
                    sheet1.Cells["D1"].Value = "Dimensi";
                    sheet1.Cells["D2"].Value = "Sub Dimensi";
                    sheet1.Cells["D3"].Value = "Item";

                    sheet1.Cells[1, 1, 3, 93].Style.Font.Bold = true;

                    int col = 5;
                    int subCol = 5;
                    int itemIndex = 1;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet1.Cells[1, col, 1, col + 14].Merge = true;
                        sheet1.Cells[1, col, 1, col + 14].Value = vd.Name;

                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet1.Cells[2, subCol, 2, subCol + 4].Merge = true;
                            sheet1.Cells[2, subCol, 2, subCol + 4].Value = svd.Name;
                            sheet1.Cells[3, subCol, 3, subCol + 4].Merge = true;
                            sheet1.Cells[3, subCol, 3, subCol + 4].Value = itemIndex;

                            subCol += 5;
                            itemIndex++;
                        }

                        col += 15;
                    }

                    int row = 4;
                    int no = 1;
                    foreach (Participant participant in participants)
                    {
                        if (!answerCultures.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerCultures[participant.ID];

                        sheet1.Cells[row, 1, row + 4, 1].Merge = true;
                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2, row + 4, 2].Merge = true;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet1.Cells[row, 3, row + 4, 3].Merge = true;
                        sheet1.Cells[row, 3].Value = participant.ParticipantUser.Name;

                        sheet1.Cells[row, 4].Value = "Respon";
                        sheet1.Cells[row + 1, 4].Value = "Urutan";
                        sheet1.Cells[row + 2, 4].Value = "Nilai";
                        sheet1.Cells[row + 3, 4].Value = "Bobot";
                        sheet1.Cells[row + 4, 4].Value = "Bobot x Nilai";
                        sheet1.Cells[row + 4, 4, row + 4, 4 + 90].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        sheet1.Cells[row + 4, 4, row + 4, 4 + 90].Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#A9D08E"));

                        col = 5;
                        foreach (Question question in questions)
                        {
                            itemIndex = 1;
                            foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if (asw != null)
                                {
                                    sheet1.Cells[row, col].Value = itemIndex;
                                    sheet1.Cells[row + 1, col].Value = asw.urutan;
                                    sheet1.Cells[row + 2, col].Value = asw.nilai;
                                    sheet1.Cells[row + 3, col].Value = asw.bobot;
                                    sheet1.Cells[row + 4, col].Formula = "=" + sheet1.Cells[row + 2, col].Address + "*" + sheet1.Cells[row + 3, col].Address;
                                }
                                itemIndex++;
                                col++;
                            }
                        }

                        row += 5;
                        no++;
                    }

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Ringkasan");
                    sheet2.Cells["A1:A3"].Merge = true;
                    sheet2.Cells["A1:A3"].Value = "No";
                    sheet2.Cells["B1:B3"].Merge = true;
                    sheet2.Cells["B1:B3"].Value = "Subject Number";
                    sheet2.Cells["C1:C3"].Merge = true;
                    sheet2.Cells["C1:C3"].Value = "Subject Name";

                    sheet2.Cells[1, 1, 3, 46].Style.Font.Bold = true;
                    col = 4;
                    subCol = 4;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet2.Cells[1, col, 1, col + 6].Merge = true;
                        sheet2.Cells[1, col, 1, col + 6].Value = vd.Name;

                        sheet2.Cells[2, col, 2, col + 2].Merge = true;
                        sheet2.Cells[2, col, 2, col + 2].Value = "Skor Situasi";
                        sheet2.Cells[2, col + 3, 2, col + 5].Merge = true;
                        sheet2.Cells[2, col + 3, 2, col + 5].Value = "Index Situasi";

                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[3, subCol].Value = svd.Name;
                            subCol++;
                        }
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[3, subCol].Value = svd.Name;
                            subCol++;
                        }

                        sheet2.Cells[2, col + 6, 3, col + 6].Merge = true;
                        sheet2.Cells[2, col + 6, 3, col + 6].Value = "Indexs Value Subjek";

                        col += 7;
                        subCol++;
                    }
                    sheet2.Cells[1, col, 3, col].Merge = true;
                    sheet2.Cells[1, col, 3, col].Value = "Index Akhlak Subjek";

                    row = 4;
                    no = 1;
                    foreach (Participant participant in participants)
                    {
                        if (!answerCultures.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerCultures[participant.ID];

                        sheet2.Cells[row, 1].Value = no;
                        sheet2.Cells[row, 2].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet2.Cells[row, 3].Value = participant.ParticipantUser.Name;

                        col = 4;
                        subCol = 4;
                        var averageCols = "";
                        int colIndex = 0;
                        foreach (VerticalDimention vd in verticalDimentions)
                        {
                            foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                            {
                                sheet2.Cells[row, subCol].Formula = "=SUM('Hasil Jawaban'!" + sheet1.Cells[8 + (row - 4) * 5, 4 + colIndex * 5].Address + ":" + sheet1.Cells[8 + (row - 4) * 5, 8 + colIndex * 5].Address + ")";
                                subCol++;
                                colIndex++;
                            }
                            foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                            {
                                sheet2.Cells[row, subCol].Formula = "=((" + sheet2.Cells[row, subCol - 3].Address + "-39)/76)*100";
                                sheet2.Cells[row, subCol].Style.Numberformat.Format = "0.00";
                                subCol++;
                            }

                            sheet2.Cells[row, col + 6].Formula = "=AVERAGE(" + sheet2.Cells[row, col + 3].Address + ":" + sheet2.Cells[row, col + 5].Address + ")";
                            sheet2.Cells[row, col + 6].Style.Numberformat.Format = "0.00";

                            if (averageCols == "")
                            {
                                averageCols += sheet2.Cells[row, col + 6].Address;
                            }
                            else
                            {
                                averageCols += "," + sheet2.Cells[row, col + 6].Address;
                            }

                            col += 7;
                            subCol++;
                        }
                        sheet2.Cells[row, col].Formula = "=AVERAGE(" + averageCols + ")";
                        sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                        row++;
                        no++;
                    }

                    var totalRow = no - 1;

                    sheet2.Cells[row, 1, row, 3].Merge = true;
                    sheet2.Cells[row, 1, row, 3].Value = "Index Value";
                    subCol = 4;
                    col = 4;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet2.Cells[row, col, row, col + 2].Merge = true;
                        subCol += 3;
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet2.Cells[row, subCol].Formula = "=AVERAGE(" + sheet2.Cells[4, subCol].Address + ":" + sheet2.Cells[3 + totalRow, subCol].Address + ")";
                            sheet2.Cells[row, subCol].Style.Numberformat.Format = "0.00";
                            subCol++;
                        }

                        sheet2.Cells[row, col + 6].Formula = "=AVERAGE(" + sheet2.Cells[4, subCol].Address + ":" + sheet2.Cells[3 + totalRow, subCol].Address + ")";
                        sheet2.Cells[row, col + 6].Style.Numberformat.Format = "0.00";

                        col += 7;
                        subCol++;
                    }

                    sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[4, col].Address + ":" + sheet2.Cells[3 + totalRow, col].Address + ")";
                    sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                    sheet2.Cells[row, 1, row, 46].Style.Font.Bold = true;

                    // Sheet 3
                    var sheet3 = package.Workbook.Worksheets.Add("Laporan");
                    row = 4 + totalRow;
                    no = 1;
                    col = 7;
                    sheet3.Cells[no, 1].Value = no;
                    sheet3.Cells[no, 2].Value = "Indeks Akhlak";
                    sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, 45].Address;
                    sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        no++;
                        sheet3.Cells[no, 1].Value = no;
                        sheet3.Cells[no, 2].Value = "-- Indeks " + vd.Name;
                        sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col + 3].Address;
                        sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            no++;
                            sheet3.Cells[no, 1].Value = no;
                            sheet3.Cells[no, 2].Value = "---- Indeks " + svd.Name;
                            sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col].Address;
                            sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";
                            col++;
                        }

                        col += 4;
                    }

                    package.Save();
                }
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Culture-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            }
            else if (construct == Construct.ENGAGEMENT)
            {
                var vwEngagementPerRows = await _db.VwEngagementPerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();

                var answerEngagements = new Dictionary<int, List<VwEngagementPerRow>>();
                foreach (VwEngagementPerRow row in vwEngagementPerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerEngagements.ContainsKey(id))
                    {
                        answerEngagements.Add(id, new List<VwEngagementPerRow>());
                    }
                    answerEngagements[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A2"].Merge = true;
                    sheet1.Cells["A1:A2"].Value = "No";
                    sheet1.Cells["B1:C1"].Merge = true;
                    sheet1.Cells["B1:C1"].Value = "Dimensi";
                    sheet1.Cells["B2"].Value = "Subjek No";
                    sheet1.Cells["C2"].Value = "Subjek Name";

                    int col = 3;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet1.Cells[1, col + 1, 1, col + 20].Merge = true;
                        sheet1.Cells[1, col + 1, 1, col + 20].Value = hd.Name;
                        for (int i = 1; i <= 20; i++)
                        {
                            sheet1.Cells[2, col + i].Value = i;
                        }
                        col += 20;
                    }

                    sheet1.Cells[1, 1, 2, 83].Style.Font.Bold = true;

                    var no = 1;
                    var row = 3;
                    foreach (Participant participant in participants)
                    {
                        if (!answerEngagements.ContainsKey(participant.ID))
                        {
                            continue;
                        }
                        var answer = answerEngagements[participant.ID];

                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet1.Cells[row, 3].Value = participant.ParticipantUser.Name;

                        col = 4;
                        foreach (Question question in questions)
                        {
                            foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if (asw != null)
                                {
                                    sheet1.Cells[row, col].Value = asw.nilai;
                                }
                                col++;
                            }
                        }

                        no++;
                        row++;
                    }

                    var totalRow = no - 1;

                    sheet1.Cells[row, 1, row, 3].Merge = true;
                    sheet1.Cells[row, 1, row, 3].Value = "Rata-rata";
                    sheet1.Cells[row, 1, row, 82].Style.Font.Bold = true;

                    col = 4;
                    foreach (Question question in questions)
                    {
                        foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                        {
                            sheet1.Cells[row, col].Formula = "=AVERAGE(" + sheet1.Cells[3, col].Address + ":" + sheet1.Cells[2 + totalRow, col].Address + ")";
                            sheet1.Cells[row, col].Style.Numberformat.Format = "0.00";
                            col++;
                        }
                    }

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Ringkasan");
                    sheet2.Cells["A1:A2"].Merge = true;
                    sheet2.Cells["A1:A2"].Value = "No";
                    sheet2.Cells["B1:B2"].Merge = true;
                    sheet2.Cells["B1:B2"].Value = "Subjek";
                    sheet2.Cells["C1:C2"].Merge = true;
                    sheet2.Cells["C1:C2"].Value = "Subjek";

                    sheet2.Cells["D1:G1"].Merge = true;
                    sheet2.Cells["D1:G1"].Value = "Skor Subjek";
                    sheet2.Cells["H1:K1"].Merge = true;
                    sheet2.Cells["H1:K1"].Value = "Index Subjek";
                    sheet2.Cells["L1:L2"].Merge = true;
                    sheet2.Cells["L1:L2"].Value = "Index Engagement Subjek";

                    sheet2.Cells["A1:L2"].Style.Font.Bold = true;

                    row = 2;
                    col = 4;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet2.Cells[row, col].Value = hd.Name;
                        sheet2.Cells[row, col + 4].Value = hd.Name;
                        col++;
                    }

                    row = 3;
                    no = 1;
                    foreach (Participant participant in participants)
                    {
                        if (!answerEngagements.ContainsKey(participant.ID))
                        {
                            continue;
                        }

                        sheet2.Cells[row, 1].Value = no;
                        sheet2.Cells[row, 2].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet2.Cells[row, 3].Value = participant.ParticipantUser.Name;

                        col = 4;
                        var colIndex = 1;
                        var averageCols = "";
                        foreach (HorizontalDimention hd in horizontalDimentions)
                        {
                            sheet2.Cells[row, col].Formula = "=AVERAGE('Hasil Jawaban'!" + sheet1.Cells[row, 3 + ((colIndex - 1) * 20)].Address + ":" + sheet1.Cells[row, 2 + (colIndex * 20)].Address + ")";
                            sheet2.Cells[row, col + 4].Formula = "=(" + sheet2.Cells[row, col].Address + "/ 6)*100";

                            if (averageCols == "")
                            {
                                averageCols += sheet2.Cells[row, col + 4].Address;
                            }
                            else
                            {
                                averageCols += "," + sheet2.Cells[row, col + 4].Address;
                            }

                            colIndex++;
                            col++;
                        }

                        sheet2.Cells[row, col + 4].Formula = "=AVERAGE(" + averageCols + ")";
                        sheet2.Cells[row, 3, row, col + 4].Style.Numberformat.Format = "0.00";

                        row++;
                        no++;
                    }

                    sheet2.Cells[row, 1, row, 2].Merge = true;
                    sheet2.Cells[row, 1, row, 2].Value = "Index Value";

                    sheet2.Cells[row, 3, row, 6].Merge = true;
                    col = 8;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[3, col].Address + ":" + sheet2.Cells[2 + totalRow, col].Address + ")";
                        sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";
                        col++;
                    }
                    sheet2.Cells[row, col].Formula = "=AVERAGE(" + sheet2.Cells[3, col].Address + ":" + sheet2.Cells[2 + totalRow, col].Address + ")";
                    sheet2.Cells[row, col].Style.Numberformat.Format = "0.00";

                    sheet2.Cells[row, 1, row, 12].Style.Font.Bold = true;

                    // Sheet 3
                    var sheet3 = package.Workbook.Worksheets.Add("Laporan 1");
                    row = 3 + totalRow;
                    no = 1;
                    sheet3.Cells[no, 1].Value = no;
                    sheet3.Cells[no, 2].Value = "Indeks Engagement";
                    sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, 11].Address;
                    sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    col = 8;
                    foreach (HorizontalDimention hd in horizontalDimentions)
                    {
                        no++;
                        sheet3.Cells[no, 1].Value = no;
                        sheet3.Cells[no, 2].Value = "-- Indeks " + hd.Name;
                        sheet3.Cells[no, 3].Formula = "='Ringkasan'!" + sheet2.Cells[row, col].Address;
                        sheet3.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        col++;
                    }

                    // Sheet 4
                    var sheet4 = package.Workbook.Worksheets.Add("Laporan 2");
                    sheet4.Cells["A1"].Value = "No";
                    sheet4.Cells["B1"].Value = "Faktor";
                    sheet4.Cells["C1"].Value = "Skor";
                    sheet4.Cells["D1"].Value = "Index";
                    sheet4.Cells["A1:D1"].Style.Font.Bold = true;

                    row = 2;
                    col = 4;
                    no = 1;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet4.Cells[row, 1].Value = no;
                            sheet4.Cells[row, 2].Value = svd.Name;

                            var averages = "";
                            for (int i = 0; i < 4; i++)
                            {
                                if (averages != "")
                                {
                                    averages += ",";
                                }
                                averages += "'Hasil Jawaban'!" + sheet1.Cells[3 + participants.Count, col + (i * 20)].Address;
                            }
                            sheet4.Cells[row, 3].Formula = "=AVERAGE(" + averages + ")";
                            sheet4.Cells[row, 4].Formula = "=(" + sheet4.Cells[row, 3].Address + "/6)*100";
                            sheet4.Cells[row, 3].Style.Numberformat.Format = "0.00";
                            sheet4.Cells[row, 4].Style.Numberformat.Format = "0.00";

                            row++;
                            col++;
                            no++;
                        }
                    }

                    var cells = sheet4.Cells[1, 1, row, 4];
                    cells.AutoFilter = true;

                    package.Save();
                }

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Engagement-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            }
            else if (construct == Construct.PERFORMANCE)
            {
                var vwPerformancePerRows = await _db.VwPerformancePerRow
                    .Where(x => x.Participant.QuestionPackage.ID == surveyID)
                    .ToListAsync();
                var answerPerformances = new Dictionary<int, List<VwPerformancePerRow>>();
                foreach (VwPerformancePerRow row in vwPerformancePerRows)
                {
                    var id = row.ParticipantID;
                    if (!answerPerformances.ContainsKey(id))
                    {
                        answerPerformances.Add(id, new List<VwPerformancePerRow>());
                    }
                    answerPerformances[id].Add(row);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    // Sheet 1
                    var sheet1 = package.Workbook.Worksheets.Add("Hasil Jawaban");
                    sheet1.Cells["A1:A4"].Merge = true;
                    sheet1.Cells["A1:A4"].Value = "No";
                    sheet1.Cells["B1:B4"].Merge = true;
                    sheet1.Cells["B1:B4"].Value = "Subjek No";
                    sheet1.Cells["C1:C4"].Merge = true;
                    sheet1.Cells["C1:C4"].Value = "Subjek Name";

                    sheet1.Cells["D1:I1"].Merge = true;
                    sheet1.Cells["D1:I1"].Value = "Kinerja Organisasi";

                    int row = 2;
                    int col = 4;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        sheet1.Cells[row, col, row, col + 5].Merge = true;
                        sheet1.Cells[row, col, row, col + 5].Value = vd.Name;
                        foreach (SubVerticalDimention svd in vd.SubVerticalDimentions)
                        {
                            sheet1.Cells[row + 1, col, row + 1, col + 1].Merge = true;
                            sheet1.Cells[row + 1, col, row + 1, col + 1].Value = svd.Name;

                            for (int i = 1; i <= 2; i++)
                            {
                                sheet1.Cells[row + 2, col].Value = col - 2;
                                col++;
                            }
                        }
                    }

                    sheet1.Cells[1, col, 4, col].Merge = true;
                    sheet1.Cells[1, col, 4, col].Value = "Skor Kinerja Subjek";
                    sheet1.Cells[1, col + 1, 4, col + 1].Merge = true;
                    sheet1.Cells[1, col + 1, 4, col + 1].Value = "Index Kinerja Subjek";

                    sheet1.Cells[1, 1, 4, 16].Style.Font.Bold = true;

                    row = 5;
                    int no = 1;
                    foreach (Participant participant in participants)
                    {
                        if (!answerPerformances.ContainsKey(participant.ID))
                        {
                            continue;
                        }

                        sheet1.Cells[row, 1].Value = no;
                        sheet1.Cells[row, 2].Value = participant.ParticipantUser.EmployeeNumber;
                        sheet1.Cells[row, 3].Value = participant.ParticipantUser.Name;

                        var answer = answerPerformances[participant.ID];

                        col = 4;
                        foreach (Question question in questions)
                        {
                            foreach (QuestionAnswer qa in question.QuestionAnswerMatrixs)
                            {
                                var asw = answer.FirstOrDefault(x => x.QuestionID == question.ID && x.MatrixRowAnswerID == qa.ID);
                                if (asw != null)
                                {
                                    sheet1.Cells[row, col].Value = asw.nilai;
                                }
                                col++;
                            }
                        }
                        sheet1.Cells[row, col].Formula = "=AVERAGE(" + sheet1.Cells[row, 3, row, 14].Address + ")";
                        sheet1.Cells[row, col].Style.Numberformat.Format = "0.00";
                        sheet1.Cells[row, col + 1].Formula = "=(" + sheet1.Cells[row, col].Address + "/6)*100";
                        sheet1.Cells[row, col + 1].Style.Numberformat.Format = "0.00";

                        no++;
                        row++;
                    }

                    var totalRow = no - 1;

                    sheet1.Cells[row, 1, row + 1, 3].Merge = true;
                    sheet1.Cells[row, 1, row + 1, 3].Value = "Index Kinerja Organisasi";

                    sheet1.Cells[row, 4, row, 9].Merge = true;
                    sheet1.Cells[row, 4, row, 9].Formula = "=AVERAGE(" + sheet1.Cells[5, 4, 4 + totalRow, 9].Address + ")";
                    sheet1.Cells[row, 4, row, 9].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row, 10, row, 15].Merge = true;
                    sheet1.Cells[row, 10, row, 15].Formula = "=AVERAGE(" + sheet1.Cells[5, 10, 4 + totalRow, 15].Address + ")";
                    sheet1.Cells[row, 10, row, 15].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 4, row + 1, 9].Merge = true;
                    sheet1.Cells[row + 1, 4, row + 1, 9].Formula = "=(" + sheet1.Cells[row, 4].Address + "/6)*100";
                    sheet1.Cells[row + 1, 4, row + 1, 9].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 10, row + 1, 15].Merge = true;
                    sheet1.Cells[row + 1, 10, row + 1, 15].Formula = "=(" + sheet1.Cells[row, 10].Address + "/6)*100";
                    sheet1.Cells[row + 1, 10, row + 1, 15].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row + 1, 17].Formula = "=AVERAGE(" + sheet1.Cells[5, 17, 4 + totalRow, 17].Address + ")";
                    sheet1.Cells[row + 1, 17].Style.Numberformat.Format = "0.00";

                    sheet1.Cells[row, 1, row + 1, 17].Style.Font.Bold = true;

                    // Sheet 2
                    var sheet2 = package.Workbook.Worksheets.Add("Laporan");
                    row = 3 + totalRow;
                    no = 1;
                    sheet2.Cells[no, 1].Value = no;
                    sheet2.Cells[no, 2].Value = "Indeks Kinerja Organisasi	";
                    sheet2.Cells[no, 3].Formula = "='Hasil Jawaban'!" + sheet1.Cells[row + 3, 17].Address;
                    sheet2.Cells[no, 3].Style.Numberformat.Format = "0.00";

                    col = 4;
                    foreach (VerticalDimention vd in verticalDimentions)
                    {
                        no++;
                        sheet2.Cells[no, 1].Value = no;
                        sheet2.Cells[no, 2].Value = "-- Indeks " + vd.Name;
                        sheet2.Cells[no, 3].Formula = "='Hasil Jawaban'!" + sheet1.Cells[row + 3, col].Address;
                        sheet2.Cells[no, 3].Style.Numberformat.Format = "0.00";

                        col += 6;
                    }

                    package.Save();
                }

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Performance-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx"); // this will be the actual export.
            }

            return null;
        }
    }
}
