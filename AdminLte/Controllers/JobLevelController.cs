using AdminLte.Data;
using AdminLte.Helpers;
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
    [CustomAuthFilter("Access_MasterData_StrukturOrganisasi_LevelJabatan")]
    public class JobLevelController : Controller
    {
        private readonly PostgreDbContext _db;
        public JobLevelController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("job-level/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.JobLevels
                    .OrderBy(x=>x.Level)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Name, row.Level.ToString() }});
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

        [HttpGet("job-level/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.JobLevels.Count();
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

        [HttpGet("job-level/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                JobLevel jobLevelFromDb = null;
                if(id != 0)
                {
                    jobLevelFromDb = await _db.JobLevels.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = jobLevelFromDb == null ? "0" : jobLevelFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = jobLevelFromDb == null ? "" : jobLevelFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Level", Name = "Level", InputType = InputType.NUMBER, Value = jobLevelFromDb == null ? "" : jobLevelFromDb.Level.ToString(), IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("job-level")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Level Jabatan";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Level", Name = "Level" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "job-level.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("job-level/save")]
        public async Task<IActionResult> Save(JobLevel jobLevel)
        {
            try
            {
                JobLevel jobLevelFromDb = await _db.JobLevels.FirstOrDefaultAsync(e => e.ID == jobLevel.ID);

                if (jobLevelFromDb == null)
                {
                    _db.JobLevels.Add(jobLevel);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    jobLevelFromDb.Name = jobLevel.Name;
                    _db.JobLevels.Update(jobLevel);
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

        [HttpPost("job-level/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var positionFromDb = await _db.JobLevels.FirstOrDefaultAsync(e => e.ID == id);

                if (positionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.JobLevels.Remove(positionFromDb);
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
