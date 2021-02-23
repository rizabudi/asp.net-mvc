using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class QuestionController : Controller
    {
        private readonly PostgreDbContext _db;
        public QuestionController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("question/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Questions
                    .Include("Section")
                    .Include("QuestionAnswers")
                    .Include("QuestionAnswerMatrixs")
                    .OrderBy(x=>x.Section.Sequence)
                    .OrderBy(x=>x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var answer = row.QuestionAnswers.Count() + row.QuestionAnswerMatrixs.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Sequence.ToString(), 
                            "HTML:<a href='/sub-question/" + row.ID + "'>" + answer + " Jawaban</a>"
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

        [HttpGet("question/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Questions.Count();
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

        [HttpGet("question/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Question questionFromDb = null;
                if(id != 0)
                {
                    questionFromDb = await _db.Questions.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionFromDb == null ? "0" : questionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Konstruk", Name = "Section", InputType = InputType.TEXT, Value = questionFromDb == null ? "" : questionFromDb.Section.Name + " - " + questionFromDb.Section.Assesment.Name });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = questionFromDb == null ? "" : questionFromDb.Sequence.ToString() });
                FormModels.Add(new FormModel { Label = "Soal", Name = "Title", InputType = InputType.TEXTAREA, Value = questionFromDb == null ? "" : questionFromDb.Title });
                FormModels.Add(new FormModel { Label = "Tipe Soal", Name = "QuestionType", InputType = InputType.TEXT, Value = questionFromDb == null ? "" : questionFromDb.QuestionType.ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("question")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Pertanyaan";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Section", Style = "width: 10%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence", Style = "width: 5%; min-width: 50px" });
            ColumnModels.Add(new ColumnModel { Label = "Soal", Name = "Title", Style = "width: 25%; min-width: 250px" });
            ColumnModels.Add(new ColumnModel { Label = "Tipe Soal", Name = "QuestionType" });
            ColumnModels.Add(new ColumnModel { Label = "Harus Diisi", Name = "IsMandatory" });
            ColumnModels.Add(new ColumnModel { Label = "Jawaban Acak", Name = "IsRandomAnswer" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "question.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("question/save")]
        public async Task<IActionResult> Save(Question question)
        {
            try
            {
                Question questionFromDb = await _db.Questions.FirstOrDefaultAsync(e => e.ID == question.ID);

                if (questionFromDb == null)
                {
                    _db.Questions.Add(question);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionFromDb.Title = question.Title;
                    questionFromDb.Description = question.Description;
                    _db.Questions.Update(questionFromDb);
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

        [HttpPost("question/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionFromDb = await _db.Questions.FirstOrDefaultAsync(e => e.ID == id);

                if (questionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Questions.Remove(questionFromDb);
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
