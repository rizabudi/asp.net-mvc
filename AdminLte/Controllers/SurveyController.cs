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
                            "HTML:<a href='/survey-question/" + row.ID + "'>" + questions + " Soal</a>"
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
    }
}
