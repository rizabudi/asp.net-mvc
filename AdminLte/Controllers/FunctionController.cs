using AdminLte.Data;
using AdminLte.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    public class FunctionController : Controller
    {
        private readonly PostgreDbContext _db;
        public FunctionController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("function/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.CompanyFunctions
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Name }});
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

        [HttpGet("function/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.CompanyFunctions.Count();
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

        [HttpGet("function/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                CompanyFunction functionFromDb = null;
                if(id != 0)
                {
                    functionFromDb = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
               
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = functionFromDb == null ? "0" : functionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = functionFromDb == null ? "" : functionFromDb.Name });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("function")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Posisi";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "function.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("function/save")]
        public async Task<IActionResult> Save(CompanyFunction function)
        {
            try
            {
                CompanyFunction functionFromDb = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == function.ID);

                if (functionFromDb == null)
                {
                    _db.CompanyFunctions.Add(function);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    functionFromDb.Name = function.Name;
                    _db.CompanyFunctions.Update(functionFromDb);
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

        [HttpPost("function/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var functionFromDb = await _db.CompanyFunctions.FirstOrDefaultAsync(e => e.ID == id);

                if (functionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.CompanyFunctions.Remove(functionFromDb);
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
