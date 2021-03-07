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
    public class HorizontalDimentionController : Controller
    {
        private readonly PostgreDbContext _db;
        public HorizontalDimentionController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("horizontal-dimention/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.HorizontalDimentions
                    .Include("Section")
                    .OrderBy(x => x.Section.Construct)
                    .ThenBy(x => x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Section.Name, 
                            row.Name, 
                            row.Description, 
                            row.Sequence.ToString()
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

        [HttpGet("horizontal-dimention/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.HorizontalDimentions.Count();
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

        [HttpGet("horizontal-dimention/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                HorizontalDimention horizontalDimentionFromDb = null;
                if(id != 0)
                {
                    horizontalDimentionFromDb = await _db.HorizontalDimentions.FirstOrDefaultAsync(e => e.ID == id);
                }

                var sections = await _db.Sections.OrderBy(x=>x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                //var situations = new Dictionary<string, string>()
                //{
                //    { ((int)SituationEvpDimention.URUTAN).ToString(), SituationEvpDimention.URUTAN.ToString() },
                //    { ((int)SituationEvpDimention.NILAI).ToString(), SituationEvpDimention.NILAI.ToString() },
                //    { ((int)SituationEvpDimention.SAY).ToString(), SituationEvpDimention.SAY.ToString() },
                //    { ((int)SituationEvpDimention.STAY_LEARNING).ToString(), SituationEvpDimention.STAY_LEARNING.ToString() },
                //    { ((int)SituationEvpDimention.STAY_GROWING).ToString(), SituationEvpDimention.STAY_GROWING.ToString() },
                //    { ((int)SituationEvpDimention.STRIVE_CONTRIBUTING).ToString(), SituationEvpDimention.STRIVE_CONTRIBUTING.ToString() },
                //};

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = horizontalDimentionFromDb == null ? "0" : horizontalDimentionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Konstruk", Name = "Section", InputType = InputType.DROPDOWN, Options = sections, Value = horizontalDimentionFromDb == null ? "" : horizontalDimentionFromDb.Section.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = horizontalDimentionFromDb == null ? "" : horizontalDimentionFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Keterangan", Name = "Description", InputType = InputType.TEXTAREA, Value = horizontalDimentionFromDb == null ? "" : horizontalDimentionFromDb.Description });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = horizontalDimentionFromDb == null ? "0" : horizontalDimentionFromDb.Sequence.ToString(), IsRequired = true });
                //FormModels.Add(new FormModel { Label = "Dimensi", Name = "SituationEvpDimention", InputType = InputType.DROPDOWN, Options = situations, Value = horizontalDimentionFromDb == null ? "" : ((int)horizontalDimentionFromDb.SituationEvpDimention).ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("horizontal-dimention")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Dimensi Horizontal";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Section", Style = "width: 10%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Keterangan", Name = "Description" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence" });
            //ColumnModels.Add(new ColumnModel { Label = "Dimensi", Name = "SituationEvpDimention" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "horizontal-dimention.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("horizontal-dimention/save")]
        public async Task<IActionResult> Save(HorizontalDimention horizontalDimention)
        {
            try
            {
                HorizontalDimention horizontalDimentionFromDb = await _db.HorizontalDimentions
                    .Include("Section")
                    .FirstOrDefaultAsync(e => e.ID == horizontalDimention.ID);

                if (horizontalDimentionFromDb == null)
                {
                    horizontalDimention.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == horizontalDimention.Section.ID);

                    _db.HorizontalDimentions.Add(horizontalDimention);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    horizontalDimentionFromDb.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == horizontalDimention.Section.ID);

                    horizontalDimentionFromDb.Name = horizontalDimention.Name;
                    horizontalDimentionFromDb.Description = horizontalDimention.Description;
                    horizontalDimentionFromDb.Sequence = horizontalDimention.Sequence;
                    horizontalDimentionFromDb.SituationEvpDimention = horizontalDimention.SituationEvpDimention;

                    _db.HorizontalDimentions.Update(horizontalDimentionFromDb);
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

        [HttpPost("horizontal-dimention/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var horizontalDimentionFromDb = await _db.HorizontalDimentions.FirstOrDefaultAsync(e => e.ID == id);

                if (horizontalDimentionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.HorizontalDimentions.Remove(horizontalDimentionFromDb);
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
