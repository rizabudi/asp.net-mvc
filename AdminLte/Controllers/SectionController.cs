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
    [CustomAuthFilter("Access_MasterData_Konstruk")]
    public class SectionController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SectionController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("section/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Sections
                    .Include("Assesment")
                    .OrderBy(x=>x.Sequence)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Assesment.Name, row.Name, row.Description, row.Sequence.ToString(), row.IsRandom ? "Ya" : "Tidak", row.Construct.ToString() }});
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

        [HttpGet("section/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Sections.Count();
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

        [HttpGet("section/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Section sectionFromDb = null;
                if(id != 0)
                {
                    sectionFromDb = await _db.Sections.FirstOrDefaultAsync(e => e.ID == id);
                }

                var assesments = await _db.Assesments.OrderBy(x=>x.Name).ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);
                var constructs = new Dictionary<string, string>()
                {
                    {((int)Construct.CULTURE).ToString(), Construct.CULTURE.ToString() },
                    {((int)Construct.PERFORMANCE).ToString(), Construct.PERFORMANCE.ToString() },
                    {((int)Construct.ENGAGEMENT).ToString(), Construct.ENGAGEMENT.ToString() },
                };

                List<FormModel> FormModels = new List<FormModel>();
                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = sectionFromDb == null ? "0" : sectionFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Jenis Survei", Name = "Assesment", InputType = InputType.DROPDOWN, Options = assesments, Value = sectionFromDb == null ? "" : sectionFromDb.Assesment.ID.ToString(), IsRequired = true, FormPosition = FormPosition.LEFT });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = sectionFromDb == null ? "" : sectionFromDb.Name, IsRequired = true, FormPosition = FormPosition.LEFT });
                FormModels.Add(new FormModel { Label = "Keterangan", Name = "Description", InputType = InputType.TEXTAREA, Value = sectionFromDb == null ? "" : sectionFromDb.Description, FormPosition = FormPosition.LEFT });
                FormModels.Add(new FormModel { Label = "Urutan", Name = "Sequence", InputType = InputType.NUMBER, Value = sectionFromDb == null ? "0" : sectionFromDb.Sequence.ToString(), FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Soal Acak", Name = "IsRandom", InputType = InputType.YESNO, Value = sectionFromDb == null ? "0" : sectionFromDb.IsRandom ? "1" : "0", FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Konstruk", Name = "Construct", InputType = InputType.DROPDOWN, Options = constructs, Value = sectionFromDb == null ? "" : ((int)sectionFromDb.Construct).ToString(), FormPosition = FormPosition.RIGHT });
                FormModels.Add(new FormModel { Label = "Narasi", Name = "Introduction", InputType = InputType.WYSIWYG, Value = sectionFromDb == null ? "" : sectionFromDb.Introduction, FormPosition = FormPosition.FULL });

                ViewData["Forms"] = FormModels;
                ViewData["ColumnNumber"] = 2;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("section")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Konstruk";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Jenis Survei", Name = "Assesment", Style = "width: 10%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name" });
            ColumnModels.Add(new ColumnModel { Label = "Keterangan", Name = "Description" });
            ColumnModels.Add(new ColumnModel { Label = "Urutan", Name = "Sequence" });
            ColumnModels.Add(new ColumnModel { Label = "Soal Acak", Name = "IsRandom" });
            ColumnModels.Add(new ColumnModel { Label = "Konstruk", Name = "Construct" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "section.js";
            ViewData["ModalStye"] = "modal-xl";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("section/save")]
        public async Task<IActionResult> Save(Section section)
        {
            try
            {
                Section sectionFromDb = await _db.Sections
                    .Include("Assesment")
                    .FirstOrDefaultAsync(e => e.ID == section.ID);

                if (sectionFromDb == null)
                {
                    section.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == section.Assesment.ID);

                    _db.Sections.Add(section);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    sectionFromDb.Assesment = await _db.Assesments.FirstOrDefaultAsync(e => e.ID == section.Assesment.ID);

                    sectionFromDb.Name = section.Name;
                    sectionFromDb.Description = section.Description;
                    sectionFromDb.Sequence = section.Sequence;
                    sectionFromDb.IsRandom = section.IsRandom;
                    sectionFromDb.Construct = section.Construct;
                    sectionFromDb.Introduction = section.Introduction;

                    _db.Sections.Update(sectionFromDb);
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

        [HttpPost("section/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var sectionFromDb = await _db.Sections.FirstOrDefaultAsync(e => e.ID == id);

                if (sectionFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Sections.Remove(sectionFromDb);
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
