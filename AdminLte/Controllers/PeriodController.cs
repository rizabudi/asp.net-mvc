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
    public class PeriodController : Controller
    {
        private readonly PostgreDbContext _db;
        public PeriodController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("period/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Periods
                    .Include("SubPeriods")
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var subPeriods = row.SubPeriods.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Name, 
                            row.Start.ToString("yyyy-MM-dd") + " s/d " + row.End.ToString("yyyy-MM-dd"),
                            "HTML:<a href='/sub-period/" + row.ID + "'>" + subPeriods + " Sub Periode</a>"
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

        [HttpGet("period/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Periods.Count();
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

        [HttpGet("period/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Period periodFromDb = null;
                if(id != 0)
                {
                    periodFromDb = await _db.Periods.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = periodFromDb == null ? "0" : periodFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = periodFromDb == null ? "" : periodFromDb.Name });
                FormModels.Add(new FormModel { Label = "Date", Name = "Date", InputType = InputType.TEXT, Value = periodFromDb == null ? "" : periodFromDb.Start.ToString("yyyy-MM-dd") + " s/d " + periodFromDb.End.ToString("yyyy-MM-dd") });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("period")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Periode";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Tanggal Mulai & Selesai", Name = "Date", Style = "width: 15%; min-width: 250px" });
            ColumnModels.Add(new ColumnModel { Label = "Sub Periode", Name = "SubPeriode" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "period.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("period/save")]
        public async Task<IActionResult> Save(Period period)
        {
            try
            {
                Period periodFromDb = await _db.Periods.FirstOrDefaultAsync(e => e.ID == period.ID);

                if (periodFromDb == null)
                {
                    _db.Periods.Add(period);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    periodFromDb.Name = period.Name;
                    periodFromDb.Start = period.Start;
                    periodFromDb.End = period.End;
                    _db.Periods.Update(periodFromDb);
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

        [HttpPost("period/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var periodFromDb = await _db.Periods.FirstOrDefaultAsync(e => e.ID == id);

                if (periodFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Periods.Remove(periodFromDb);
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
