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
    public class EntityController : Controller
    {
        private readonly PostgreDbContext _db;
        public EntityController(PostgreDbContext db)
        {
            _db = db;
        }

        [HttpGet("entity/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1)
        {
            try
            {
                var data = await _db.Entities
                    .OrderBy(x=>x.Name)
                    .Skip((page-1)*10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach(var row in data)
                {
                    rows.Add(new RowModel { ID = row.ID, Value = new string[] { row.Name, row.Level.ToString(), row.ParentEntity != null ? row.ParentEntity.Name : "-" }});
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

        [HttpGet("entity/table-paging-view")]
        public IActionResult GetPaging(int page = 1)
        {
            try
            {
                var total = _db.Entities.Count();
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

        [HttpGet("entity/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0)
        {
            try
            {
                Entity entityFromDb = null;
                if(id != 0)
                {
                    entityFromDb = await _db.Entities.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();
                var entities = await _db.Entities
                    .Where(x=>x.ID != id && (id == 0 ? true : x.Level < entityFromDb.Level))
                    .OrderBy(x => x.Name)
                    .ToDictionaryAsync(x => x.ID.ToString(), y => y.Name);

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = entityFromDb == null ? "0" : entityFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Nama", Name = "Name", InputType = InputType.TEXT, Value = entityFromDb == null ? "" : entityFromDb.Name });
                FormModels.Add(new FormModel { Label = "Level", Name = "Level", InputType = InputType.NUMBER, Value = entityFromDb == null ? "0" : entityFromDb.Level.ToString() });
                FormModels.Add(new FormModel { Label = "Entitas Induk", Name = "ParentEntity", InputType = InputType.DROPDOWN, Options = entities, Value = entityFromDb == null || entityFromDb.ParentEntity == null ? "" : entityFromDb.ParentEntity.ID.ToString() });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("entity")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Entitas";
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Nama", Name = "Name", Style = "width: 15%; min-width: 200px" });
            ColumnModels.Add(new ColumnModel { Label = "Level", Name = "Level" });
            ColumnModels.Add(new ColumnModel { Label = "Entitas Induk", Name = "ParentEntity" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "entity.js";

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("entity/save")]
        public async Task<IActionResult> Save(Entity entity)
        {
            try
            {
                Entity entityFromDb = await _db.Entities.FirstOrDefaultAsync(e => e.ID == entity.ID);

                if (entityFromDb == null)
                {
                    if(entity.ParentEntity != null && entity.ParentEntity != null)
                    {
                        entity.ParentEntity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == entity.ParentEntity.ID);
                    }

                    _db.Entities.Add(entity);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    if (entity.ParentEntity != null && entity.ParentEntity != null)
                    {
                        entity.ParentEntity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == entity.ParentEntity.ID);
                    }

                    entityFromDb.Name = entity.Name;
                    entityFromDb.Level = entity.Level;
                    _db.Entities.Update(entityFromDb);
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

        [HttpPost("entity/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entityFromDb = await _db.Entities.FirstOrDefaultAsync(e => e.ID == id);

                if (entityFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.Entities.Remove(entityFromDb);
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
