using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    .OrderBy(x => x.Assesment.Name)
                    .ThenBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var questions = row.QuestionPackageLines.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Assesment.Name,
                            row.Name,
                            "HTML:<a href='/survey-question/" + row.ID + "'><i class='fa fa-question'></i> " + questions + " Soal</a>",
                            "HTML:<a href='/survey/result/" + row.ID + "'><i class='fa fa-file'></i> Hasil Survei</a>"
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
        public IActionResult Index()
        {
            ViewData["Title"] = "Daftar Survei";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Jenis Survei", Name = "Assesment", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Daftar Soal", Name = "Questions" });
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
        [Route("survey/result/{surveyID:int}")]
        public async Task<IActionResult> ResultAsync(int surveyID)
        {
            var questionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(x => x.ID == surveyID);

            if(questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            var participants = await _db.Participants
                .Include(x=>x.ParticipantUser)
                .Where(x => x.FinishedAt != null && x.QuestionPackage.ID == surveyID)
                .OrderBy(x=>x.ParticipantUser.Name)
                .ToListAsync();

            var questions = await _db.QuestionPackageLines
                .Include(x=>x.Question)
                .ThenInclude(x=>x.QuestionAnswerMatrixs)
                .Include(x => x.Question.Section)
                .Where(x => x.QuestionPackage.ID == surveyID)
                .Select(x=>x.Question)
                .ToListAsync();

            var verticalDimentions = await _db.VerticalDimentions
                .Include(x=>x.Section)
                .Include(x=>x.SubVerticalDimentions)
                .ToListAsync();

            var horizontalDimentions = await _db.HorizontalDimentions
                .Include(x => x.Section)
                .ToListAsync();

            var vwCulturePerRows = await _db.VwCulturePerRow
                .ToListAsync();
            var vwPerformancePerRows = await _db.VwPerformancePerRow
                .ToListAsync();
            var vwEngagementPerRows = await _db.VwEngagementPerRow
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

            ViewData["Survey"] = questionPackage;
            ViewData["Participants"] = participants;
            ViewData["VerticalDimentions"] = verticalDimentions;
            ViewData["HorizontalDimentions"] = horizontalDimentions;
            ViewData["Questions"] = questions;
            ViewData["AnswerCultures"] = answerCultures;
            ViewData["AnswerPerformances"] = answerPerformances;
            ViewData["AnswerEngagements"] = answerEngagements;

            ViewData["SideBarCollapse"] = true;


            return View();
        }
    }
}
