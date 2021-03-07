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
    public class AssesmentController : Controller
    {
        private readonly PostgreDbContext _db;
        public AssesmentController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("assesment/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            { 
                var data = await _db.Assesments
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Name, row.Description }});
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

        [HttpGet("assesment/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Assesments.Count();
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

        [HttpGet("assesment/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Assesment assesmentFromDb = null;
                if(id != 0)
                {
                    assesmentFromDb = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = assesmentFromDb == null ? "0" : assesmentFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = assesmentFromDb == null ? "" : assesmentFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Keterangan", Name = "Description", InputType = InputType.TEXTAREA, Value = assesmentFromDb == null ? "" : assesmentFromDb.Description });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("assesment")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Jenis Survei";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Keterangan", Name = "Description" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "assesment.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("assesment/save")]
        public async Task<IActionResult> Save(Assesment assesment)
        {
            try
            {
                Assesment assesmentFromDb = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == assesment.ID);

                if (assesmentFromDb == null)
                {
                    _db.Assesments.Add(assesment);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    assesmentFromDb.Name = assesment.Name;
                    assesmentFromDb.Description = assesment.Description;
                    _db.Assesments.Update(assesmentFromDb);
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

        [HttpPost("assesment/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var assesmentFromDb = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == id);

                if (assesmentFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Assesments.Remove(assesmentFromDb);
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
