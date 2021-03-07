using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    public class SurveyQuestionController : Controller
    {
        private readonly PostgreDbContext _db;
        public SurveyQuestionController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("survey-question/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int surveyID = 0)
        {
            try
            {
                var data = await _db.QuestionPackageLines
                    .Include(x=> x.QuestionPackage)
                    .Include(x => x.Question)
                    .Include(x => x.Question.Section)
                    .Include(x => x.Question.Section.Assesment)
                    .Where(x=> x.QuestionPackage.ID == surveyID)
                    .OrderBy(x=>x.Question.Section.Sequence)
                    .ThenBy(x => x.Question.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] {
                            row.Question.Section.Name + " - " + row.Question.Section.Assesment.Name,
                            row.Question.Sequence.ToString(),
                            row.Question.Title
                        } 
                    });
                }

                ViewData["Rows"] = rows;
                ViewData["Page"] = page;
                ViewData["IsEditable"] = false;

                return PartialView("~/Views/Shared/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey-question/table-paging-view")]
        public IActionResult GetPaging(int page = 1, int surveyID = 0)
        {
            try
            {
                var total = _db.QuestionPackageLines
                    .Where(x => x.QuestionPackage.ID == surveyID)
                    .Count();
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

        [HttpGet("survey-question/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0, int surveyID = 0)
        {
            try
            {
                var questionPackage = await _db.QuestionPackages
                    .Include(x => x.Assesment)
                    .Include(x => x.QuestionPackageLines)
                    .ThenInclude(x=>x.Question)
                    .FirstOrDefaultAsync(x => x.ID == surveyID);

                if (questionPackage == null)
                {
                    return null;
                }

                QuestionPackageLine questionPackageLineFromDb = null;
                if(id != 0)
                {
                    questionPackageLineFromDb = await _db.QuestionPackageLines.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();

                var questionExists = questionPackage
                    .QuestionPackageLines.Select(x => x.Question.ID).ToList();
                var questions = await _db.Questions
                    .Include(x => x.Section)
                    .Where(x => x.Section.Assesment == questionPackage.Assesment && !questionExists.Contains(x.ID))
                    .OrderBy(x=>x.Section.Sequence)
                    .ThenBy(x=>x.Sequence)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Section.Name + " | " + y.Sequence.ToString() + " - " + y.Title);

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionPackageLineFromDb == null ? "0" : questionPackageLineFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Pertanyaan", Name = "Question", InputType = InputType.DROPDOWN, Options = questions, Value = questionPackageLineFromDb == null ? "" : questionPackageLineFromDb.Question.ID.ToString(), IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey-question")]
        [Route("survey-question/{surveyID:int}")]
        public async Task<IActionResult> IndexAsync(int surveyID)
        {
            var questionPackage = await _db.QuestionPackages
                .Include(x=>x.Assesment)
                .FirstOrDefaultAsync(x => x.ID == surveyID);

            if (questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            ViewData["Title"] = "Daftar Soal | " + questionPackage.Assesment.Name + " - " + questionPackage.Name;
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Section", Style = "width: 10%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence", Style = "width: 5%; min-width: 50px" });
            ColumnModels.Add(new ColumnModel { Label = "Soal", Name = "Title", Style = "width: 25%; min-width: 250px" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "survey-question.js";
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Daftar Survey", "/survey"}
            }; 
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"QuestionPackage", surveyID.ToString()}
            };
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("survey-question/save")]
        public async Task<IActionResult> Save(QuestionPackageLine questionPackageLine)
        {
            try
            {
                QuestionPackageLine questionPackageLineFromDb = await _db.QuestionPackageLines.FirstOrDefaultAsync(e => e.ID == questionPackageLine.ID);

                if (questionPackageLineFromDb == null)
                {
                    questionPackageLine.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackageLine.QuestionPackage.ID);
                    questionPackageLine.Question = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionPackageLine.Question.ID);
                   
                    _db.QuestionPackageLines.Add(questionPackageLine);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionPackageLineFromDb.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackageLine.QuestionPackage.ID);
                    questionPackageLineFromDb.Question = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionPackageLine.Question.ID);

                    _db.QuestionPackageLines.Update(questionPackageLineFromDb);
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

        [HttpPost("survey-question/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionPackageLineFromDb = await _db.QuestionPackageLines.FirstOrDefaultAsync(e => e.ID == id);

                if (questionPackageLineFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionPackageLines.Remove(questionPackageLineFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }
    }
}
