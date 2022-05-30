using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_MasterData_Pertanyaan")]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _db;
        private IWebHostEnvironment _hostingEnvironment;

        public QuestionController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _hostingEnvironment = environment;
        }

        [HttpGet("question/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Questions
                    .Include(x => x.Section)
                    .Include(x => x.Section.Assesment)
                    .OrderBy(x => x.Section.Construct)
                    .ThenBy(x => x.Section.Sequence)
                    .ThenBy(x=>x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var answers = await _db.QuestionAnswer.Where(x => data.Contains(x.Question) || data.Contains(x.MatrixQuestion)).ToListAsync();

                var rows = new List<RowModel>();
                foreach (var row in data)
                {
                    var answer = answers.Where(x => (x.Question != null && x.Question.ID == row.ID) || (x.MatrixQuestion != null && x.MatrixQuestion.ID == row.ID)).Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Section.Name + " - " + row.Section.Assesment.Name,
                            row.Sequence.ToString(),
                            row.Title,
                            "IMAGE:"+row.Attachment,
                            row.QuestionType.ToString() + " - " + row.MatrixSubType.ToString(),
                            row.IsMandatory ? "Iya" : "Tidak",
                            row.IsRandomAnswer ? "Iya" : "Tidak",
                            "HTML:<a href='question-answer/" + row.ID + "'>" + answer + " Jawaban</a>"
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

                var sections = await _db.Sections
                    .Include("Assesment")
                    .OrderBy(x => x.Assesment.Name + " - " + x.Name)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Assesment.Name + " - " + y.Name);
                var questionTypes = new Dictionary<string, string>()
                {
                    {((int)QuestionType.MATRIX).ToString(), QuestionType.MATRIX.ToString()},
                    {((int)QuestionType.SIMPLE_CHOICE).ToString(), QuestionType.SIMPLE_CHOICE.ToString()}
                };
                var matrixSubTypes = new Dictionary<string, string>()
                {
                    {((int)MatrixSubType.SIMPLE).ToString(), MatrixSubType.SIMPLE.ToString()},
                    {((int)MatrixSubType.CUSTOM).ToString(), MatrixSubType.CUSTOM.ToString()}
                };

                string image = "";
                if(questionFromDb != null && questionFromDb.Attachment != null)
                {
                    try
                    {
                        string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                        string filePath = Path.Combine(uploads, questionFromDb.Attachment);
                        byte[] b = System.IO.File.ReadAllBytes(filePath);
                        image = "data:image/png;base64," + Convert.ToBase64String(b);
                    } 
                    catch (Exception)
                    {
                    }
                }

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionFromDb == null ? "0" : questionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Konstruk", Name = "Section", InputType = InputType.DROPDOWN, Options = sections, Value = questionFromDb == null ? "" : questionFromDb.Section.ID.ToString(), FormPosition = FormPosition.LEFT, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = questionFromDb == null ? "0" : questionFromDb.Sequence.ToString(), FormPosition = FormPosition.RIGHT, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Tipe Soal", Name = "QuestionType", InputType = InputType.DROPDOWN, Options = questionTypes, Value = questionFromDb == null ? "" : ((int)questionFromDb.QuestionType).ToString() });
                FormModels.Add(new FormModel { Label = "Tipe Soal Matrix", Name = "MatrixSubtype", InputType = InputType.DROPDOWN, Options = matrixSubTypes, Value = questionFromDb == null ? "" : ((int)questionFromDb.MatrixSubType).ToString() });
                FormModels.Add(new FormModel { Label = "Harus Diisi", Name = "IsMandatory", InputType = InputType.YESNO, Value = questionFromDb == null ? "" : questionFromDb.IsMandatory ? "1" : "0", FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Jawaban Acak", Name = "IsRandomAnswer", InputType = InputType.YESNO, Value = questionFromDb == null ? "" : questionFromDb.IsRandomAnswer ? "1" : "0", FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Judul Soal", Name = "Title", InputType = InputType.TEXTAREA, Value = questionFromDb == null ? "" : questionFromDb.Title, FormPosition = FormPosition.LEFT, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Gambar", Name = "Attachment", InputType = InputType.IMAGE, Value = image, FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Isi Soal", Name = "Description", InputType = InputType.WYSIWYG, Value = questionFromDb == null ? "" : questionFromDb.Description, FormPosition = FormPosition.FULL });

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

        [HttpGet("question")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Pertanyaan";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Section", Style = "width: 10%; min-width: 100px" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence", Style = "width: 5%; min-width: 50px" });
            ColumnModels.Add(new ColumnModel { Label = "Soal", Name = "Title", Style = "width: 25%; min-width: 250px" });
            ColumnModels.Add(new ColumnModel { Label = "Gambar", Name = "Attachment" });
            ColumnModels.Add(new ColumnModel { Label = "Tipe Soal", Name = "QuestionType" });
            ColumnModels.Add(new ColumnModel { Label = "Harus Diisi", Name = "IsMandatory" });
            ColumnModels.Add(new ColumnModel { Label = "Jawaban Acak", Name = "IsRandomAnswer" });
            ColumnModels.Add(new ColumnModel { Label = "Daftar Jawaban", Name = "QuestionAnswer" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "question.js";
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("question/save")]
        public async Task<IActionResult> Save(Question question)
        {
            try
            {
                Question questionFromDb = await _db.Questions
                    .Include("Section")
                    .FirstOrDefaultAsync(e => e.ID == question.ID);

                if (questionFromDb == null)
                {
                    question.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == question.Section.ID);

                    _db.Questions.Add(question);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionFromDb.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == question.Section.ID);

                    questionFromDb.Sequence = question.Sequence;
                    questionFromDb.QuestionType = question.QuestionType;
                    questionFromDb.MatrixSubType = question.MatrixSubType;
                    questionFromDb.IsMandatory = question.IsMandatory;
                    questionFromDb.IsRandomAnswer = question.IsRandomAnswer;
                    questionFromDb.Title = question.Title;
                    questionFromDb.Description = question.Description;

                    if(question.Attachment != null)
                    {
                        questionFromDb.Attachment = question.Attachment;
                    }

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

                var questionAnswersFromDb = await _db.QuestionAnswer.Where(e => e.Question.ID == id).ToListAsync();
                var questionAnswersFromDb1 = await _db.QuestionAnswer.Where(e => e.MatrixQuestion.ID == id).ToListAsync();
                
                _db.QuestionAnswer.RemoveRange(questionAnswersFromDb);
                _db.QuestionAnswer.RemoveRange(questionAnswersFromDb1);

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

        [HttpPost("question/upload-image")]
        public async Task<IActionResult> UploadImageAsync(IFormFile image)
        {
            try
            {
                string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (image != null)
                {
                    string filePath = Path.Combine(uploads, image.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    byte[] b = System.IO.File.ReadAllBytes(filePath);
                    return Json(new { success = true, data = new { base64 = "data:image/png;base64," + Convert.ToBase64String(b), filename = image.FileName } });
                }
                else
                {
                    return Json(new { success = false, message = "File tidak ditemukan" });
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }
    }
}
