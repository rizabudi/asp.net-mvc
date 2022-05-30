using AdminLte.Data;
using AdminLte.Helpers;
using AdminLte.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLte.Controllers
{
    [Authorize(Roles = "Pengguna Khusus")]
    [CustomAuthFilter("Access_MasterData_DaftarSurvei")]
    public class SurveyEntityController : Controller
    {
        private readonly ApplicationDbContext _db;
        public SurveyEntityController(ApplicationDbContext db)
        {
            _db = db;
        }



        [HttpGet("survey-entity/table-data-view")]
        public async Task<IActionResult> GetAll(int page = 1, int surveyID = 0)
        {
            try
            {
                var data = await _db.QuestionPackageEntities
                    .Include(x => x.QuestionPackage)
                    .Include(x => x.Entity)
                    .Where(x => x.QuestionPackage.ID == surveyID)
                    .OrderBy(x => x.Entity.ID)
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .ToListAsync();

                var rows = new List<RowModel>();
                foreach (var row in data)
                {
                    string prefix = "";
                    for (int i = 0; i < row.Entity.Level; i++)
                    {
                        prefix += "-- ";
                    }

                    rows.Add(new RowModel
                    {
                        ID = row.ID,
                        Value = new string[] {
                            prefix + row.Entity.Name,
                            row.EmployeeCount.ToString("n0"),
                            row.TargetRespondent.ToString("n0")
                        }
                    });
                }

                ViewData["Rows"] = rows;
                ViewData["Page"] = page;
                ViewData["IsEditable"] = false;

                return PartialView("~/Views/Shared/_TableData.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey-entity/table-paging-view")]
        public IActionResult GetPaging(int page = 1, int surveyID = 0)
        {
            try
            {
                var total = _db.QuestionPackageEntities
                    .Where(x => x.QuestionPackage.ID == surveyID)
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

        [HttpGet("survey-entity/form-view")]
        public async Task<IActionResult> GetFormAsync(int id = 0, int surveyID = 0)
        {
            try
            {
                var questionPackage = await _db.QuestionPackages
                    .Include(x => x.Assesment)
                    .FirstOrDefaultAsync(x => x.ID == surveyID);

                if (questionPackage == null)
                {
                    return null;
                }

                QuestionPackageEntity questionPackageEntityFromDb = null;
                if (id != 0)
                {
                    questionPackageEntityFromDb = await _db.QuestionPackageEntities.FirstOrDefaultAsync(e => e.ID == id);
                }
                List<FormModel> FormModels = new List<FormModel>();

                int entityID = 0;
                byte[] bytes;
                if (HttpContext.Session.TryGetValue("User_Entity", out bytes))
                {
                    string value = Encoding.ASCII.GetString(bytes);
                    int.TryParse(value, out entityID);
                }

                var entityList = await _db.Entities
                    .Where(x => (entityID == 0 ? true : x.ID == entityID))
                    .OrderBy(x => x.Name).ToListAsync();
                var entities = Entity.getEntities(entityList, 0, 0);

                FormModels.Add(new FormModel { Label = "ID", Name = "ID", InputType = InputType.HIDDEN, Value = questionPackageEntityFromDb == null ? "0" : questionPackageEntityFromDb.ID.ToString() });
                FormModels.Add(new FormModel { Label = "Entitas", Name = "Entity", InputType = InputType.DROPDOWN, Options = entities, Value = questionPackageEntityFromDb == null ? "" : questionPackageEntityFromDb.Entity.ID.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Jumlah Karyawan", Name = "EmployeeCount", InputType = InputType.NUMBER_POSITIVE, Value = questionPackageEntityFromDb == null ? "" : questionPackageEntityFromDb.EmployeeCount.ToString(), IsRequired = true });
                FormModels.Add(new FormModel { Label = "Target Responden", Name = "TargetRespondent", InputType = InputType.NUMBER_POSITIVE, Value = questionPackageEntityFromDb == null ? "" : questionPackageEntityFromDb.TargetRespondent.ToString(), IsRequired = true });

                ViewData["Forms"] = FormModels;

                return PartialView("~/Views/Shared/_FormView.cshtml");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        [HttpGet("survey-entity")]
        [Route("survey-entity/{surveyID:int}")]
        public async Task<IActionResult> IndexAsync(int surveyID)
        {
            var questionPackage = await _db.QuestionPackages
                .Include(x => x.Assesment)
                .FirstOrDefaultAsync(x => x.ID == surveyID);

            if (questionPackage == null)
            {
                return Redirect("/home/errors/404");
            }

            ViewData["Title"] = "Daftar Entitas | " + questionPackage.Assesment.Name + " - " + questionPackage.Name;
            List<ColumnModel> ColumnModels = new List<ColumnModel>();
            ColumnModels.Add(new ColumnModel { Label = "Entitas", Name = "Entity", Style = "width: 40%; min-width: 400px" });
            ColumnModels.Add(new ColumnModel { Label = "Jumlah Karyawan", Name = "EmployeeCount", Style = "width: 15%; min-width: 150px" });
            ColumnModels.Add(new ColumnModel { Label = "Target Responden", Name = "TargetRespondent", Style = "width: 15%; min-width: 150px" });

            ViewData["Columns"] = ColumnModels;
            ViewData["Script"] = "survey-entity.js";
            ViewData["BreadCrump"] = new Dictionary<string, string>()
            {
                {"Daftar Survey", "survey"}
            };
            ViewData["Values"] = new Dictionary<string, string>()
            {
                {"QuestionPackage", surveyID.ToString()}
            };

            return View("~/Views/Shared/_Index.cshtml");
        }

        [HttpPost("survey-entity/save")]
        public async Task<IActionResult> Save(QuestionPackageEntity questionPackageEntity)
        {
            try
            {
                QuestionPackageEntity questionPackageEntityFromDb = await _db.QuestionPackageEntities.FirstOrDefaultAsync(e => e.ID == questionPackageEntity.ID);

                if (questionPackageEntityFromDb == null)
                {
                    questionPackageEntity.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackageEntity.QuestionPackage.ID);
                    questionPackageEntity.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == questionPackageEntity.Entity.ID);

                    _db.QuestionPackageEntities.Add(questionPackageEntity);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Data berhasil disimpan" });
                }
                else
                {
                    questionPackageEntityFromDb.QuestionPackage = await _db.QuestionPackages.FirstOrDefaultAsync(e => e.ID == questionPackageEntity.QuestionPackage.ID);
                    questionPackageEntityFromDb.Entity = await _db.Entities.FirstOrDefaultAsync(e => e.ID == questionPackageEntity.Entity.ID);

                    _db.QuestionPackageEntities.Update(questionPackageEntityFromDb);
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

        [HttpPost("survey-entity/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var questionPackageEntityFromDb = await _db.QuestionPackageEntities.FirstOrDefaultAsync(e => e.ID == id);

                if (questionPackageEntityFromDb == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan" });
                }

                _db.QuestionPackageEntities.Remove(questionPackageEntityFromDb);
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
