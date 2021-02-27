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
    public class VerticalDimentionController : Controller
    {
        private readonly PostgreDbContext _db;
        public VerticalDimentionController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("vertical-dimention/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.VerticalDimentions
                    .Include("Section")
                    .Include("SubVerticalDimentions")
                    .OrderBy(x=>x.Section.Construct)
                    .ThenBy(x=>x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    var subVerticalDimention = row.SubVerticalDimentions.Count();
                    rows.Add(new RowModel { 
                        ID = row.ID, 
                        Value = new string[] { 
                            row.Section.Name, 
                            row.Name, 
                            row.Description, 
                            row.Sequence.ToString(), 
                            //row.ValueDriverDimention.ToString(),
                            "HTML:<a href='/sub-vertical-dimention/" + row.ID + "'>" + subVerticalDimention + " Sub Dimensi Vertical</a>"
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

        [HttpGet("vertical-dimention/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.VerticalDimentions.Count();
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

        [HttpGet("vertical-dimention/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                VerticalDimention verticalDimentionFromDb = null;
                if(id != 0)
                {
                    verticalDimentionFromDb = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == id);
                }

                var sections = await _db.Sections.OrderBy(x=>x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                //var drivens = new Dictionary<string, string>()
                //{
                //    {((int)ValueDriverDimention.AMANAH).ToString(), ValueDriverDimention.AMANAH.ToString()},
                //    {((int)ValueDriverDimention.KOMPETEN).ToString(), ValueDriverDimention.KOMPETEN.ToString()},
                //    {((int)ValueDriverDimention.HARMONIS).ToString(), ValueDriverDimention.HARMONIS.ToString()},
                //    {((int)ValueDriverDimention.LOYAL).ToString(), ValueDriverDimention.LOYAL.ToString()},
                //    {((int)ValueDriverDimention.ADAPTIF).ToString(), ValueDriverDimention.ADAPTIF.ToString()},
                //    {((int)ValueDriverDimention.KOLABORATIF).ToString(), ValueDriverDimention.KOLABORATIF.ToString()},
                //    {((int)ValueDriverDimention.REPUTASI_ORGANISASI).ToString(), ValueDriverDimention.REPUTASI_ORGANISASI.ToString()},
                //    {((int)ValueDriverDimention.KEPEMIMPINAN).ToString(), ValueDriverDimention.KEPEMIMPINAN.ToString()},
                //    {((int)ValueDriverDimention.KARIR_DAN_PENGEMBANGAN_DIRI).ToString(), ValueDriverDimention.KARIR_DAN_PENGEMBANGAN_DIRI.ToString()},
                //    {((int)ValueDriverDimention.PEKERJAAN).ToString(), ValueDriverDimention.PEKERJAAN.ToString()},
                //    {((int)ValueDriverDimention.KEBUTUHAN_DASAR).ToString(), ValueDriverDimention.KEBUTUHAN_DASAR.ToString()},
                //    {((int)ValueDriverDimention.HUBUNGAN_SOSIAL).ToString(), ValueDriverDimention.HUBUNGAN_SOSIAL.ToString()},
                //};

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = verticalDimentionFromDb == null ? "0" : verticalDimentionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Konstruk", Name = "Section", InputType = InputType.DROPDOWN, Options = sections, Value = verticalDimentionFromDb == null ? "" : verticalDimentionFromDb.Section.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = verticalDimentionFromDb == null ? "" : verticalDimentionFromDb.Name, IsRequired = true });
                FormModels.Add(new FormModel { Label = "Keterangan", Name = "Description", InputType = InputType.TEXTAREA, Value = verticalDimentionFromDb == null ? "" : verticalDimentionFromDb.Description });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = verticalDimentionFromDb == null ? "0" : verticalDimentionFromDb.Sequence.ToString(), IsRequired = true });
                //FormModels.Add(new FormModel { Label = "Nilai", Name = "ValueDriverDimention", InputType = InputType.DROPDOWN, Options = drivens, Value = verticalDimentionFromDb == null ? "" : ((int)verticalDimentionFromDb.ValueDriverDimention).ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("vertical-dimention")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Dimensi Vertikal";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Section", Style = "width: 10%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Keterangan", Name = "Description" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence" });
            //ColumnModels.Add(new ColumnModel { Label = "Dimensi", Name = "SituationEvpDimention" });
            ColumnModels.Add(new ColumnModel { Label = "Sub Dimensi Vertical", Name = "SubVerticalDimentions" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "vertical-dimention.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("vertical-dimention/save")]
        public async Task<IActionResult> Save(VerticalDimention verticalDimention)
        {
            try
            {
                VerticalDimention verticalDimentionFromDb = await _db.VerticalDimentions
                    .Include("Section")
                    .FirstOrDefaultAsync(e => e.ID == verticalDimention.ID);

                if (verticalDimentionFromDb == null)
                {
                    verticalDimention.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == verticalDimention.Section.ID);

                    _db.VerticalDimentions.Add(verticalDimention);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    verticalDimentionFromDb.Section = await _db.Sections.FirstOrDefaultAsync(e => e.ID == verticalDimention.Section.ID);

                    verticalDimentionFromDb.Name = verticalDimention.Name;
                    verticalDimentionFromDb.Description = verticalDimention.Description;
                    verticalDimentionFromDb.Sequence = verticalDimention.Sequence;
                    verticalDimentionFromDb.ValueDriverDimention = verticalDimention.ValueDriverDimention;

                    _db.VerticalDimentions.Update(verticalDimentionFromDb);
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

        [HttpPost("vertical-dimention/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var verticalDimentionFromDb = await _db.VerticalDimentions.FirstOrDefaultAsync(e => e.ID == id);

                if (verticalDimentionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.VerticalDimentions.Remove(verticalDimentionFromDb);
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
