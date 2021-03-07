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
    public class SurveyScheduleController : Controller
    {
        private readonly PostgreDbContext _db;
        public SurveyScheduleController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("survey-schedule/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.QuestionPackagePeriods
                    .Include(x=>x.QuestionPackage)
                    .Include(x => x.QuestionPackage.Assesment)
                    .Include(x => x.Period)
                    .OrderBy(x=>x.Period.Start)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.QuestionPackage.Assesment.Name + " - " + row.QuestionPackage.Name,
                            row.Period.Start.ToString("yyyy-MM-dd") + " s/d " + row.Period.End.ToString("yyyy-MM-dd"),
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

        [HttpGet("survey-schedule/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.QuestionPackagePeriods.Count();
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

        [HttpGet("survey-schedule/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                QuestionPackagePeriod questionPackagePeriodFromDb = null;
                if(id != 0)
                {
                    questionPackagePeriodFromDb = await _db.QuestionPackagePeriods.FirstOrDefaultAsync(e => e.ID == id);
                }
                var questionPackages = await _db.QuestionPackages
                    .Include(x=>x.Assesment)
                    .OrderBy(x=>x.Assesment.Name)
                    .ThenBy(x => x.Name)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Assesment.Name + " - " + y.Name);

                var periodes = await _db.Periods
                    .OrderBy(x => x.Name)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Name + " - " + y.Start.ToString("yyyy-MM-dd") + " s/d " + y.End.ToString("yyyy-MM-dd"));

                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionPackagePeriodFromDb == null ? "0" : questionPackagePeriodFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Survei", Name = "QuestionPackage", InputType = InputType.DROPDOWN, Options = questionPackages, Value = questionPackagePeriodFromDb == null ? "" : questionPackagePeriodFromDb.QuestionPackage.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Periode", Name = "Period", InputType = InputType.DROPDOWN, Options = periodes, Value = questionPackagePeriodFromDb == null ? "" : questionPackagePeriodFromDb.Period.ID.ToString(), IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey-schedule")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Penjadwalan Survey";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Survei", Name = "Assesment", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Periode", Name = "Name" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "survey-schedule.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("survey-schedule/save")]
        public async Task<IActionResult> Save(QuestionPackagePeriod questionPackagePeriod)
        {
            try
            {
                QuestionPackagePeriod questionPackageFromDb = await _db.QuestionPackagePeriods.FirstOrDefaultAsync(e => e.ID == questionPackagePeriod.ID);

                if (questionPackageFromDb == null)
                {
                    questionPackagePeriod.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackagePeriod.QuestionPackage.ID);
                    questionPackagePeriod.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == questionPackagePeriod.Period.ID);

                    _db.QuestionPackagePeriods.Add(questionPackagePeriod);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionPackageFromDb.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackageFromDb.QuestionPackage.ID);
                    questionPackageFromDb.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == questionPackageFromDb.Period.ID);

                    _db.QuestionPackagePeriods.Update(questionPackageFromDb);
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

        [HttpPost("survey-schedule/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionPackagePeriodFromDb = await _db.QuestionPackagePeriods.FirstOrDefaultAsync(e => e.ID == id);

                if (questionPackagePeriodFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionPackagePeriods.Remove(questionPackagePeriodFromDb);
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
