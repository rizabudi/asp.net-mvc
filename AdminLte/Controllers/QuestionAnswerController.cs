﻿using AdminLte.Data;
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
    public class QuestionAnswerController : Controller
    {
        private readonly PostgreDbContext _db;
        public QuestionAnswerController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("question-answer/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int questionID = 0)
        {
            try
            {
                var data = await _db.QuestionAnswer
                    .Include("Question")
                    .Include("MatrixQuestion")
                    .Include("VerticalDimention")
                    .Include("SubVerticalDimention")
                    .Include("HorizontalDimention")
                    .Where(x=> x.Question.ID == questionID || x.MatrixQuestion.ID == questionID)
                    .OrderBy(x=> x.MatrixQuestion == null ? 1 : 2)
                    .ThenBy(x=>x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] {
                            row.Sequence.ToString(),
                            row.Value,
                            row.Question == null ? "Baris" : "Kolom",
                            row.Weight.ToString(),
                            row.AnswerScore.ToString(),
                            row.VerticalDimention != null ? row.VerticalDimention.Name : "",
                            row.SubVerticalDimention != null ? row.SubVerticalDimention.Name : "",
                            row.HorizontalDimention != null ? row.HorizontalDimention.Name : ""
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

        [HttpGet("question-answer/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.QuestionAnswer.Count();
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

        [HttpGet("question-answer/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                QuestionAnswer questionAnswerFromDb = null;
                if(id != 0)
                {
                    questionAnswerFromDb = await _db.QuestionAnswer
                        .Include("Question")
                        .Include("MatrixQuestion")
                        .FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();

                var answerTypes = new Dictionary<string, string>()
                {
                    {"1", "Kolom"},
                    {"2", "Baris"},
                };

                var verticalDimentions = await _db.VerticalDimentions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var subVerticalDimentions = await _db.SubVerticalDimentions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var horizontalDimentions = await _db.HorizontalDimentions.OrderBy(x => x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionAnswerFromDb == null ? "0" : questionAnswerFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = questionAnswerFromDb == null ? "" : questionAnswerFromDb.Sequence.ToString() });
                FormModels.Add(new FormModel { Label = "Jawaban", Name = "Value", InputType = InputType.TEXTAREA, Value = questionAnswerFromDb == null ? "" : questionAnswerFromDb.Value });
                FormModels.Add(new FormModel { Label = "Tampilan Matrix", Name = "Type", InputType = InputType.DROPDOWN, Options = answerTypes, Value = questionAnswerFromDb == null ? "" : questionAnswerFromDb.Question != null ? "1" : "2" });
                FormModels.Add(new FormModel { Label = "Bobot", Name = "Weight", InputType = InputType.NUMBER, Value = questionAnswerFromDb == null ? "" : questionAnswerFromDb.Weight.ToString() });
                FormModels.Add(new FormModel { Label = "Nilai", Name = "AnswerScore", InputType = InputType.NUMBER, Value = questionAnswerFromDb == null ? "" : questionAnswerFromDb.AnswerScore.ToString() });
                FormModels.Add(new FormModel { Label = "Dimensi Vertical", Name = "VerticalDimention", InputType = InputType.DROPDOWN, Options = verticalDimentions, Value = questionAnswerFromDb == null || questionAnswerFromDb.VerticalDimention == null ? "" : ((int)questionAnswerFromDb.VerticalDimention.ID).ToString(), FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Sub Dimensi Vertical", Name = "SubVerticalDimention", InputType = InputType.DROPDOWN, Options = subVerticalDimentions, Value = questionAnswerFromDb == null || questionAnswerFromDb.SubVerticalDimention == null ? "" : ((int)questionAnswerFromDb.SubVerticalDimention.ID).ToString(), FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Dimensi Horizontal", Name = "HorizontalDimention", InputType = InputType.DROPDOWN, Options = horizontalDimentions, Value = questionAnswerFromDb == null || questionAnswerFromDb.HorizontalDimention == null ? "" : ((int)questionAnswerFromDb.HorizontalDimention.ID).ToString(), FormPosition = FormPosition.RIGHT });

                ViewData["Forms"] = FormModels;
                ViewData["ColumnNumber"] = 2;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("question-answer")]
        [Route("question-answer/{questionID:int}")]
        public async Task<IActionResult> IndexAsync(int questionID)
        {
            var question = await _db.Questions.FirstOrDefaultAsync(x => x.ID == questionID);

            if (question == null)
            {
                return Redirect("/home/errors/404");
            }

            ViewData["Title"] = "Daftar Jawaban | " + question.Title;
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence", Style = "width: 5%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Jawaban", Name = "Value" });
            ColumnModels.Add(new ColumnModel { Label = "Tampilan Matrix", Name = "TypeAnswer" });
            ColumnModels.Add(new ColumnModel { Label = "Bobot", Name = "Weight" });
            ColumnModels.Add(new ColumnModel { Label = "Nilai", Name = "AnswerScore" });
            ColumnModels.Add(new ColumnModel { Label = "Dimensi Vertikal", Name = "VerticalDimention" });
            ColumnModels.Add(new ColumnModel { Label = "Sub Dimensi Vertikal", Name = "SubVerticalDimention" });
            ColumnModels.Add(new ColumnModel { Label = "Dimensi Horizontal", Name = "HorizontalDimention" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "question-answer.js";
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Question", "/question"}
            }; 
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"Question", questionID.ToString()}
            };
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("question-answer/save")]
        public async Task<IActionResult> Save(QuestionAnswer questionAnswer)
        {
            try
            {
                QuestionAnswer questionAnswerFromDb = await _db.QuestionAnswer
                    .Include("VerticalDimention")
                    .Include("SubVerticalDimention")
                    .Include("HorizontalDimention")
                    .Include("Question")
                    .Include("MatrixQuestion")
                    .FirstOrDefaultAsync(e => e.ID == questionAnswer.ID);

                if (questionAnswerFromDb == null)
                {
                    questionAnswer.Question = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionAnswer.Question.ID);
                    questionAnswer.MatrixQuestion = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionAnswer.MatrixQuestion.ID);
                    questionAnswer.VerticalDimention = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.VerticalDimention.ID);
                    questionAnswer.SubVerticalDimention = await _db.SubVerticalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.SubVerticalDimention.ID);
                    questionAnswer.HorizontalDimention = await _db.HorizontalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.HorizontalDimention.ID);

                    _db.QuestionAnswer.Add(questionAnswer);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionAnswerFromDb.Question = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionAnswer.Question.ID);
                    questionAnswerFromDb.MatrixQuestion = await _db.Questions.FirstOrDefaultAsync(e => e.ID == questionAnswer.MatrixQuestion.ID);
                    questionAnswerFromDb.VerticalDimention = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.VerticalDimention.ID);
                    questionAnswerFromDb.SubVerticalDimention = await _db.SubVerticalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.SubVerticalDimention.ID);
                    questionAnswerFromDb.HorizontalDimention = await _db.HorizontalDimentions.FirstOrDefaultAsync(e => e.ID == questionAnswer.HorizontalDimention.ID);

                    questionAnswerFromDb.Sequence = questionAnswer.Sequence;
                    questionAnswerFromDb.Value = questionAnswer.Value;
                    questionAnswerFromDb.Weight = questionAnswer.Weight;
                    questionAnswerFromDb.AnswerScore = questionAnswer.AnswerScore;

                    _db.QuestionAnswer.Update(questionAnswerFromDb);
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

        [HttpPost("question-answer/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var periodFromDb = await _db.QuestionAnswer.FirstOrDefaultAsync(e => e.ID == id);

                if (periodFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionAnswer.Remove(periodFromDb);
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