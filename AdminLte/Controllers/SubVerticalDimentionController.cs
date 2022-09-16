using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_MasterData_DimensiVertical")]
    public class SubVerticalDimentionController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SubVerticalDimentionController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("sub-vertical-dimention/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int verticalDimentionID = 0)
        {
            try
            {
                var data = await _db.SubVerticalDimentions
                    .Include("VerticalDimention")
                    .Where(x=> x.VerticalDimention.ID == verticalDimentionID)
                    .OrderBy(x=>x.Sequence)
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
                            row.Description, 
                            row.Sequence.ToString(), 
                            row.ValueDriverDimention.ToString() 
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

        [HttpGet("sub-vertical-dimention/table-paging-view")]
        public IActionResult GetPaging(int page = 1, int verticalDimentionID = 0)
        {
            try
            {
                var total = _db.SubVerticalDimentions
                    .Where(x => x.VerticalDimention.ID == verticalDimentionID)
                    .Count();
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

        [HttpGet("sub-vertical-dimention/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                SubVerticalDimention subVerticalFromDb = null;
                if(id != 0)
                {
                    subVerticalFromDb = await _db.SubVerticalDimentions.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
                var drivens = new Dictionary<string, string>()
                {
                    {((int)ValueDriverDimention.LEARNING).ToString(), ValueDriverDimention.LEARNING.ToString()},
                    {((int)ValueDriverDimention.GROWING).ToString(), ValueDriverDimention.GROWING.ToString()},
                    {((int)ValueDriverDimention.CONTRIBUTING).ToString(), ValueDriverDimention.CONTRIBUTING.ToString()},
                    {((int)ValueDriverDimention.EFISIENSI).ToString(), ValueDriverDimention.EFISIENSI.ToString()},
                    {((int)ValueDriverDimention.EFEKTIVITAS).ToString(), ValueDriverDimention.EFEKTIVITAS.ToString()},
                    {((int)ValueDriverDimention.KEADILAN).ToString(), ValueDriverDimention.KEADILAN.ToString()}
                };

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = subVerticalFromDb == null ? "0" : subVerticalFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = subVerticalFromDb == null ? "" : subVerticalFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Keterangan", Name = "Description", InputType = InputType.TEXTAREA, Value = subVerticalFromDb == null ? "" : subVerticalFromDb.Description });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = subVerticalFromDb == null ? "0" : subVerticalFromDb.Sequence.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Nilai", Name = "ValueDriverDimention", InputType = InputType.DROPDOWN, Options = drivens, Value = subVerticalFromDb == null ? "" : ((int)subVerticalFromDb.ValueDriverDimention).ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("sub-vertical-dimention")]
        [Route("sub-vertical-dimention/{verticalDimentionID:int}")]
        public async Task<IActionResult> IndexAsync(int verticalDimentionID)
        {
            var verticalDimention = await _db.VerticalDimentions.FirstOrDefaultAsync(x => x.ID == verticalDimentionID);

            if (verticalDimention == null)
            {
                return Redirect("/home/errors/404");
            }

            ViewData["Title"] = "Sub Dimensi Vertikal | " + verticalDimention.Name;
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Keterangan", Name = "Description" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence" });
            ColumnModels.Add(new ColumnModel { Label = "Dimensi", Name = "SituationEvpDimention" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "sub-vertical-dimention.js";
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Dimensi Vertikal", "vertical-dimention"}
            }; 
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"VerticalDimention", verticalDimentionID.ToString()},
            };

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("sub-vertical-dimention/save")]
        public async Task<IActionResult> Save(SubVerticalDimention subVerticalDimention)
        {
            try
            {
                SubVerticalDimention subVerticalFromDb = await _db.SubVerticalDimentions
                    .Include("Section")
                    .FirstOrDefaultAsync(e => e.ID == subVerticalDimention.ID);

                if (subVerticalFromDb == null)
                {
                    subVerticalDimention.VerticalDimention = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == subVerticalDimention.VerticalDimention.ID);

                    _db.SubVerticalDimentions.Add(subVerticalDimention);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    subVerticalDimention.VerticalDimention = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == subVerticalDimention.VerticalDimention.ID);

                    subVerticalFromDb.Name = subVerticalDimention.Name;
                    subVerticalFromDb.Name = subVerticalDimention.Name;
                    subVerticalFromDb.Description = subVerticalDimention.Description;
                    subVerticalFromDb.Sequence = subVerticalDimention.Sequence;
                    subVerticalFromDb.ValueDriverDimention = subVerticalDimention.ValueDriverDimention;

                    _db.SubVerticalDimentions.Update(subVerticalFromDb);
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

        [HttpPost("sub-vertical-dimention/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var subVerticalFromDb = await _db.SubVerticalDimentions.FirstOrDefaultAsync(e => e.ID == id);

                if (subVerticalFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.SubVerticalDimentions.Remove(subVerticalFromDb);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Terjadi kesalahan. Err : " + ex.Message });
            }
        }

        [HttpGet("sub-vertical-dimention/select-option")]
        public async Task<IActionResult> GetSelectOptions(int verticalDimentionID = 0)
        {
            try
            {
                var data = await _db.SubVerticalDimentions
                    .Include("VerticalDimention")
                    .Where(x => x.VerticalDimention.ID == verticalDimentionID)
                    .OrderBy(x => x.Sequence)
                    .Take(10)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.VerticalDimention.Name + " - " + y.Name);

                return PartialView("~/Views/Shared/_SelectOptionView.cshtml", data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
