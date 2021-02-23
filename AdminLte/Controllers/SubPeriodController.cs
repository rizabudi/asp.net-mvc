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
    public class SubPeriodController : Controller
    {
        private readonly PostgreDbContext _db;
        public SubPeriodController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("sub-period/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int periodID = 0)
        {
            try
            {
                var data = await _db.SubPeriods
                    .Include("Period")
                    .Where(x=> x.Period.ID == periodID)
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Name, 
                            row.Start.ToString("yyyy-MM-dd") + " s/d " + row.End.ToString("yyyy-MM-dd"),
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

        [HttpGet("sub-period/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.SubPeriods.Count();
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

        [HttpGet("sub-period/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                SubPeriod periodFromDb = null;
                if(id != 0)
                {
                    periodFromDb = await _db.SubPeriods.FirstOrDefaultAsync(e => e.ID == id);
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

        [HttpGet("sub-period")]
        [Route("sub-period/{periodID:int}")]
        public async Task<IActionResult> IndexAsync(int periodID)
        {
            var periode = await _db.Periods.FirstOrDefaultAsync(x => x.ID == periodID);

            if (periode == null)
            {
                return Redirect("/home/errors/404");
            }

            ViewData["Title"] = "Sub Periode | " + periode.Name;
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Tanggal Mulai & Selesai", Name = "Date" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "sub-period.js";
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Periode", "/period"}
            }; 
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"Period", periodID.ToString()},
                {"PeriodStart", periode.Start.ToString("yyyy-MM-dd") },
                {"PeriodEnd", periode.End.ToString("yyyy-MM-dd") }
            };

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("sub-period/save")]
        public async Task<IActionResult> Save(SubPeriod subPeriod)
        {
            try
            {
                SubPeriod periodFromDb = await _db.SubPeriods.FirstOrDefaultAsync(e => e.ID == subPeriod.ID);

                if (periodFromDb == null)
                {
                    subPeriod.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == subPeriod.Period.ID);

                    _db.SubPeriods.Add(subPeriod);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    subPeriod.Period = await _db.Periods.FirstOrDefaultAsync(e => e.ID == subPeriod.Period.ID);

                    periodFromDb.Name = subPeriod.Name;
                    periodFromDb.Start = subPeriod.Start;
                    periodFromDb.End = subPeriod.End;
                    _db.SubPeriods.Update(periodFromDb);
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

        [HttpPost("sub-period/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var periodFromDb = await _db.SubPeriods.FirstOrDefaultAsync(e => e.ID == id);

                if (periodFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.SubPeriods.Remove(periodFromDb);
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
